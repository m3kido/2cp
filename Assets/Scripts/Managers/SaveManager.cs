using System.Collections.Generic;
using UnityEngine;

using System;

// WATCH FRIEREN
public class SaveManager : MonoBehaviour 
{
    [SerializeField] UnitManager Um;
    [SerializeField] MapManager Mm;
    [SerializeField] List<UnitSaveData> UmSaveData=new();

    private void Start()
    {
        GameManager.OnSave += SerializeUnitManager;
    }

    public void SerializeUnitManager()
    {
        var str = JsonUtility.ToJson(ExtractUnitManager(),true);
        print(str);
    }

    public UnitManagerData ExtractUnitManager()
    {
        foreach (var unit in Um.Units)
        {
            UmSaveData.Add(ExtractUnitData(unit));
        }

        return new UnitManagerData(UmSaveData);

    }

    public UnitSaveData ExtractUnitData(Unit unit)
    {
        var pos = Mm.Map.WorldToCell(unit.transform.position);
        return new UnitSaveData(unit.Health, unit.Fuel, unit.Owner, unit.Type, unit.HasMoved,pos);
    }
    

}

[Serializable]
public struct UnitSaveData
{
    public int Health;
    public int Fuel;
    public int Owner;

    public EUnitType Type;

    public bool HasMoved;
    public Vector3Int Position;

    public UnitSaveData(int health, int fuel, int owner, EUnitType type, bool hasMoved, Vector3Int position)
    {
        Health = health;
        Fuel = fuel;
        Owner = owner;
        Type = type;
        HasMoved = hasMoved;
        Position = position;
    }
}

[Serializable]
public struct UnitManagerData
{
    public List<UnitSaveData> UnitManagerDatas;

    public UnitManagerData(List<UnitSaveData> unitManagerDatas)
    {
        UnitManagerDatas = unitManagerDatas;
    }
}
