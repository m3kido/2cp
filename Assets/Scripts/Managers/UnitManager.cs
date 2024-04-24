using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// This script handles unit interactions and
// keeps track of units and the path drawn by the cursor
public class UnitManager : MonoBehaviour
{
    private GameManager _gm;
    private MapManager _mm;

    public List<Unit> Units { get; set; } = new();

    public GameObject[] UnitPrefabs; // We will need access to unit prefabs.
                                     // Check GameDataSaveManager : LoadUnits()
    public Unit SelectedUnit { get; set; }
    public Vector3Int SaveTile { get; set; }
    public List<Vector3Int> Path { get; set; } = new();
    public int PathCost { get; set; }

    private void Start()
    {
        _mm = FindAnyObjectByType<MapManager>();
        _gm = FindAnyObjectByType<GameManager>();

        // Spawn unit
        Instantiate(UnitPrefabs[0], Vector3Int.zero, Quaternion.identity, transform);
        Instantiate(UnitPrefabs[0], Vector3Int.right, Quaternion.identity, transform);

        // Seek for units in the scene
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
            if (_mm.Map.WorldToCell(unit.transform.position) == pos)
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
        if (!unit.Data.IsWalkable(_mm.GetTileData(pos).TerrainType)) { return true; }
        if (tileUnit != null && _gm.InGamePlayers[tileUnit.Owner].TeamSide !=
            _gm.InGamePlayers[unit.Owner].TeamSide) { return true; }
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
                    _mm.DrawArrow(_mm.Map.WorldToCell(SelectedUnit.transform.position), Path[0],
                        Path[Mathf.Clamp(1, 0, Path.Count - 1)]);
                    continue;
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

    // select a given unit
    public void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
        SelectedUnit.HighlightTiles();
        DrawPath();
        _gm.CurrentStateOfPlayer = EPlayerStates.Selecting;
    }

    // Deselect the selected unit
    public void DeselectUnit()
    {
        SelectedUnit.ResetTiles();
        UndrawPath();
        SelectedUnit = null;
        Path.Clear();
        PathCost = 0;
        _gm.CurrentStateOfPlayer = EPlayerStates.Idle;
    }

    // Move the selected unit
    public IEnumerator MoveUnit()
    {  
        SelectedUnit.IsMoving = true;
        SelectedUnit.ResetTiles();
        UndrawPath();
        
        foreach (var pos in Path)
        {
            SelectedUnit.transform.position = pos;
            yield return new WaitForSeconds(0.08f) ;

        }
        yield return 1f;
        SelectedUnit.IsMoving = false;
        
        _gm.CurrentStateOfPlayer = EPlayerStates.InActionsMenu;
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
    }
}
