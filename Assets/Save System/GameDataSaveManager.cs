using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Save system
public class GameDataSaveManager : MonoBehaviour
{
    #region Variables
    private GameManager _gm;
    private MapManager _mm;
    private UnitManager _um;
    private BuildingManager _bm;
    private CaptainBar _capBar;

    [SerializeField] private UnitDispositionSO _unitDisposition;

    private GameData _gameData; // Data that will be saved
    private GameDataFileHandler _gameDataHandler; // Class to handle writing game data in the file

    [Header("Data Storage File Configuartion")]
    [SerializeField] public string _dataFileName;
    [SerializeField] private bool _useEncryption;

    public static GameDataSaveManager Instance { get; private set; }
    private bool _mapScanComplete = false;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one game data save manager in the scene.");
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        _gm = FindAnyObjectByType<GameManager>();
        _mm = FindAnyObjectByType<MapManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _bm = FindAnyObjectByType<BuildingManager>();
        _capBar = FindAnyObjectByType<CaptainBar>();

        _gameData = new GameData();
        _gameDataHandler = new GameDataFileHandler(Application.persistentDataPath, _dataFileName, _useEncryption);

        // Subscribe to the MapScanComplete event
        _bm.MapScanComplete += OnMapScanComplete;

        // Wait until the map scanning is complete
        while (!_mapScanComplete)
        {
            yield return null;
        }

        LoadGame();
    }

    public void OnApplicationQuit() // When the player leaves the game
    {
        // Ask if player wants to save first
        // SaveGame();

        // DestroyAllUnits();
    }
    #endregion

    #region Methods
    // Method called when map scanning is complete
    private void OnMapScanComplete()
    {
        _mapScanComplete = true;

        // Unsubscribe from the event
        _bm.MapScanComplete -= OnMapScanComplete;
    }

    public void NewGame() // Method to initialize a new game
    {
        _gameData = new GameData();
        _um.PlaceUnits(_unitDisposition);
        _um.Units = FindObjectsOfType<Unit>().ToList();

        Debug.Log("Initialized new game.");
    }

    public void LoadGame() // Method to load a game
    {
        _gameData = _gameDataHandler.Load();

        if (_gameData == null)
        {
            Debug.Log("No saved data was found.");
            NewGame();
        }
        else
        {
            // Load everything
            LoadGameData();
            LoadPlayers();
            LoadUnits();
            LoadBuildings();

            Debug.Log("Game Loaded.");
        }
    }

    public void SaveGame() // Method to save a game
    {
        // Put all data of the game in _gameData
        ExtractGameData();
        ExtractPlayersData();
        ExtractUnitsData();
        ExtractBuildingsData();

        _gameDataHandler.Save(_gameData); // Write _gameData in the file

        Debug.Log("Game Saved.");
    }

    // Put the game data in _gameData
    public void ExtractGameData()
    {
        _gameData.GameLogicSave = new GameSaveData(_gm.Day, _gm.PlayerTurn);
    }

    // Load data from _gameData to game
    public void LoadGameData()
    {
        _gm.Day = _gameData.GameLogicSave.Day;
        _gm.PlayerTurn = _gameData.GameLogicSave.PlayerTurn;
        _capBar.UpdateCaptain();
    }

    // Put the players data in _gameData
    public void ExtractPlayersData()
    {
        List<PlayerSaveData> playerSaves = new();
        foreach (Player player in _gm.Players)
        {
            playerSaves.Add(player.GetDataToSave());
        }

        _gameData.PlayerSaves = playerSaves;
    }

    // Load data from _gameData to players
    public void LoadPlayers()
    {
        _gm.Players.Clear();

        // Get saved players
        foreach (var playerSave in _gameData.PlayerSaves)
        {
            // Create a new player object and assign the saved data to it
            Player player = new(playerSave.ID, playerSave.PlayerNumber, playerSave.Name, playerSave.Color, playerSave.Team,
                playerSave.CaptainData.CaptainName, playerSave.Gold, playerSave.Lost);

            player.PlayerCaptain.SetSaveData(playerSave.CaptainData);

            // Add it to the game
            _gm.Players.Add(player);
        }
    }

    // Put the units data in _gameData
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
            if (unit.LoadedUnit != null)
            {
                attackingUnitSaves.Add(((AttackingUnit)unit.LoadedUnit).GetDataToSave());
            }
        }

        _gameData.AttackingUnitSaves = attackingUnitSaves;
        _gameData.LoadingUnitSaves = loadingUnitSaves;
    }

    // Load data from _gameData to units
    public void LoadUnits()
    {
        // Get attacking unit saves
        foreach (var attackingUnitSave in _gameData.AttackingUnitSaves)
        {
            // Create prefab based of each saved unit type
            GameObject unitPrefab = GetPrefabFromUnitType(attackingUnitSave.UnitType);
            if (unitPrefab != null)
            {
                // Instantiate it in the right saved position
                GameObject unitObject = Instantiate(unitPrefab, _mm.Map.CellToWorld(attackingUnitSave.Position),
                    Quaternion.identity, _um.transform);
                if (unitObject.TryGetComponent<AttackingUnit>(out var unitComponent))
                {
                    // Assign saved data to that prefab
                    unitComponent.SetSavedData(attackingUnitSave);

                    // Add the unit to the units list
                    _um.Units.Add(unitComponent);
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
        foreach (var loadingUnitSave in _gameData.LoadingUnitSaves)
        {
            // Create prefab based of each saved unit type
            GameObject unitPrefab = GetPrefabFromUnitType(loadingUnitSave.UnitType);
            if (unitPrefab != null)
            {
                // Instantiate it in the right saved position
                GameObject unitObject = Instantiate(unitPrefab, _mm.Map.CellToWorld(loadingUnitSave.Position),
                    Quaternion.identity, _um.transform);
                if (unitObject.TryGetComponent<LoadingUnit>(out var unitComponent))
                {
                    // Assign saved data to that prefab
                    unitComponent.SetSavedData(loadingUnitSave);

                    // Add the unit to the units list
                    _um.Units.Add(unitComponent);
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
        AttackingUnit[] attackingUnits = FindObjectsOfType<AttackingUnit>();
        LoadingUnit[] loadingUnits = FindObjectsOfType<LoadingUnit>();

        if (attackingUnits.Length == 0 && loadingUnits.Length == 0)
        {
            Debug.Log("No units found to destroy.");
            return;
        }

        if (attackingUnits.Length != 0)
        {
            foreach (AttackingUnit unit in attackingUnits)
            {
                Destroy(unit.gameObject);
            }
        }

        if (loadingUnits.Length != 0)
        {
            foreach (LoadingUnit unit in loadingUnits)
            {
                Destroy(unit.gameObject);
            }
        }
    }

    // Put the buildings data in _gameData
    public void ExtractBuildingsData()
    {
        List<BuildingSaveData> buildingSaves = new();
        foreach (var pos in _bm.CapturableBuildings.Keys)
        {
            buildingSaves.Add(_bm.CapturableBuildings[pos].GetDataToSave());
        }
    
        _gameData.BuildingSaves = buildingSaves;
    }

    // Load data from _gameData to buildings
    public void LoadBuildings()
    {
        foreach (var buildingSave in _gameData.BuildingSaves)
        {
            if (_bm.CapturableBuildings.TryGetValue(buildingSave.Position, out Building building))
            {
                _bm.ChangeBuildingOwner(building, buildingSave.Owner);
                building.SetSaveData(buildingSave);

                Unit potentialCapturingUnit = _um.FindUnit(buildingSave.Position);
                if (potentialCapturingUnit != null && potentialCapturingUnit.IsCapturing)
                {
                    building.CapturingUnit = potentialCapturingUnit;
                }
            }
        }
    }
    #endregion
}