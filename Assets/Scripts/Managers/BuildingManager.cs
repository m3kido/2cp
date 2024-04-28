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
    #endregion

    #region UnityMethods
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
    #endregion

    #region Methods
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
                foreach(var player in _gm.Players){
                    if(player.Color==currData.Color)
                    {
                        _buildingFromPosition.Add(pos, new Building(currData.BuildingType, pos, _gm.Players.IndexOf(player)));
                    }
                }
               
            }
        }
    }
    //change the sprite
    private void ChangeBuildingOwner(Building building,int owner)
    {
       ;
        foreach(var SO in _buildingDatas)
        {
            if(SO.Color == _gm.Players[_gm.PlayerTurn].Color && SO.BuildingType==building.BuildingType) {
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
        
        BuildingFromPosition[pos].Health -= _um.SelectedUnit.Health;
        print(BuildingFromPosition[pos].Health);
        if (BuildingFromPosition[pos].Health <= 0)
        {
            ChangeBuildingOwner(BuildingFromPosition[pos], _gm.PlayerTurn);
        }
    }
    
    // Spawn a unit from a building
    public void SpawnUnit(EUnits unitType, Vector3Int pos, int owner)
    {
        Unit unitPrefab = null ;
        foreach(var prefab in _unitPrefabs)
        {
            if(prefab.Data.UnitType== unitType)
            {
                unitPrefab = prefab;
            }
        }
        Unit newUnit = Instantiate<Unit>(unitPrefab, pos, Quaternion.identity,_um.transform);
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
            
            if (building.Owner < 4 && building.BuildingType==EBuildings.Village)
            {
                _gm.Players[building.Owner].Gold += 1000;
            }
        }
    }
    #endregion
}
