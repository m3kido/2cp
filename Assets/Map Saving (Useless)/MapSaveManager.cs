using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSaveManager : MonoBehaviour
{
    public static MapSaveManager Instance { get; private set; }
    [SerializeField] private MapManager _mapManager;

    private Map _map; // Map data that will be saved
    private MapFileHandler _mapDataHandler; // Class to handle writing map data in the file

    [SerializeField, ReadOnly]
    private int _nextAvailableID;

    [Header("Map Properties")]
    [SerializeField] private string _mapName;
    [SerializeField] private int _maxPlayers;

    public static List<int> MapIDs { get; set; } = new(); // List of map IDs

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
        CheckExistingFiles();
        _map = new Map();
        string mapsDirectory = Path.Combine(Application.persistentDataPath, "Maps");
        _mapDataHandler = new MapFileHandler(mapsDirectory, $"map{_nextAvailableID}.json");
        MapIDs.Add(_nextAvailableID);

        _map.MapProperties = new MapSaveData(_nextAvailableID, _mapName, _maxPlayers);
        _map.TileSaveDatas = ExtractTilesData();
        _mapDataHandler.Save(_map);
        _nextAvailableID = GetNextAvailableID();
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

    // Delete a map (the file where it was saved)
    public void DeleteMap(int mapID)
    {
        // Delete the map file
        string mapsDirectory = Path.Combine(Application.persistentDataPath, "Maps");
        string filePath = Path.Combine(mapsDirectory, $"map{mapID}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Map with ID {mapID} deleted.");
        }
        else
        {
            Debug.LogWarning($"Map file not found for ID {mapID}.");
        }

        // Reset the index to the deleted map's
        _nextAvailableID = mapID; 
    }

    private void CheckExistingFiles()
    {
        _nextAvailableID = 0;
        MapIDs.Clear();

        string mapsDirectory = Path.Combine(Application.persistentDataPath, "Maps");

        if (!Directory.Exists(mapsDirectory))
        {
            Debug.Log("Maps directory not found. Creating a new one.");
            Directory.CreateDirectory(mapsDirectory);
            return;
        }

        string[] mapFiles = Directory.GetFiles(mapsDirectory, "*.json");

        if (mapFiles.Length == 0)
        {
            Debug.Log("No map files found in the directory.");
            return;
        }

        foreach (string mapFile in mapFiles)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mapFile);

            if (fileNameWithoutExtension.Length > 3)
            {
                if (int.TryParse(fileNameWithoutExtension[3..], out int parsedID))
                {
                    MapIDs.Add(parsedID);
                }
                else
                {
                    Debug.LogError($"Invalid map file name: {mapFile}. Skipping.");
                }
            }
            else
            {
                Debug.LogWarning($"Unexpected map file name format: {mapFile}. Skipping.");
            }
        }
        _nextAvailableID = GetNextAvailableID();
    }

    // Find the next available ID
    private int GetNextAvailableID()
    {
        int nextID = 0;
        while (MapIDs.Contains(nextID)) { nextID++; }
        return nextID;
    }
}

// Don't pay attention to this
// This is just a technique to display something in the inspector without it being able to be modified
public class ReadOnlyAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Disable editing
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true; // Re-enable editing
    }
}