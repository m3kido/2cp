using System;
using System.Collections.Generic;
using UnityEngine;

// Class to represent a weapon
[Serializable]
public class Weapon
{
    [SerializeField] List<int> _damageList;
    [SerializeField] int _energyOrbs;
    [SerializeField] EWeapons _weaponType;
    [SerializeField] int _energyPerAttack;
    [SerializeField] int _minRange;
    [SerializeField] int _maxRange;

    public List<int> DamageList { get => _damageList; set => _damageList = value; }
    public int EnergyOrbs { get => _energyOrbs; set => _energyOrbs = value; }
    public EWeapons WeaponType { get => _weaponType; set => _weaponType = value; }
    public int MinRange { get => _minRange; set => _minRange = value; }
    public int MaxRange { get => _maxRange; set => _maxRange = value; }

    public static event Action OnEnergyRanOut; // event when a weapon has ran out of bullets ( need more bulleeets !!!)

    public void ConsumeEnergy()
    {
        EnergyOrbs -= _energyPerAttack;
        if (_energyOrbs < 0) { OnEnergyRanOut?.Invoke(); }
    }
}


