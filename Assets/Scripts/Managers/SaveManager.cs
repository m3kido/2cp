using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// WATCH FRIEREN
public class SaveManager : MonoBehaviour
{
    [SerializeField] UnitManager Um;
    [SerializeField] MapManager Mm;
    [SerializeField] Infantry Infantry;
    //TODO: make a dictionary <unit type,prefabs>


    private void OnEnable()
    {
        GameManager.OnSave += SaveUnits;
        GameManager.OnLoad += LoadUnits;
    }

    private void OnDisable()
    {
        GameManager.OnSave -= SaveUnits;
        GameManager.OnLoad -= LoadUnits;
    }

    public void SaveUnits() // press S
    {
        var str = JsonUtility.ToJson(ExtractUnitManager(), true);
        print(str);
        var path = $"{Application.persistentDataPath}/save.json";
        File.WriteAllText(path, str);

        //------TEST------
        foreach (var unit in FindObjectsOfType<AttackingUnit>())
        {
            Destroy(unit.gameObject);
        }
    }

    public void LoadUnits() // press D (not L)
    {
        var path = $"{Application.persistentDataPath}/save.json";
        var str = File.ReadAllText(path);
        print(str);
        var unitManager = JsonUtility.FromJson<UnitManagerData>(str);

        //------TEST------
        foreach (var unitSave in unitManager.AttackingUnits)
        {
            var unit = Instantiate(Infantry, Mm.Map.CellToWorld(unitSave.Position),Quaternion.identity);
            unit.SetSaveData(unitSave);
        }
    }


    public UnitManagerData ExtractUnitManager()
    {
        List<AttackingUnitSaveData> attackingUnitSaves = new();
        foreach (var unit in FindObjectsOfType<AttackingUnit>())
        {
            attackingUnitSaves.Add(unit.GetSaveData());
        }

        List<LoadingUnitSaveData> loadingUnitSaves = new();
        foreach (var unit in FindObjectsOfType<LoadingUnit>())
        {
            loadingUnitSaves.Add(unit.GetSaveData());
        }

        return new UnitManagerData(attackingUnitSaves, loadingUnitSaves);

    }


}

[Serializable]
public struct AttackingUnitSaveData
{
    public int Health;
    public int Fuel;
    public int Owner;

    public EUnitType Type;

    public bool HasMoved;

    public Weapon CurrentWeapon;
    public int CurrentWeaponIndex;
    public bool HasAttacked;

    public Vector3Int Position;

    public AttackingUnitSaveData(int health, int fuel, int owner, EUnitType type, bool hasMoved, Weapon currentWeapon, int currentWeaponIndex, bool hasAttacked, Vector3Int position)
    {
        Health = health;
        Fuel = fuel;
        Owner = owner;
        Type = type;
        HasMoved = hasMoved;
        CurrentWeapon = currentWeapon;
        CurrentWeaponIndex = currentWeaponIndex;
        HasAttacked = hasAttacked;
        Position = position;
    }

}

[Serializable]
public struct LoadingUnitSaveData
{
    public int Health;
    public int Fuel;
    public int Owner;

    public EUnitType Type;

    public bool HasMoved;

    public AttackingUnitSaveData LoadedUnitSaveData; // obviously a loading unit cant load another loading unit

    // to be continued 

    public Vector3Int Position;

    public LoadingUnitSaveData(int health, int fuel, int owner, EUnitType type, bool hasMoved, AttackingUnitSaveData loadedUnitSaveData, Vector3Int position)
    {
        Health = health;
        Fuel = fuel;
        Owner = owner;
        Type = type;
        HasMoved = hasMoved;
        LoadedUnitSaveData = loadedUnitSaveData;
        Position = position;
    }
}
