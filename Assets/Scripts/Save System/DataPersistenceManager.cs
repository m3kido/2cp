using System.Collections.Generic;
using UnityEngine;

// Equivalent name : SaveManager
public class DataPersistenceManager : MonoBehaviour
{
    private SaveData _saveData; // Data that will be saved
    private GameManager _gm;
    private MapManager _mm;
    private UnitManager _um;
    private FileDataHandler _dataHandler; // Class to handle writing in the file

    [Header("File storage configuartion.")]
    [SerializeField] private string _fileName;
    [SerializeField] private bool _useEncryption;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one data persistence manager in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        _saveData = new SaveData();
        _gm = FindAnyObjectByType<GameManager>();
        _mm = FindAnyObjectByType<MapManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
    }

    public void NewGame() // Method to initialize a new game
    {
        _saveData = new SaveData();
    }

    public void LoadGame() // Method to load a game
    {
        _saveData = _dataHandler.Load();

        if (_saveData == null)
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

        _dataHandler.Save(_saveData); // Write _saveData in the file

        Debug.Log("Game Saved.");
    }

    public void OnApplicationQuit() // When the player leaves the game
    {
        SaveGame();
    }

    // Put the game data in _saveData
    public void ExtractGameData()
    {
        _saveData.GameLogicSaveData = new GameSaveData(_gm.Day, _gm.PlayerTurn);

        Debug.Log("Extracted game data.");
    }

    // Load data from _saveData to game
    public void LoadGameData()
    {
        _gm.Day = _saveData.GameLogicSaveData.Day;
        _gm.PlayerTurn = _saveData.GameLogicSaveData.PlayerTurn;

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

        _saveData.PlayerSaveDatas = playerSaves;

        Debug.Log("Extracted player datas.");
    }

    // Load data from _saveData to players
    public void LoadPlayers()
    {
        foreach(var playerSave in _saveData.PlayerSaveDatas)
        {
            PlayerInGame player = new(playerSave.PlayerID, playerSave.PlayerNumber, playerSave.Color,
                playerSave.Team, playerSave.PlayerCaptain)
            {
                CPCharge = playerSave.CPCharge,
                IsCPActivated = playerSave.IsCPActivated,
                Gold = playerSave.Gold
            };
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

        _saveData.AttackingUnitSaveDatas = attackingUnitSaves;
        _saveData.LoadingUnitSaveDatas = loadingUnitSaves;

        Debug.Log("Extracted unit datas.");
    }

    // Load data from _saveData to units
    public void LoadUnits()
    {
        foreach (var attackingUnitSave in _saveData.AttackingUnitSaveDatas)
        {
            GameObject unitPrefab = GetPrefabFromUnitType(attackingUnitSave.UnitType);
            if (unitPrefab != null)
            {
                GameObject unitObject = Instantiate(unitPrefab, _mm.Map.CellToWorld(attackingUnitSave.Position),
                    Quaternion.identity);
                if (unitObject.TryGetComponent<AttackingUnit>(out var unitComponent))
                {
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

        foreach (var loadingUnitSave in _saveData.LoadingUnitSaveDatas)
        {
            GameObject unitPrefab = GetPrefabFromUnitType(loadingUnitSave.UnitType);
            if (unitPrefab != null)
            {
                GameObject unitObject = Instantiate(unitPrefab, _mm.Map.CellToWorld(loadingUnitSave.Position),
                    Quaternion.identity);
                if (unitObject.TryGetComponent<LoadingUnit>(out var unitComponent))
                {
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

}
