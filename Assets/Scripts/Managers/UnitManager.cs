using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

// This script handles _unit interactions
// Keeps track of units and the path drawn by the cursor
public class UnitManager : MonoBehaviour
{
    #region Variables
    // Managers will be needed
    private GameManager _gm;
    private MapManager _mm;
    private Pathfinding Pathfinder;

    public GameObject[] UnitPrefabs; // We will need access to unit prefabs.
                                     // Check GameDataSaveManager : LoadUnits()
    public List<Unit> Units { get; set; }
    public Unit SelectedUnit { get; set; }
    public Vector3Int SaveTile { get; set; }
    public List<Vector3Int> Path  = new();
    public int PathCost { get; set; }

    public static event Action OnMoveEnd;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        // Get map and game managers from the hierarchy
        _mm = FindAnyObjectByType<MapManager>();
        _gm = FindAnyObjectByType<GameManager>();
      
        // Seek for units in the hierarchy
        Units = FindObjectsOfType<Unit>().ToList();
        Pathfinder = FindObjectOfType<Pathfinding>();
    }

    private void OnEnable()
    {
        // Subscribe to the day end event
        GameManager.OnTurnEnd += ResetUnits;
    }
    private void OnDisable()
    {
        // Unsubscribe from the day end event
        GameManager.OnTurnEnd -= ResetUnits;
    }
    #endregion

    #region Methods
    // Get _unit from given grid position
    public Unit FindUnit(Vector3Int pos)
    {
        foreach (Unit unit in Units)
        {
            if (unit.gameObject.activeInHierarchy == false) { continue; }
            if (_mm.Map.WorldToCell(unit.transform.position) == pos)
            {
                if (_gm.CurrentStateOfPlayer == EPlayerStates.InActionsMenu)
                {
                    if(unit != SelectedUnit) { return unit; }
                }
                else
                {
                    return unit;
                }
                
            }
        }
        return null;
    }
   
    // Check if the given position is an obstacle
    public bool IsObstacle(Vector3Int pos, Unit unit)
    {
        Unit tileUnit = FindUnit(pos);
        if (!unit.Data.IsWalkable(_mm.GetTileData(pos).TerrainType)) { return true; }
        if (tileUnit != null && _gm.Players[tileUnit.Owner].TeamSide != _gm.Players[unit.Owner].TeamSide) { return true; }
        return false;
    }

    // Draw the arrow path
    public void DrawPath()
    {

        for (int i = 0; i < Path.Count; i++)
        {
            if (i == 0)
            {
                if (i == 0)
                {
                    //start case because the start point is not in the path list
                    _mm.DrawArrow(_mm.Map.WorldToCell(SelectedUnit.transform.position), Path[0], Path[Mathf.Clamp(1, 0, Path.Count - 1)]);
                    continue;
                }
                //the clamp is for capping the i at its max (path.count -1)
                _mm.DrawArrow(Path[i - 1], Path[i], Path[Mathf.Clamp(i + 1, 0, Path.Count - 1)]);
            }
            //the clamp is for capping the i at its max (path.count -1)
            _mm.DrawArrow(Path[i - 1], Path[i], Path[Mathf.Clamp(i + 1, 0, Path.Count - 1)]);
        }
    }

    // Undraw the arrow path
    public void UndrawPath()
    {
        foreach (var pos in Path)
        {
            _mm.DrawArrow(pos, pos, pos);
        }
    }

    // select a given _unit
    public void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
        SelectedUnit.HighlightTiles();
        DrawPath();
        _gm.CurrentStateOfPlayer = EPlayerStates.Selecting;
    }

    // Deselect the selected _unit
    public void DeselectUnit()
    {
        SelectedUnit.ResetTiles();
        UndrawPath();
        SelectedUnit = null;
        Path.Clear();
        PathCost = 0;
        _gm.CurrentStateOfPlayer = EPlayerStates.Idle;
    }

    // Move the selected _unit
    public IEnumerator MoveUnit()
    {  
        SelectedUnit.IsMoving = true;
        SelectedUnit.ResetTiles();
        UndrawPath();
        Vector3Int lastoffset= Vector3Int.zero;
        foreach (var pos in Path)
        {
            var offset = pos - SelectedUnit.GetGridPosition();
           
            if (SelectedUnit.animator != null)
            {
               
              
                if (offset != lastoffset)
                {
                   
                    if (offset.x == 1)
                    {
                        SelectedUnit.animator.SetTrigger("right");
                    }
                    else if (offset.x == -1)
                    {
                         SelectedUnit.animator.SetTrigger("left");
                    }
                    else if (offset.y == 1)
                    {
                        SelectedUnit.animator.SetTrigger("up");
                    }
                    else if (offset.y == -1)
                    {
                        SelectedUnit.animator.SetTrigger("down");
                        
                    }
                    lastoffset= offset;
                }
               

            }
            SelectedUnit.transform.position = pos;

            yield return new WaitForSeconds(0.08f);

        }
        yield return 1f;
        SelectedUnit.IsMoving = false;
        if( SelectedUnit.animator != null )
        {
            SelectedUnit.animator.SetTrigger("idle");
        }
        
        _gm.CurrentStateOfPlayer = EPlayerStates.InActionsMenu;
    }
    public void CallPathfinding(Vector3Int end)
    {
        
        List<Vector3Int> paths = new List<Vector3Int>();
        paths = Pathfinder.FindPath(SelectedUnit,SelectedUnit.GetGridPosition(),end);
        if (paths.Count > 0)
        {
            UndrawPath();
            Path.Clear();
            PathCost = 0;
            foreach (var pos in paths)
            {
                Path.Add(pos);
                PathCost += _mm.GetTileData(pos).ProvisionsCost;
            }
            DrawPath();
        }

    }
    
    // Runs at the end of the day 
    private void ResetUnits()
    {
        foreach(var unit in Units) {
            unit.HasMoved = false;
        }
    }

    // Confirm the move had ended
    public void EndMove()
    {
        SelectedUnit.Provisions -= PathCost;
        Path.Clear();
        PathCost = 0;

        SelectedUnit.HasMoved = true;
        SelectedUnit = null;
        _gm.CurrentStateOfPlayer = EPlayerStates.Idle;
        checkUnits(_gm.Players[_gm.PlayerTurn]); 
        OnMoveEnd?.Invoke();
    }

    public bool checkUnits(Player currentPlayer)
    {

        if(currentPlayer == null) return false;
        else
        {

            HashSet<int> uniqueOwners = new HashSet<int>();

            foreach (var unit in Units)
            {
                uniqueOwners.Add(unit.Owner);
            }

            // Calculate the sum of unique owners' indices
            int totalIndex = 0;
            foreach (var ownerIndex in uniqueOwners)
            {
                totalIndex += ownerIndex;
            }
            int totalPlayers = 0;
            for (int i = 0; i < _gm.Players.Count; i++)
            {
                totalPlayers += i + 1;
            }
            int deadPlayerIndex = totalPlayers - totalIndex - 1; 
            if ( deadPlayerIndex >= 0 && deadPlayerIndex == _gm.PlayerTurn )
            {
                _gm.Players[_gm.PlayerTurn].Lost = true;
                return false; 
            } 

        }
            
            
            return true;
    }


}
#endregion