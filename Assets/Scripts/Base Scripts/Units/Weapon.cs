using System;
using System.Collections.Generic;
using UnityEngine;

// Class to represent a weapon
[Serializable]
public class Weapon
{
    [SerializeField] private List<int> _damageList;
    [SerializeField] private int _energyOrbs;
    [SerializeField] private EWeapons _weapon;
    [SerializeField] private int _minRange;
    [SerializeField] private int _maxRange;

    public List<int> DamageList { get => _damageList; set => _damageList = value; }
    public int EnergyOrbs { get => _energyOrbs; set => _energyOrbs = value; }
    public EWeapons WeaponType { get => _weapon; set => _weapon = value; }
    public int MinRange { get => _minRange; set => _minRange = value; }
    public int MaxRange { get => _maxRange; set => _maxRange = value; }

    
}


