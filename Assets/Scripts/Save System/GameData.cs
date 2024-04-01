using System;
using UnityEngine;

[Serializable]
public class GameData
{

}

[Serializable]
public struct AttackingUnitSaveData
{
    public int Health;
    public int Fuel;
    public int Owner;
    public EUnits UnitType;
    public bool HasMoved;
    public Vector3Int Position;

    public AttackingUnitSaveData(int health, int fuel, int owner, EUnits unitType, bool hasMoved, Vector3Int position)
    {
        Health = health;
        Fuel = fuel;
        Owner = owner;
        UnitType = unitType;
        HasMoved = hasMoved;
        Position = position;
    }
}

[Serializable]
public struct LoadingUnitSaveData
{
    public int Health;
    public int Fuel;
    public int Owner;
    public EUnits UnitType;
    public bool HasMoved;
    public AttackingUnitSaveData LoadedUnitSaveData;
    public Vector3Int Position;

    public LoadingUnitSaveData(int health, int fuel, int owner, EUnits unitType, bool hasMoved, AttackingUnitSaveData loadedUnitSaveData, Vector3Int position)
    {
        Health = health;
        Fuel = fuel;
        Owner = owner;
        UnitType = unitType;
        HasMoved = hasMoved;
        LoadedUnitSaveData = loadedUnitSaveData;
        Position = position;
    }
}
