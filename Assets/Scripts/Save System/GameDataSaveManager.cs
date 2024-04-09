using System.Collections.Generic;
using UnityEngine;
public class GameDataSaveManager : MonoBehaviour
{
    private GameManager _gm;
    private MapManager _mm;
    private UnitManager _um;

    private GameData _gameData; // Data that will be saved
    private GameDataFileHandler _gameDataHandler; // Class to handle writing game data in the file

    [Header("Data Storage File Configuartion")]
    [SerializeField] private string _dataFileName;
    [SerializeField] private bool _useEncryption;

    public static GameDataSaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one game data save manager in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {   
        _gm = FindAnyObjectByType<GameManager>();
        _mm = FindAnyObjectByType<MapManager>();
        _um = FindAnyObjectByType<UnitManager>();

        _gameData = new GameData();
        _gameDataHandler = new GameDataFileHandler(Application.persistentDataPath, _dataFileName, _useEncryption);

        LoadGame();
    }


    public void NewGame() // Method to initialize a new game
    {
        _gameData = new GameData();

        Debug.Log("Initialized new game.");
    }

    public void LoadGame() // Method to load a game
    {
        _gameData = _gameDataHandler.Load();

        if (_gameData == null)
        {
            Debug.Log("No data was found, initializing data to defaults.");
            NewGame();
        } else
        {
            // Load everything
            LoadGameData();
            LoadPlayers();
            LoadUnits();
        }

        Debug.Log("Game Loaded.");
    }

    public void SaveGame() // Method to save a game
    {
        // Put all data of the game in _saveData
        ExtractGameData();
        ExtractPlayersData();
        ExtractUnitsData();

        _gameDataHandler.Save(_gameData); // Write _gameData in the file

        Debug.Log("Game Saved.");
    }

    public void OnApplicationQuit() // When the player leaves the game
    {
        SaveGame();
        // DestroyAllUnits();
    }

    // Put the game data in _saveData
    public void ExtractGameData()
    {
        _gameData.GameLogicSaveData = new GameSaveData(_gm.Day, _gm.PlayerTurn);

        Debug.Log("Extracted game data.");
    }

    // Load data from _saveData to game
    public void LoadGameData()
    {
        _gm.Day = _gameData.GameLogicSaveData.Day;
        _gm.PlayerTurn = _gameData.GameLogicSaveData.PlayerTurn;

        Debug.Log("Game data loaded.");
    }

    // Put the players data in _saveData
    public void ExtractPlayersData()
    {
        List<PlayerSaveData> playerSaves = new();
        foreach (PlayerInGame player in _gm.InGamePlayers)
        {
            playerSaves.Add(player.GetDataToSave());
        }

        _gameData.PlayerSaveDatas = playerSaves;

        Debug.Log("Extracted player datas.");
    }

    // Load data from _saveData to players
    public void LoadPlayers()
    {
        // Get saved players
        foreach(var playerSave in _gameData.PlayerSaveDatas)
        {
            // Create a new player object and assign the saved data to it
            PlayerInGame player = new(playerSave.PlayerID, playerSave.PlayerNumber, playerSave.Color,
                playerSave.Team, playerSave.PlayerCaptain)
            {
                CPCharge = playerSave.CPCharge,
                IsCPActivated = playerSave.IsCPActivated,
                Gold = playerSave.Gold
            };
            // Add it to the game
            _gm.InGamePlayers.Add(player);
        }
        Debug.Log("Players loaded.");
    }

    // Put the units data in _saveData
    public void ExtractUnitsData()
    {
        List<AttackingUnitSaveData> attackingUnitSaves = new();
        foreach (var unit in FindObjectsOfType<AttackingUnit>())
        {
            attackingUnitSaves.Add(unit.GetDataToSave());
        }

        List<LoadingUnitSaveData> loadingUnitSaves = new();
        foreach (var unit in FindObjectsOfType<LoadingUnit>())
        {
            loadingUnitSaves.Add(unit.GetDataToSave());
        }

        _gameData.AttackingUnitSaveDatas = attackingUnitSaves;
        _gameData.LoadingUnitSaveDatas = loadingUnitSaves;

        Debug.Log("Extracted unit datas.");
    }

    // Load data from _saveData to units
    public void LoadUnits()
    {
        // Get attacking unit saves
        foreach (var attackingUnitSave in _gameData.AttackingUnitSaveDatas)
        {
            // Create prefab based of each saved unit type
            GameObject unitPrefab = GetPrefabFromUnitType(attackingUnitSave.UnitType);
            if (unitPrefab != null)
            {
                // Instantiate it in the right saved position
                GameObject unitObject = Instantiate(unitPrefab, _mm.Map.CellToWorld(attackingUnitSave.Position),
                    Quaternion.identity);
                if (unitObject.TryGetComponent<AttackingUnit>(out var unitComponent))
                {
                    // Assign saved data to that prefab
                    unitComponent.SetSavedData(attackingUnitSave);
                }
                else
                {
                    Debug.LogError("AttackingUnit component not found on instantiated object.");
                }
            }
            else
            {
                Debug.LogError($"Prefab for unit type {attackingUnitSave.UnitType} not found.");
            }
        }

        // Get loading unit saves
        foreach (var loadingUnitSave in _gameData.LoadingUnitSaveDatas)
        {
            // Create prefab based of each saved unit type
            GameObject unitPrefab = GetPrefabFromUnitType(loadingUnitSave.UnitType);
            if (unitPrefab != null)
            {
                // Instantiate it in the right saved position
                GameObject unitObject = Instantiate(unitPrefab, _mm.Map.CellToWorld(loadingUnitSave.Position),
                    Quaternion.identity);
                if (unitObject.TryGetComponent<LoadingUnit>(out var unitComponent))
                {
                    // Assign saved data to that prefab
                    unitComponent.SetSavedData(loadingUnitSave);
                }
                else
                {
                    Debug.LogError("LoadingUnit component not found on instantiated object.");
                }
            }
            else
            {
                Debug.LogError($"Prefab for unit type {loadingUnitSave.UnitType} not found.");
            }
        }
        Debug.Log("Units loaded.");
    }

    // Gets unit prefab from unit type (used to create unit object based on the saved data)
    public GameObject GetPrefabFromUnitType(EUnits unitType)
    {
        // The prefabs passed in the inspector should be given in the right order
        if (_um.UnitPrefabs[(int)unitType] != null)
        {
            return _um.UnitPrefabs[(int)unitType];
        }
        else
        {
            Debug.LogError("Unit type not found in dictionary.");
            return null;
        }
    }

    public void DestroyAllUnits()
    {
        foreach(AttackingUnit unit in FindObjectsOfType<AttackingUnit>()) {
            Destroy(unit);
        }
        Debug.Log("Destroyed all units.");
    }
}