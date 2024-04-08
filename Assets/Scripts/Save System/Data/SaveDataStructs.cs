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
    public string PlayerID; /*
    We need to store players' IDs so that when we load a game, we know "who" was playing.
    If we do not save IDs, then we can't know who was player one, was it ComputaKilla99 or SweetSneeze_0
    */
    public int PlayerNumber; /*
    This is the player's number in the game {1, 2, 3 ,4}
    This one's saved so we know which one's turn it is
    */
    public ETeamColors Color;
    public ETeams Team;
    public Captain PlayerCaptain;
    public float CPCharge; // Celestial Power charge
    public bool IsCPActivated;
    public int Gold;

    // Constructor
    public PlayerSaveData(string playerID, int playerNumber, ETeamColors color, ETeams team, Captain playerCaptain,
        float cPCharge, bool isCPActivated, int gold)
    {
        PlayerID = playerID;
        PlayerNumber = playerNumber;
        Color = color;
        Team = team;
        PlayerCaptain = playerCaptain;
        CPCharge = cPCharge;
        IsCPActivated = isCPActivated;
        Gold = gold;
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
    public Weapon CurrentWeapon;
    public int CurrentWeaponIndex;
    public bool HasAttacked;
    public Vector3Int Position;

    // Constructor
    public AttackingUnitSaveData(EUnits unitType, int health, int provisions, int owner, bool hasMoved,
        int energyOrbs, Weapon currentWeapon, int currentWeaponIndex, bool hasAttacked, Vector3Int position)
    {
        UnitType = unitType;
        Health = health;
        Provisions = provisions;
        Owner = owner;
        HasMoved = hasMoved;
        EnergyOrbs = energyOrbs;
        CurrentWeapon = currentWeapon;
        CurrentWeaponIndex = currentWeaponIndex;
        HasAttacked = hasAttacked;
        Position = position;
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
    public AttackingUnitSaveData LoadedUnitSaveData;
    public Vector3Int Position;

    // Constructor
    public LoadingUnitSaveData(EUnits unitType, int health, int provisions, int owner, bool hasMoved,
        AttackingUnitSaveData loadedUnitSaveData, Vector3Int position)
    {
        UnitType = unitType;
        Health = health;
        Provisions = provisions;
        Owner = owner;
        HasMoved = hasMoved;
        LoadedUnitSaveData = loadedUnitSaveData;
        Position = position;
    }
}

// Struct for building data to save
[Serializable]
public struct BuildingSaveData
{
    public EBuildings BuildingType;
    public Vector3Int Position;
    public int Health;
    public int Owner;

    // Constructor
    public BuildingSaveData(EBuildings buildingType, Vector3Int position, int health, int owner)
    {
        BuildingType = buildingType;
        Position = position;
        Health = health;
        Owner = owner;
    }
}

// Struct for map data to save
[Serializable]
public class MapSaveData
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
public class TileSaveData
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