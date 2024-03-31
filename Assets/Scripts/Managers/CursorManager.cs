using UnityEngine;
using UnityEngine.Tilemaps;

// Class to manage the cursor
public class CursorManager : MonoBehaviour
{
    // Managers will be needed
    private UnitManager _um;
    private MapManager _mm;
    private BuildingManager _bm;
    private GameManager _gm;

    // This is a property that holds the tile which the cursor is hovering over
    public Vector3Int HoveredOverTile
    {
        get => _mm.Map.WorldToCell(transform.position);
        set => transform.position = value;
    }

    // Auto-property (the compiler automatically creates a private field for it)
    public Vector3Int SaveTile { get; set; }

    void Start()
    {
        // Get the unit, map, game and building managers from the hierarchy
        _um = FindAnyObjectByType<UnitManager>();
        _mm = FindAnyObjectByType<MapManager>();
        _gm = FindAnyObjectByType<GameManager>();
        _bm = FindAnyObjectByType<BuildingManager>();
    }

    void Update()
    {
        // Handle input every frame
        if(_gm.CurrentStateOfPlayer == EPlayerStates.Idle || _gm.CurrentStateOfPlayer == EPlayerStates.Selecting) {
            HandleInput();
        }
    }

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

        // Arrow keys
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelector(Vector3Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelector(Vector3Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelector(Vector3Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelector(Vector3Int.down);
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
                    // Add tile to path
                    int cost = _mm.GetTileData(_mm.Map.GetTile<Tile>(HoveredOverTile + offset)).ProvisionsCost;
                    if (_um.PathCost + cost > _um.SelectedUnit.Provisions) { return; }
                    _um.UndrawPath();
                    _um.Path.Add(HoveredOverTile + offset);
                    _um.PathCost += cost;
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
    }

    // Handle X Click
    private void XClicked()
    {
        if(_gm.CurrentStateOfPlayer == EPlayerStates.Selecting) 
        {
            // Cancel selection
            HoveredOverTile = _mm.Map.WorldToCell(_um.SelectedUnit.transform.position);
            _um.DeselectUnit();       
        }
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
                if (_um.SelectedUnit == refUnit) {  StartCoroutine(_um.MoveUnit());  }
                return;
            }

            // Can't select an enemy unit
            if (refUnit.Owner != _gm.PlayerTurn) { return; }

            // Can't select a unit that has already moved
            if (refUnit.HasMoved) { return; }
            SaveTile = HoveredOverTile;
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
                if (_bm.BuildingFromPosition.ContainsKey(HoveredOverTile) && _bm.BuildingFromPosition[HoveredOverTile].Owner==_gm.PlayerTurn)
                {
                    _bm.SpawnUnit(EUnits.Infantry, _bm.BuildingFromPosition[HoveredOverTile], _gm.PlayerTurn);
                }
                else
                {
                    _gm.CurrentStateOfPlayer = EPlayerStates.InSettingsMenu;
                }
            }
        }
    }
}
