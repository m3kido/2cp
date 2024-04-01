using System;
using UnityEngine;

[Serializable]
public struct PlayerSaveData
{
    public int PlayerNumber;
    public ETeams Team;
    public Captain PlayerCaptain;
    public float CPCharge;
    public bool IsCPActivated;
    public int Gold;

    public PlayerSaveData(int playerNumber, ETeams team, Captain playerCaptain, float cPCharge,
        bool isCPActivated, int gold)
    {
        PlayerNumber = playerNumber;
        Team = team;
        PlayerCaptain = playerCaptain;
        CPCharge = cPCharge;
        IsCPActivated = isCPActivated;
        Gold = gold;
    }
}

[Serializable]
public struct AttackingUnitSaveData
{
    public int Health;
    public int Provisions;
    public int Owner;
    public EUnits UnitType;
    public bool HasMoved;
    public int EnergyOrbs;
    public Weapon CurrentWeapon;
    public int CurrentWeaponIndex;
    public bool HasAttacked;
    public Vector3Int Position;

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
