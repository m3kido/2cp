using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

// Class to manage Buildings
public class BuildingManager : MonoBehaviour
{
    // Managers will be needed
    private MapManager _mm;
    private UnitManager _um;
    private GameManager _gm;

    // List to store units that can be bought in the building (provided in the inspector)
    [FormerlySerializedAs("UnitPrefabs")] [SerializeField] private List<Unit> _unitPrefabs;

    // Array containing building datas of all buildings (provided in the inspector)
    [SerializeField] private BuildingDataSO[] _buildingDatas;

    // Dictionary mapping a tile (on which there's a building) to its building data
    private Dictionary<Tile, BuildingDataSO> _buildingDataFromTile;

    // Dictionary mapping a position (on which there's a building) to its building
    private Dictionary<Vector3Int, Building> _buildingFromPosition;

    // Readonly Properties for the previous fields
    public List<Unit> UnitPrefabs => _unitPrefabs;
    public BuildingDataSO[] BuildingDatas => _buildingDatas;
    public Dictionary<Tile, BuildingDataSO> BuildingDataFromTile => _buildingDataFromTile;
    public Dictionary<Vector3Int, Building> BuildingFromPosition => _buildingFromPosition;

    private void Awake()
    {
        // Fill the _buildingDataFromTile dictionary
        _buildingDataFromTile = new Dictionary<Tile, BuildingDataSO>();
        foreach (var buildingData in _buildingDatas)
        {
            // Put the Building tile as a key, and the building data as a value
            _buildingDataFromTile.Add(buildingData.BuildingTile, buildingData);
        }
    }

    void Start()
    {
        // Get the Map, Game and Unit Managers from the hierarchy
        _mm = FindAnyObjectByType<MapManager>();
        _gm = FindAnyObjectByType<GameManager>();
        _um = FindAnyObjectByType<UnitManager>();

        // Scan the map and put all the buldings in the Buildings dictionary
        ScanMapForBuildings();
    }

    private void OnEnable()
    {
        // GetGoldFromBuildings subscribes to day end event
        GameManager.OnDayEnd += GetGoldFromBuildings;
    }

    private void OnDisable()
    {
        // GetGoldFromBuildings unsubscribes from day end event
        GameManager.OnDayEnd -= GetGoldFromBuildings;
    }

    // Scan the map and put all the buldings in the Buildings dictionary
    private void ScanMapForBuildings()
    {
        _buildingFromPosition = new Dictionary<Vector3Int, Building>();
        foreach (var pos in _mm.Map.cellBounds.allPositionsWithin)
        {
            TerrainDataSO posTile = _mm.GetTileData(pos);
            if (posTile != null && posTile.TerrainType == ETerrains.Building)
            {
                BuildingDataSO currData = _buildingDataFromTile[_mm.Map.GetTile<Tile>(pos)];
                _buildingFromPosition.Add(pos, new Building(currData.BuildingType, pos, (int)currData.Color));
            }
        }
    }

    // Get building data of given grid position
    public BuildingDataSO GetBuildingData(Vector3Int pos)
    {
        return _buildingDataFromTile[_mm.Map.GetTile<Tile>(pos)];
    }

    // Capture building
    public void CaptureBuilding(Building building, Unit unit)
    {
        building.Health -= unit.Health;
        if (building.Health <= 0)
        {
            building.Owner = unit.Owner;
            building.Health = 20;
        }
    }

    // Spawn a unit from a building
    public void SpawnUnit(EUnits unitType, Building building, int owner)
    {
        Unit newUnit = Instantiate<Unit>(_unitPrefabs[(int)unitType], building.Position, Quaternion.identity);
        newUnit.Owner = owner;
        newUnit.HasMoved = true;
        if (newUnit == null) { print("d"); return; }
        _um.Units.Add(newUnit);
    }

    // Gain gold every day
    private void GetGoldFromBuildings()
    {
        foreach (var building in _buildingFromPosition.Values)
        {
            // MODIFICATION NEEDED : We have to check whether the building can provide gold or not (only villages can)
            if (building.Owner < 4)
            {
                _gm.Players[building.Owner].Gold += 1000;
            }
        }
    }
}
