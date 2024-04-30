using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class AttackingUnit : Unit
{
    #region Variables
    // List of damages that this attacking unit can apply to other units using each weapon 
    [SerializeField] List<Weapon> _weapons;

    public List<Weapon> Weapons { get => _weapons; set => _weapons = value; }
    public int CurrentWeaponIndex = 0;
    private bool _isAttacking = false;
    public bool HasAttacked = false;
    [SerializeField] public bool IndirectUnit;

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
    #endregion

    #region UnityMethods
    private void OnEnable()
    {
        Weapon.OnAmmoRanOut += MoveToNextWeapon;
    }

    private void OnDisable()
    {
        Weapon.OnAmmoRanOut -= MoveToNextWeapon;
    }
    #endregion

    #region Methods
    // scans area for targets in an Intervall [ min range, max range[
    // Assumed that every unit can be in one tile which can be in one grid position
    public List<Unit> ScanTargets()
    {
        var attackerPos = GetGridPosition();
        List<Unit> targets = new();

        foreach (var unit in _um.Units)
        {
            if (unit == this) continue;

            var potentialTargetPos = _mm.Map.WorldToCell(unit.transform.position);

            var currentWeapon = Weapons[CurrentWeaponIndex]; // getting the current weapon from the attacker

            bool IsInRange = (L1Distance2D(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance2D(attackerPos, potentialTargetPos) < currentWeapon.MaxRange);
            bool IsEnemy = Owner != unit.Owner;
            bool IsDamageable = Weapons[CurrentWeaponIndex].DamageList[(int)unit.Data.UnitType] != 0;

            // print($"{L1Distance2D(attackerPos, potentialTargetPos)} / {currentWeapon.MinRange} / {currentWeapon.MaxRange} / {unit}");
            if (IsInRange && IsEnemy && IsDamageable)
            {

                targets.Add(unit);
            }
        }
        // print("targets : " + targets.Count);
        return targets;
    }

    public bool CheckAttack()
    {
        var attackerPos = GetGridPosition();

        foreach (var unit in _um.Units)
        {
            if (unit == this || !unit.gameObject.activeInHierarchy) continue;

            var potentialTargetPos = unit.GetGridPosition();

            var currentWeapon = Weapons[CurrentWeaponIndex];// getting the current weapon from the attacker

            bool IsInRange = (L1Distance2D(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance2D(attackerPos, potentialTargetPos) < currentWeapon.MaxRange);
            bool IsEnemy = Owner != unit.Owner;
            bool IsDamageable = Weapons[CurrentWeaponIndex].DamageList[(int)unit.Data.UnitType] != 0;
            //print(IsDamageable);
            //print($"{L1Distance2D(attackerPos, potentialTargetPos)} / {currentWeapon.MinRange} / {currentWeapon.MaxRange} / {unit}");
            if (IsInRange && IsEnemy && IsDamageable)
            {
                return true;
            }
        }
        return false;
    }

    public void UnHighlightTargets()
    {
        List<Unit> targets = ScanTargets();
        foreach (var target in targets)
        {
            // Change the material color of the target to white
            if (target.TryGetComponent<Renderer>(out var renderer))
            {
                MaterialPropertyBlock propBlock = new();
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", Color.white); // Set the color to white
                renderer.SetPropertyBlock(propBlock);
            }
        }
    }

    public void UnHighlightTarget(Unit target)
    {
        if (target.TryGetComponent<Renderer>(out var renderer))
        {
            MaterialPropertyBlock propBlock = new();
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", Color.white); // Set the color to red
            renderer.SetPropertyBlock(propBlock);
        }
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
            foreach (var pos in ValidTiles.Keys)
            {
                float dis = L1Distance2D(GetGridPosition(), pos);
                if ((dis >= Weapons[CurrentWeaponIndex].MinRange && dis < Weapons[CurrentWeaponIndex].MaxRange))
                {
                    extraTiles.Add(pos);
                }
            }
            ValidTiles.Clear();
            foreach (var pos in extraTiles)
            {
                ValidTiles.Add(pos, 0);
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
            else { return; }
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
    #endregion
}


