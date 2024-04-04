using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This script handles unit interactions and
// keeps track of units and the path drawn by the cursor
public class UnitManager : MonoBehaviour
{
    // Array of units
    public List<Unit> Units;

    public Unit SelectedUnit;
    public Vector3Int SaveTile;
    public List<Vector3Int> Path = new();
    public int PathCost = 0;

    GameManager Gm;
    MapManager Mm;
    
    void Awake()
    {
        // Get map and game managers from the hierarchy
        Mm = FindAnyObjectByType<MapManager>();
        Gm = FindAnyObjectByType<GameManager>();

        // Seek for units in the hierarchy
        Units = FindObjectsOfType<Unit>().ToList();
        
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

    // Get unit from given grid position
    public Unit FindUnit(Vector3Int pos)
    {
        foreach (Unit unit in Units)
        {
            if (Mm.Map.WorldToCell(unit.transform.position) == pos)
            {
                return unit;
            }
        }
        return null;
    }

    // Check if the given position is an obstacle
    public bool IsObstacle(Vector3Int pos, Unit unit)
    {
        Unit tileUnit = FindUnit(pos);
        if (!unit.Data.IsWalkable(Mm.GetTileData(pos).TileType)) { return true; }
        if (tileUnit != null && Gm.Players[tileUnit.Owner].TeamSide != Gm.Players[unit.Owner].TeamSide) { return true; }
        return false;
    }

    // Draw the arrow path
    public void DrawPath()
    {
       
            for (int i = 0; i < Path.Count; i++)
            {
                if (i == 0)
                {
                    //start case because the start point is not in the path list
                    Mm.DrawArrow(Mm.Map.WorldToCell(SelectedUnit.transform.position), Path[0], Path[Mathf.Clamp(1, 0, Path.Count - 1)]);
                    continue;
                }
                //the clamp is for capping the i at its max (path.count -1)
                Mm.DrawArrow(Path[i - 1], Path[i], Path[Mathf.Clamp(i + 1, 0, Path.Count - 1)]);
            }
    }

    // Undraw the arrow path
    public void UnDrawPath()
    {
        foreach (var pos in Path)
        {
            Mm.DrawArrow(pos, pos, pos);
        }
    }

    // select a given unit
    public void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
        SelectedUnit.HighlightTiles();
        DrawPath();
        Gm.GameState = EGameStates.Selecting;
    }

    // Deselect the selected unit
    public void DeselectUnit()
    {
        SelectedUnit.ResetTiles();
        UnDrawPath();
        SelectedUnit = null;
        Path.Clear();
        PathCost = 0;
        Gm.GameState = EGameStates.Idle;
    }

    // Move the selected unit
    public IEnumerator MoveUnit()
    {
        
        SelectedUnit.IsMoving = true;
        SelectedUnit.ResetTiles();
        UnDrawPath();
        
        foreach (var pos in Path)
        {
            SelectedUnit.transform.position = pos;
            yield return new WaitForSeconds(0.08f) ;

        }
        yield return 1f;
        SelectedUnit.IsMoving = false;
        
        Gm.GameState = EGameStates.ActionMenu;
      
    }
    
    // Runs at the end of the day 
    private void ResetUnits()
    {
        foreach(var unit in Units) {
            unit.HasMoved=false;
        }
    }
    public void EndMove()
    {
        SelectedUnit.Fuel -= PathCost;
        Path.Clear();
        PathCost = 0;
        SelectedUnit.HasMoved = true;
        SelectedUnit = null;
    }


}
