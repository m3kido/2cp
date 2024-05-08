using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class AttackingUnit : Unit
{
    [SerializeField] List<Weapon> _weapons;
    public List<Weapon> Weapons { get => _weapons; set => _weapons = value; }

    public int CurrentWeaponIndex = 0;
    private bool _isAttacking;
    public bool HasAttacked;

    public bool IndirectUnit;

    public bool IsAttacking
    {
        get
        {
            return _isAttacking;
        }
        set
        {
            _isAttacking = value;
            if (_isAttacking)
            {
                _rend.color = Color.red;
            }
            else
            {
                _rend.color = Color.white;
            }
        }
    }

    public int CurrentWeaponAmmo
    {
        get { return _weapons[CurrentWeaponIndex].EnergyOrbs; }
        set
        {
            if (value <= 0 && CurrentWeaponIndex < _weapons.Count-1)
            {
                CurrentWeaponIndex++;
            }
            else
            {
                _weapons[CurrentWeaponIndex].EnergyOrbs = value;
            }
        }
    }

    public void ConsumeAmmo()
    {
        CurrentWeaponAmmo-=Weapons[CurrentWeaponIndex].EnergyConsumption;
    }

    // scans area for targets in an Intervall [min range, max range]
    // Assumed that every unit can be in one tile which can be in one grid position
    public List<Unit> ScanTargets()
    {
        var attackerPos = GetGridPosition();
        List<Unit> targets = new();

        foreach (var unit in _um.Units)
        {
            if (unit == this) continue;
            var potentialTargetPos = _mm.Map.WorldToCell(unit.transform.position);
            var currentWeapon = Weapons[CurrentWeaponIndex];
            Player player = _gm.Players[Owner];
            Captain captain = player.PlayerCaptain;
            bool IsInRange = (L1Distance2D(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance2D(attackerPos, potentialTargetPos) < (currentWeapon.MaxRange + captain.AttackRangeAdditioner));
            bool IsEnemy = Owner != unit.Owner;
            bool IsDamageable = Weapons[CurrentWeaponIndex].DamageList[(int)unit.Data.UnitType] != 0;
            if (IsInRange && IsEnemy && IsDamageable)
            {

                targets.Add(unit);
            }
        }
        return targets;
    }

    public List<Unit> ScanTargets(int moveRange)
    {
        var attackerPos = GetGridPosition();
        List<Unit> targets = new();

        foreach (var unit in _um.Units)
        {
            if (unit == this) continue;

            var potentialTargetPos = _mm.Map.WorldToCell(unit.transform.position);

            var currentWeapon = Weapons[CurrentWeaponIndex]; // Getting the current weapon from the attacker
            Player player = _gm.Players[Owner];
            Captain captain = player.PlayerCaptain;
            bool IsInRange = (L1Distance2D(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance2D(attackerPos, potentialTargetPos) < (currentWeapon.MaxRange + captain.AttackRangeAdditioner + moveRange));
            bool IsEnemy = Owner != unit.Owner;
            bool IsDamageable = Weapons[CurrentWeaponIndex].DamageList[(int)unit.Data.UnitType] != 0;

            if (IsInRange && IsEnemy && IsDamageable)
            {
                targets.Add(unit);
            }
        }

        return targets;
    }

    public void UnHighlightTargets()
    {
        List<Unit> targets = ScanTargets();
        foreach (var target in targets)
        {
            target._rend.color = Color.white;
        }
    }

    public void UnHighlightTarget(Unit target)
    {
        target._rend.color = Color.white;
    }

    public bool CheckAttack()
    {
        List<Unit> targets = ScanTargets();

        if (targets == null)
        {
            Debug.LogWarning("Targets list is null. Unable to check if attacker can attack.");
            return false;
        }

        // Now, check if the attacker has any valid targets to attack
        if (targets.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanAttackThis(Unit target)
    {
        List<Unit> targets = ScanTargets();
        if (!targets.Contains(target))
        { return false; }
        return true;
    }

    public void MoveToNextWeapon()
    {
        if (CurrentWeaponIndex < Weapons.Count - 1) CurrentWeaponIndex++; // Cannot exceed last weapon index
    }

    public void ResetWeapons()
    {
        CurrentWeaponIndex = 0;
    }

    public void AttackTiles()
    {
        SeekTile(GetGridPosition(), -1, 0);
        List<Vector3Int> extraTiles = new();
        foreach (var pos in ValidTiles.Keys)
        {
            ExpandFromTiles(extraTiles, pos, Weapons[CurrentWeaponIndex].MaxRange);
        }
        foreach (var pos in extraTiles)
        {
            if (!ValidTiles.ContainsKey(pos))
            {
                ValidTiles.Add(pos, 0);
            }
        }
        if (IndirectUnit)
        {
            extraTiles.Clear();
            ValidTiles.Clear();

            AttackFromTiles(extraTiles, GetGridPosition(), Weapons[CurrentWeaponIndex].MaxRange);
            foreach (var pos in extraTiles)
            {
                if (L1Distance2D(GetGridPosition(), pos) >= Weapons[CurrentWeaponIndex].MinRange)
                {
                    ValidTiles.Add(pos, 0);
                }
            }
        }

        foreach (var pos in ValidTiles.Keys)
        {
            if (ValidTiles[pos] <= Provisions)
            {
                _mm.Map.SetTileFlags(pos, TileFlags.None);
                _mm.HighlightAttackTile(pos);
            }
        }
    }

    private void ExpandFromTiles(List<Vector3Int> list, Vector3Int currentPosition, int range)
    {
        if (range == 0) { return; }
        if (range != Weapons[CurrentWeaponIndex].MaxRange)
        {
            if (!ValidTiles.ContainsKey(currentPosition) && !list.Contains(currentPosition))
            {
                if (_mm.Map.GetTile<Tile>(currentPosition))
                {
                    list.Add(currentPosition);
                }
                else { return; }
            }
        }

        Vector3Int up = currentPosition + Vector3Int.up;
        Vector3Int down = currentPosition + Vector3Int.down;
        Vector3Int left = currentPosition + Vector3Int.left;
        Vector3Int right = currentPosition + Vector3Int.right;

        ExpandFromTiles(list, up, range - 1);
        ExpandFromTiles(list, down, range - 1);
        ExpandFromTiles(list, left, range - 1);
        ExpandFromTiles(list, right, range - 1);
    }

    private void AttackFromTiles(List<Vector3Int> list, Vector3Int currentPosition, int range)
    {
        if (range == 0) { return; }
        if (range != Weapons[CurrentWeaponIndex].MaxRange)
        {
            if (!list.Contains(currentPosition))
            {
                if (_mm.Map.GetTile<Tile>(currentPosition))
                {
                    list.Add(currentPosition);
                }
                else { return; }
            }
        }

        Vector3Int up = currentPosition + Vector3Int.up;
        Vector3Int down = currentPosition + Vector3Int.down;
        Vector3Int left = currentPosition + Vector3Int.left;
        Vector3Int right = currentPosition + Vector3Int.right;

        AttackFromTiles(list, up, range - 1);
        AttackFromTiles(list, down, range - 1);
        AttackFromTiles(list, left, range - 1);
        AttackFromTiles(list, right, range - 1);
    }

    //public void InitiateTargetSelection()
    //{
    //    Debug.Log("Initiating target selection from the AU");
    //    AttackManager.Instance.InitiateTargetSelection(this);
    //    Debug.Log("Finished");
    //}

    //// Method to handle keyboard input for navigating through targets
    //private void HandleTargetSelectionInput()
    //{
    //    AttackManager.Instance.HandleTargetSelectionInput();
    //}

    // Update method to handle target selection input
    //private void Update()
    //{
    //    HandleTargetSelectionInput();
    //}

    public AttackingUnitSaveData GetDataToSave()
    {
        return new AttackingUnitSaveData(UnitType, Health, Provisions, Owner, HasMoved, CurrentWeaponAmmo,
            CurrentWeaponIndex, GetGridPosition(), IsCapturing);
    }

    public void SetSavedData(AttackingUnitSaveData saveData)
    {
        UnitType = saveData.UnitType;
        Health = saveData.Health;
        Provisions = saveData.Provisions;
        Owner = saveData.Owner;
        HasMoved = saveData.HasMoved;
        CurrentWeaponAmmo = saveData.EnergyOrbs;
        CurrentWeaponIndex = saveData.CurrentWeaponIndex;
        IsCapturing = saveData.IsCapturing;
    }
}



