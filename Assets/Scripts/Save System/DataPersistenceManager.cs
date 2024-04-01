using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DataPersistenceManager : MonoBehaviour
{
    private GameData _gd;
    private List<IDataPersistence> _dataPersistenceObjects;
    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            // By design, there should be only on data persistence manager in the scene, at any given time
            Debug.LogError("Found more than one data persistence manager in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    // Method to initialize a new game
    public void NewGame()
    {
        _gd = new GameData();
    }

    // Method to load a game
    public void LoadGame()
    {
        if (_gd == null)
        {
            Debug.Log("No data was found, initializing data to defaults.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(_gd);
        }

        Debug.Log("Game Loaded.");
    }

    // Method to save a game
    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in _dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref _gd);
        }

        Debug.Log("Game Saved.");
    }

    // When the player leaves the game
    public void OnApplicationQuit()
    {
        SaveGame();
    }

    // Get all objects that are related to data saving
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
