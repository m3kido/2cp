using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class BuildingManager : MonoBehaviour
{
    MapManager Mm;
    UnitManager Um;
    GameManager Gm;


    [SerializeField] private List<Unit> _unitPrefabs;
    private Dictionary<Tile, BuildingData> _buildingTileData;
    [SerializeField] private BuildingData[] _buildingDatas;


    public Dictionary<Vector3Int, Building> Buildings;

    // Get Building datas of every building type from the inspector
    private void Awake()
    {
        _buildingTileData = new Dictionary<Tile, BuildingData>();

        foreach (var buildingData in _buildingDatas)
        {
            _buildingTileData.Add(buildingData.Building, buildingData);
        }
    }

    void Start()
    {
        // Get the Map and Game Managers from the hierarchy
        Mm = FindAnyObjectByType<MapManager>();
        Gm = FindAnyObjectByType<GameManager>();
        Um = FindAnyObjectByType<UnitManager>();

        // Scan the map and put all the buldings in the _buildings dictionary
        ScanMapForBuildings();

    }
    private void OnEnable()
    {
        GameManager.OnDayEnd += AddGold;
    }
    private void OnDisable()
    {
        GameManager.OnDayEnd -= AddGold;
    }

    // Scan the map and put all the buldings in the _buildings dictionary
    private void ScanMapForBuildings()
    {
        Buildings = new Dictionary<Vector3Int, Building>();
        foreach (var pos in Mm.Map.cellBounds.allPositionsWithin)
        {
            TileData posTile = Mm.GetTileData(pos);


            if (posTile != null && posTile.TileType == ETileTypes.Building)
            {
                BuildingData currData = _buildingTileData[Mm.Map.GetTile<Tile>(pos)];
                Buildings.Add(pos, new Building(pos, currData.BuildingType, (int)currData.Color));

            }
        }
    }

    // Get building data of given grid position
    public BuildingData GetBuildingData(Vector3Int pos)
    {
        return _buildingTileData[Mm.Map.GetTile<Tile>(pos)];
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

    public void SpawnUnit(EUnitType UnitType, Building building, int Owner)
    {
        Unit newUnit = Instantiate(_unitPrefabs[(int)UnitType], building.Position, Quaternion.identity);
        newUnit.Owner = Owner;
        newUnit.HasMoved = true;
        //outline this mf
        if (newUnit == null) { print("d");return; }
        Um.Units.Add(newUnit);

    }
    private void AddGold()
    {
        foreach (var building in Buildings.Values)
        {
            if(building.Owner < 4)
            {
                Gm.Players[building.Owner].Gold += 1000;
            }
           
        }

    }

}
