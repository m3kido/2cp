using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections;

// Class to manage the cursor
public class CursorManager : MonoBehaviour
{
    #region Variables
    // Managers will be needed
    private UnitManager _um;
    private MapManager _mm;
    private BuildingManager _bm;
    private GameManager _gm;
    private Camera _camera;

    private float _cooldown=0.25f;
    private float _duration =0;
    private Vector3Int _lastOffset = Vector3Int.zero;
    private Vector3Int _offset = Vector3Int.zero;

    public static event Action OnCursorMove;

    // This is a property that holds the tile which the cursor is hovering over
    public Vector3Int HoveredOverTile
    {
        get => _mm.Map.WorldToCell(transform.position);
        set 
        { 
            transform.position = new Vector3Int( math.clamp( value.x,_mm.Map.cellBounds.xMin, _mm.Map.cellBounds.xMax-1), math.clamp(value.y, _mm.Map.cellBounds.yMin, _mm.Map.cellBounds.yMax-1),value.z); 
            
        }
    }

    // Auto-property (the compiler automatically creates a private field for it)
    public Vector3Int SaveTile { get; set; }
    public Vector3 SaveCamera { get; set; }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        // Get the unit, map, game and building managers from the hierarchy
        _um = FindAnyObjectByType<UnitManager>();
        _mm = FindAnyObjectByType<MapManager>();
        _gm = FindAnyObjectByType<GameManager>();
        _bm = FindAnyObjectByType<BuildingManager>();
        _camera = Camera.main;
        HoveredOverTile = _mm.Map.WorldToCell(transform.position);
    } 

    void Update()
    { 
        // Handle input every frame
        if(_gm.CurrentStateOfPlayer == EPlayerStates.Idle || _gm.CurrentStateOfPlayer == EPlayerStates.Selecting) {
            HandleInput();
        }
    }
    #endregion

    #region Methods
    // Handles keyboard input
    void HandleInput()
    {
        // Dont handle any input if a unit is moving or attacking
        if (_um.SelectedUnit!= null && _um.SelectedUnit.IsMoving) { return; }

        // X key
        if (Input.GetKeyDown(KeyCode.X))
        {
            XClicked();
        }

        // Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpaceClicked();
        }

        if (_duration > 0)
        {
            _duration -= Time.deltaTime;
        }
        // Arrow keys

        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            _offset = Vector3Int.up+Vector3Int.right;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            _offset = Vector3Int.right+Vector3Int.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            _offset = Vector3Int.left+ Vector3Int.up;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            _offset = Vector3Int.left + Vector3Int.down;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _offset = Vector3Int.right;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            _offset = Vector3Int.left;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            _offset = Vector3Int.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _offset = Vector3Int.down;
        }
        else
        {
            _lastOffset = _offset;
            _duration = 0.2f * _cooldown;
            return;
        }
        //this is just making sure that a diagnol movement works nicely
        bool diff = ((_offset.x == 0 || _offset.y==0)&& (_lastOffset.x != 0 && _lastOffset.y!=0) )|| ((_lastOffset.x == 0 || _lastOffset.y == 0) && (_offset.x != 0 && _offset.y != 0));
       

        if ((_offset != _lastOffset && !diff ) || _duration <= 0 )
        {
            if(_offset.x !=0 && _offset.y != 0)
            {
                MoveSelector(new Vector3Int(_offset.x, 0, 0));
                MoveSelector(new Vector3Int(0,_offset.y, 0));
            }
            else
            {
                MoveSelector(_offset);
            }
            if (_offset == _lastOffset)
            {
                _duration = 0.3f * _cooldown;

            }
            else
            {
                _duration = _cooldown;

            }
            _lastOffset = _offset;
           
        }

       
        

      
    }
   
    // Move the cursor 
     void MoveSelector(Vector3Int offset)
    {
        // Dont let the cursor move out of the highlited tiles
        if (_um.SelectedUnit != null && !_um.SelectedUnit.ValidTiles.ContainsKey(HoveredOverTile + offset))
        {
            return;
        }

        // If a unit is selected, record the path
        if (_um.SelectedUnit != null)
        {
            // Undraw the path if we get back the start point
            if (_um.SelectedUnit.transform.position == HoveredOverTile + offset)
            {
                _um.UndrawPath();
                _um.Path.Clear();
                _um.PathCost = 0;
            }
            else
            {
                int index = _um.Path.IndexOf(HoveredOverTile + offset); // Returns -1 if not found
                if (index < 0)
                {
                    if(_um.Path.Count >= _um.SelectedUnit.Data.MoveRange)
                    {
                        _um.CallPathfinding(HoveredOverTile + offset);
                    }
                    else
                    {
                        // Add tile to path
                        int cost = _mm.GetTileData(_mm.Map.GetTile<Tile>(HoveredOverTile + offset)).ProvisionsCost;
                        if (_um.PathCost + cost > _um.SelectedUnit.Provisions) { return; }
                        _um.UndrawPath();
                        _um.Path.Add(HoveredOverTile + offset);
                        _um.PathCost += cost;
                    }
                  
                }
                else
                {
                    // Remove the arrow loop
                    _um.UndrawPath();
                    _um.Path.RemoveRange(index + 1, _um.Path.Count - index - 1);

                    // Recalculate the new provisions cost
                    _um.PathCost = 0;
                    foreach (Vector3Int pos in _um.Path)
                    {
                        _um.PathCost += _mm.GetTileData(_mm.Map.GetTile<Tile>(pos)).ProvisionsCost;
                    }
                }
            }
            _um.DrawPath();
        }
        HoveredOverTile += offset;
        MoveCamera(offset);
        OnCursorMove?.Invoke();
        
    }

    // Handle X Click
    private void XClicked()
    {
        if(_gm.CurrentStateOfPlayer == EPlayerStates.Selecting) 
        {
            // Cancel selection
            HoveredOverTile = _mm.Map.WorldToCell(_um.SelectedUnit.transform.position);
            Camera.main.transform.position = SaveCamera;
            _um.DeselectUnit();       
        }
        else if (_gm.CurrentStateOfPlayer == EPlayerStates.Idle)
        {
            var unit = _um.FindUnit(HoveredOverTile);
            if (unit != null && unit is AttackingUnit)
            {
                (unit as AttackingUnit).AttackTiles();
                StartCoroutine(UnhighlightAttackTiles(unit));
            }

        }
    }
    private IEnumerator UnhighlightAttackTiles(Unit unit)
    {

        while (!Input.GetKeyUp(KeyCode.X))
        {
            yield return null;
        }
        unit.ResetTiles();

    }


    // Handle Space click
    private void SpaceClicked()
    {
        Unit refUnit = _um.FindUnit(HoveredOverTile);

        // If there is a unit on the hovered tile
        if (refUnit != null)
        {
            // Can't select an another unit when one is selected 
            if (_um.SelectedUnit != null)
            {
                bool loadcase = (refUnit is LoadingUnit) && (refUnit as LoadingUnit).CanLoadUnit(_um.SelectedUnit);
                if (_um.SelectedUnit == refUnit || loadcase ) {  StartCoroutine(_um.MoveUnit());  }
                return;
            }

            // Can't select an enemy unit
            if (refUnit.Owner != _gm.PlayerTurn || refUnit.HasMoved) {
                _gm.CurrentStateOfPlayer = EPlayerStates.InSettingsMenu;
                return; 
            }

           
            SaveTile = HoveredOverTile;
            SaveCamera = _camera.transform.position;
            _um.SelectUnit(refUnit);
        }
        else
        {
            if (_um.SelectedUnit != null)
            {
                // Move towards the selected tile
                StartCoroutine(_um.MoveUnit());
            }
            else
            {
                if (_bm.BuildingFromPosition.ContainsKey(HoveredOverTile) && _bm.BuildingFromPosition[HoveredOverTile].Owner == _gm.PlayerTurn )
                {
                     var CurrBuilingType= _bm.BuildingDataFromTile[_mm.Map.GetTile<Tile>(HoveredOverTile)].BuildingType;
                     bool Isspawner = CurrBuilingType == EBuildings.Port || CurrBuilingType == EBuildings.Camp;
               
                    if( Isspawner )
                    {
                        _gm.CurrentStateOfPlayer = EPlayerStates.InBuildingMenu;
                    }
                    else
                    {
                        _gm.CurrentStateOfPlayer = EPlayerStates.InSettingsMenu;
                    }
                }
                else
                {
                    _gm.CurrentStateOfPlayer = EPlayerStates.InSettingsMenu;
                }
            }
        }
    }
    private void MoveCamera(Vector3Int offset)
    {
        var bounds = _mm.Map.cellBounds;
        var xdistance = HoveredOverTile.x - _camera.transform.position.x;
        var ydistance = HoveredOverTile.y - _camera.transform.position.y;
        //if we hit a certain tile move the camera with it
        if ((xdistance>5 && offset.x>0)||( xdistance < -6 && offset.x < 0) ||( ydistance > 2 && offset.y > 0 )|| (ydistance< -3 && offset.y < 0))
        {
        //move the camera and make sure to not go out of bounds
        _camera.transform.position = new Vector3(math.clamp( _camera.transform.position.x + offset.x,bounds.xMin+9,bounds.xMax-9), math.clamp(_camera.transform.position.y + offset.y, bounds.yMin+5, bounds.yMax-5), _camera.transform.position.z);
        }
    }
    #endregion
}
