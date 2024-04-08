using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSaveManager : MonoBehaviour
{
    public static MapSaveManager Instance { get; private set; }
    [SerializeField] private MapManager _mapManager;

    private Map _map; // Map data that will be saved
    private MapFileHandler _mapDataHandler; // Class to handle writing map data in the file

    [Header("Map Properties")]
    [SerializeField] private int _mapId;
    [SerializeField] private string _mapName;
    [SerializeField] private int _maxPlayers;

    [Header("Map Storage File Configuartion")]
    [SerializeField] private string _mapDataFileName;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one map save manager in the scene.");
        }
        Instance = this;
    }

    public void SaveMap()
    {
        _map = new Map();
        _mapDataHandler = new MapFileHandler(Application.persistentDataPath, _mapDataFileName);

        _map.MapProperties = new MapSaveData(_mapId, _mapName, _maxPlayers);
        _map.TileSaveDatas = ExtractTilesData();
        _mapDataHandler.Save(_map);
    }

    // Iterate through the tilemap and get all tile datas to save
    public List<TileSaveData> ExtractTilesData()
    {
        List<TileSaveData> tileDataList = new();
        _mapManager.Map.CompressBounds();
        BoundsInt bounds = _mapManager.Map.cellBounds;

        // Clear and refill the dictionary
        _mapManager.DataFromTile.Clear();
        _mapManager.FillDataFromTileDictionary();

        for (int y = bounds.min.y; y < bounds.max.y; y++)
        {
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                Vector3Int localPlace = new(x, y, 0);
                Tile tile = _mapManager.Map.GetTile<Tile>(localPlace);

                if (tile != null)
                {
                    TileSaveData data = new(_mapManager.GetTileData(tile).TerrainType, localPlace);
                    tileDataList.Add(data);
                }
            }
        }
        // Clear the dictionary again
        _mapManager.DataFromTile.Clear();
        return tileDataList;
    }
}
