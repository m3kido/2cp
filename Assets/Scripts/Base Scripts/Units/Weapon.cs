using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Weapon
{
    [SerializeField] EWeapons _weaponType;
    [SerializeField] List<int> _damageList;
    [SerializeField] int _energyOrbs;
    [SerializeField] int _minAttackRange;
    [SerializeField] int _maxAttackRange;

    public List<int> DamageList { get; set; }
    public int EnergyOrbs { get; set; }
    public EWeapons WeaponType { get; set; }
    public int MinRange { get; set; }
    public int MaxRange { get; set; }

    public static event Action OnEnergyRanOut; // event when a weapon has ran out of bullets ( need more bulleeets !!!)

    public void DecrementEnergyOrbs()
    {
        EnergyOrbs--;
        if (_energyOrbs < 0) { OnEnergyRanOut?.Invoke(); }
    }
}


