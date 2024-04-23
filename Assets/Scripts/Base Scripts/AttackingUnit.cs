using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackingUnit : Unit
{
    // List of damages that this attacking unit can apply to other units using each weapon 
    [SerializeField] List<Weapon> _weapons;

    public List<Weapon> Weapons { get => _weapons; set => _weapons = value; }
    public int CurrentWeaponIndex = 0;
    private bool _isAttacking = false;
    public bool HasAttacked = false;

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

    private void OnEnable()
    {
        Weapon.OnAmmoRanOut += MoveToNextWeapon;
    }

    private void OnDisable()
    {
        Weapon.OnAmmoRanOut -= MoveToNextWeapon;
    }

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

            var currentWeapon = Weapons[CurrentWeaponIndex];// getting the current weapon from the attacker

            bool IsInRange = (L1Distance2D(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance2D(attackerPos, potentialTargetPos) < currentWeapon.MaxRange);
            bool IsEnemy = Owner != unit.Owner;
            bool IsDamageable = Weapons[CurrentWeaponIndex].DamageList[(int)unit.Data.UnitType] != 0;

            //print($"{L1Distance2D(attackerPos, potentialTargetPos)} / {currentWeapon.MinRange} / {currentWeapon.MaxRange} / {unit}");
            if (IsInRange && IsEnemy && IsDamageable)
            {

                targets.Add(unit);
            }
        }
        //print("targets : " + targets.Count);
        return targets;
    }
    public bool CheckAttack()
    {
        var attackerPos = GetGridPosition();
        

        foreach (var unit in _um.Units)
        {
            if (unit == this) continue;

            var potentialTargetPos = _mm.Map.WorldToCell(unit.transform.position);

            var currentWeapon = Weapons[CurrentWeaponIndex];// getting the current weapon from the attacker

            bool IsInRange = (L1Distance2D(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance2D(attackerPos, potentialTargetPos) < currentWeapon.MaxRange);
            bool IsEnemy = Owner != unit.Owner;
            bool IsDamageable = Weapons[CurrentWeaponIndex].DamageList[(int)unit.Data.UnitType] != 0;
            print(IsInRange.ToString() +IsEnemy.ToString());
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

    public bool CanAttack()
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

    public void MoveToNextWeapon()
    {
        if (CurrentWeaponIndex < Weapons.Count - 1) CurrentWeaponIndex++; // Cannot exceed last weapon index

    }

    public void ResetWeapons()
    {
        CurrentWeaponIndex = 0;
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


}


