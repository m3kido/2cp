using System;
using UnityEngine;


// Struct for game data to save
[Serializable]
public struct GameSaveData
{
    public int Day;
    public int PlayerTurn;

    // Constructor
    public GameSaveData(int day, int playerTurn)
    {
        Day = day;
        PlayerTurn = playerTurn;
    }
}

// Struct for player data to save
[Serializable]
public struct PlayerSaveData
{
    public string ID; /*
    We need to store players' IDs so that when we load a game, we know "who" was playing.
    If we do not save IDs, then we can't know who was player one, was it ComputaKilla99 or SweetSneeze_0
    */
    public int PlayerNumber; /*
    This is the player's number in the game {1, 2, 3 ,4}
    This one's saved so we know which one's turn it is
    */
    public string Name;
    public ETeamColors Color;
    public ETeams Team;
    public CaptainSaveData CaptainData;
    public int Gold;
    public bool Lost;

    // Constructor
    public PlayerSaveData(string id, int playerNumber, string name, ETeamColors color, ETeams team,
        CaptainSaveData captainData, int gold, bool lost)
    {
        ID = id;
        PlayerNumber = playerNumber;
        Name = name;
        Color = color;
        Team = team;
        CaptainData = captainData;
        Gold = gold;
        Lost = lost;
    }
}

// Struct for captain data to save
[Serializable]
public struct CaptainSaveData
{
    // What we need to save from a captain
    public ECaptains CaptainName;
    public bool IsCelesteActive;
    public int SuperMeter;

    // Constuctor
    public CaptainSaveData(ECaptains captainName, bool isCelesteActive, int superMeter)
    {
        CaptainName = captainName;
        IsCelesteActive = isCelesteActive;
        SuperMeter = superMeter;
    }
}

// Struct for attacking unit data to save
[Serializable]
public struct AttackingUnitSaveData
{
    // These obviously need to be saved
    public EUnits UnitType;
    public int Health;
    public int Provisions;
    public int Owner;
    public bool HasMoved;
    public int EnergyOrbs;
    public int CurrentWeaponIndex;
    public Vector3Int Position;
    public bool IsCapturing;

    // Constructor
    public AttackingUnitSaveData(EUnits unitType, int health, int provisions, int owner, bool hasMoved,
        int energyOrbs, int currentWeaponIndex, Vector3Int position, bool isCapturing)
    {
        UnitType = unitType;
        Health = health;
        Provisions = provisions;
        Owner = owner;
        HasMoved = hasMoved;
        EnergyOrbs = energyOrbs;
        CurrentWeaponIndex = currentWeaponIndex;
        Position = position;
        IsCapturing = isCapturing;
    }
}

// Struct for loading unit data to save
[Serializable]
public struct LoadingUnitSaveData
{
    public EUnits UnitType;
    public int Health;
    public int Provisions;
    public int Owner;
    public bool HasMoved;
    public Vector3Int LoadedUnitPosition;
    public Vector3Int Position;

    // Constructor
    public LoadingUnitSaveData(EUnits unitType, int health, int provisions, int owner, bool hasMoved,
        Vector3Int loadedUnitPosition, Vector3Int position)
    {
        UnitType = unitType;
        Health = health;
        Provisions = provisions;
        Owner = owner;
        HasMoved = hasMoved;
        LoadedUnitPosition = loadedUnitPosition;
        Position = position;
    }
}

// Struct for building data to save
[Serializable]
public struct BuildingSaveData
{
    public Vector3Int Position;
    public int Health;
    public int Owner;

    // Constructor
    public BuildingSaveData(Vector3Int position, int health, int owner)
    {
        Position = position;
        Health = health;
        Owner = owner;
    }
}

// Struct for map data to save
[Serializable]
public struct MapSaveData
{
    public int MapID;
    public string MapName;
    public int MaxPlayers;

    // Constructor
    public MapSaveData(int mapID, string mapName, int maxPlayers)
    {
        MapID = mapID;
        MapName = mapName;
        MaxPlayers = maxPlayers;
    }
}

// Struct for tile data to save
[Serializable]
public struct TileSaveData
{
    public ETerrains TerrainType;
    public Vector3Int Position;

    // Constructor
    public TileSaveData(ETerrains terrainType, Vector3Int position)
    {
        TerrainType = terrainType;
        Position = position;
    }
}