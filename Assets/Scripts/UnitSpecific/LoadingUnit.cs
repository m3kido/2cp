using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LoadingUnit : Unit
{
    
    public Unit LoadedUnit;
    
    public List<Vector3Int> DropTiles;

    public void LoadUnit(Unit unit)
    {
        LoadedUnit = unit;
        LoadedUnit.gameObject.SetActive(false);
    }
    public bool CanLoadUnit(Unit unit)
    {
        bool sameOwner = (unit.Owner == Owner);
        bool compatible = (Data as LoadingUnitDataSO).LoadableUnits.Contains(unit.Data.UnitType);
        if(LoadedUnit==null && sameOwner && compatible) { return true; }
        return false;
    }
    public void InisitateDropUnit()
    {
        _gm.CurrentStateOfPlayer = EPlayerStates.Droping;
        StartCoroutine(DropAtSpot());        
    }
    private void DropUnit(Vector3Int spot)
    {
        LoadedUnit.gameObject.SetActive(true);
        LoadedUnit.transform.position = spot;
        LoadedUnit = null;
    }
    private IEnumerator DropAtSpot()
    {
        yield return null;
        bool Unitdroped=false;
        int SpotIndex = 0;
        CursorManager _cm = FindAnyObjectByType<CursorManager>();
        Vector3Int saveCursor = _cm.HoveredOverTile;

        while (!Unitdroped)
        {

            if (DropTiles.Count == 1)
            {
                _cm.HoveredOverTile = DropTiles[SpotIndex];            
            }
            else
            {

                _cm.HoveredOverTile = DropTiles[SpotIndex];

                // Handle navigation keys
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {   
                    SpotIndex = (SpotIndex + 1) % DropTiles.Count;
                    _cm.HoveredOverTile = DropTiles[SpotIndex];
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    SpotIndex = (SpotIndex - 1 + DropTiles.Count) % DropTiles.Count;
                    _cm.HoveredOverTile = DropTiles[SpotIndex];
                }
            }

           
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                DropUnit(DropTiles[SpotIndex]);
                Unitdroped= true;
                _um.EndMove();
            }

            if (Input.GetKeyDown(KeyCode.X)) 
            {
                //return cursor and camera
                _cm.HoveredOverTile = saveCursor;
                _gm.CurrentStateOfPlayer = EPlayerStates.InActionsMenu;
                break;
            }

            yield return null;
        }
       
        
    }
    
    public bool GetDropTiles()
    {
        DropTiles.Clear();
        TileValid(GetGridPosition() + Vector3Int.right);
        TileValid(GetGridPosition() + Vector3Int.down);
        TileValid(GetGridPosition() + Vector3Int.left);
        TileValid(GetGridPosition() + Vector3Int.up);
        if(DropTiles.Count> 0)
        {
            return true;
        }
        return false;
    }
    private void TileValid(Vector3Int pos)
    {
        var tile = _mm.GetTileData(pos);
        if(tile == null) { return; }
        bool walkable = LoadedUnit.Data.WalkableTerrains.Contains(tile.TerrainType);
        print(walkable);
        bool clear = _um.FindUnit(pos) == null;
        print(clear);
        if(walkable && clear) { DropTiles.Add(pos); }
    }

}
