using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    [SerializeField] List<int> _damageList;
    [SerializeField] int _ammo;
    [SerializeField] EWeaponTypes _weaponType;
    [SerializeField] int _ammoPerAttack;
    [SerializeField] int _minRange;
    [SerializeField] int _maxRange;

    public List<int> DamageList { get => _damageList; set => _damageList = value; }
    public int Ammo { get => _ammo; set => _ammo = value; }
    public EWeaponTypes Type { get => _weaponType; set => _weaponType = value; }
    public int MinRange { get => _minRange; set => _minRange = value; }
    public int MaxRange { get => _maxRange; set => _maxRange = value; }

    public static event Action OnAmmoRanOut; // event when a weapon has ran out of bullets ( need more bulleeets !!!)

    public void DecrementAmmo()
    {
        Ammo -= _ammoPerAttack;
        if (_ammo < 0) { OnAmmoRanOut?.Invoke(); }
    }
}


