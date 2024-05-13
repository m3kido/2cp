using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

// Class to manage Buildings
public class BuildingManager : MonoBehaviour
{
    #region Variables
    // Managers will be needed
    private MapManager _mm;
    private UnitManager _um;
    private GameManager _gm;
    private CaptainBar _cp;

    // List to store units that can be bought in the building (provided in the inspector)
    [FormerlySerializedAs("UnitPrefabs")][SerializeField] private List<Unit> _unitPrefabs;

    // Array containing building datas of all buildings (provided in the inspector)
    [SerializeField] private BuildingDataSO[] _buildingDatas;
    public GameObject HealSprite;

    // Dictionary mapping a tile (on which there's a building) to its building data
    private Dictionary<Tile, BuildingDataSO> _buildingDataFromTile = new();

    // Dictionary mapping a position (on which there's a building) to its building
    private Dictionary<Vector3Int, Building> _buildingFromPosition = new();

    private Dictionary<Vector3Int, Building> _capturableBuildings = new();

    // Readonly Properties for the previous fields
    public List<Unit> UnitPrefabs => _unitPrefabs;
    public BuildingDataSO[] BuildingDatas => _buildingDatas;
    public Dictionary<Tile, BuildingDataSO> BuildingDataFromTile => _buildingDataFromTile;
    public Dictionary<Vector3Int, Building> BuildingFromPosition => _buildingFromPosition;
    public Dictionary<Vector3Int, Building> CapturableBuildings => _capturableBuildings;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        // Fill the _buildingDataFromTile dictionary
        foreach (var buildingData in _buildingDatas)
        {
            // Put the Building tile as a key, and the building data as a value
            _buildingDataFromTile.Add(buildingData.BuildingTile, buildingData);
        }
    }

    public event Action MapScanComplete;

    private IEnumerator Start()
    {
        // Get the Map, Game and Unit Managers from the hierarchy
        _mm = FindAnyObjectByType<MapManager>();
        _gm = FindAnyObjectByType<GameManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _cp = FindAnyObjectByType<CaptainBar>();

        // Scan the map and put all the buldings in the Buildings dictionary
        yield return StartCoroutine(ScanMapForBuildings());

        MapScanComplete?.Invoke();
    }

    private void Update()
    {
        CheckCapturingUnitsPositions();
    }

    private void OnEnable()
    {
        // GetGoldFromBuildings subscribes to day end event
       GameManager.OnDayEnd += GetGoldFromBuildings;
       GameManager.OnDayEnd += HealUnits;
    }

    private void OnDisable()
    {
        // GetGoldFromBuildings unsubscribes from day end event
        GameManager.OnDayEnd -= GetGoldFromBuildings;
        GameManager.OnDayEnd -= HealUnits;
    }
    #endregion

    #region Methods
    // Scan the map and put all the buldings in the Buildings dictionary
    private IEnumerator ScanMapForBuildings()
    {
        yield return null;

        foreach (var pos in _mm.Map.cellBounds.allPositionsWithin)
        {
            TerrainDataSO posTile = _mm.GetTileData(pos);
            if (posTile != null && posTile.TerrainType == ETerrains.Building)
            {
                BuildingDataSO buildingData = _buildingDataFromTile[_mm.Map.GetTile<Tile>(pos)];
                foreach (var player in _gm.Players)
                {
                    if (player.Color == buildingData.Color)
                    {
                        _buildingFromPosition.Add(pos, new Building(buildingData.BuildingType, pos, _gm.Players.IndexOf(player)));
                        if (buildingData.BuildingType == EBuildings.Village || buildingData.BuildingType == EBuildings.Castle)
                        {
                            _capturableBuildings.Add(pos, new Building(buildingData.BuildingType, pos, _gm.Players.IndexOf(player)));
                        }
                    }
                }
            }
        }
    }

    // Change the building sprite based on owner
    public void ChangeBuildingOwner(Building building, int owner)
    {
        foreach (var SO in _buildingDatas)
        {
            if (SO.Color == _gm.Players[owner].Color && SO.BuildingType == building.BuildingType) {
                _mm.Map.SetTile(building.Position, SO.BuildingTile);
            }
        }
        building.Owner = owner;
        building.Health = 200;
    }

    // Get building data of given grid position
    public BuildingDataSO GetBuildingData(Vector3Int pos)
    {
        return _buildingDataFromTile[_mm.Map.GetTile<Tile>(pos)];
    }

    // Capture building
    public void CaptureBuilding(Vector3Int pos)
    {
        _capturableBuildings[pos].Health -= (int)(_um.SelectedUnit.Health * _gm.Players[_gm.PlayerTurn].PlayerCaptain.CaptureMultiplier);
        print(_capturableBuildings[pos].Health);
        if (_capturableBuildings[pos].Health <= 0)
        {
            if (BuildingFromPosition[pos].BuildingType == EBuildings.Castle)
            {
                _gm.Players[_capturableBuildings[pos].Owner].Lost = true;
            }

            ChangeBuildingOwner(_capturableBuildings[pos], _gm.PlayerTurn);
            _um.SelectedUnit.IsCapturing = false;
            _capturableBuildings[pos].CapturingUnit = null;
        }
    }

    // Check if a capturing unit has moved before finishing the capture. if so, reset the village
    public void CheckCapturingUnitsPositions()
    {
        foreach (var village in _capturableBuildings.Values)
        {
            if (village.CapturingUnit != null && village.CapturingUnit.GetGridPosition() != village.Position
                && village.CapturingUnit.HasMoved)
            {
                village.Health = 200;
                village.CapturingUnit.IsCapturing = false;
                village.CapturingUnit = null;
            }
        }
    }

    // Spawn a unit from a building
    public void SpawnUnit(EUnits unitType, Vector3Int pos, int owner)
    {
        Unit unitPrefab = null;
        foreach (var prefab in _unitPrefabs)
        {
            if (prefab.Data.UnitType == unitType)
            {
                unitPrefab = prefab;
            }
        }
        Unit newUnit = Instantiate(unitPrefab, pos, Quaternion.identity, _um.transform);
        newUnit.Owner = owner;
        newUnit.HasMoved = true;
        if (newUnit == null) { return; }
        _um.Units.Add(newUnit);
        Instantiate(_um.SpawnEffect, newUnit.transform);
    }

    // Gain gold every day
    private void GetGoldFromBuildings()
    {
        foreach (var village in _capturableBuildings.Values)
        {
            if (village.Owner < 4 && village.BuildingType == EBuildings.Village)
            {
                _gm.Players[village.Owner].Gold += 2000;
                _cp.UpdateGold();
                
            }
        }
    }

    private void HealUnits()
    {
        foreach (var building in BuildingFromPosition.Values)
        {
            var unit = _um.FindUnit(building.Position);
            if (unit && building.Owner == unit.Owner)
            {
                if (unit.Health < 100)
                {
                    print(unit.Health);
                    print(unit.transform.position);
                    Instantiate(HealSprite, unit.transform);
                }
                unit.Health += 20;

            }
        }
    }
    #endregion
}
