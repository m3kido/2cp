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
                rend.color = Color.red;
            }
            else
            {
                rend.color = Color.white;
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

    /// <summary>
    /// Sets the loaded Data
    /// </summary>
    public void SetSaveData(AttackingUnitSaveData saveData)
    {
        Health = saveData.Health;
        Fuel = saveData.Fuel;
        Owner = saveData.Owner;
        Type = saveData.Type;
        HasMoved = saveData.HasMoved;
        CurrentWeaponIndex = saveData.CurrentWeaponIndex;
        Weapons[CurrentWeaponIndex] = saveData.CurrentWeapon;
        HasAttacked = saveData.HasAttacked;
    }

    /// <summary></summary>
    /// <returns>Data to be loaded</returns>
    public AttackingUnitSaveData GetSaveData()
    {
        return new AttackingUnitSaveData(Health, Fuel, Owner, Type, HasMoved, Weapons[CurrentWeaponIndex], CurrentWeaponIndex, HasAttacked, GetGridPosition());
    }

    public float CalculateDamage(Unit target, AttackingUnit attacker)
    {

        int baseDamage = _weapons[CurrentWeaponIndex].DamageList[(int)target.Type];
        Player attackerPlayer = Gm.Players[attacker.Owner];
        Captain attackerCaptain = attackerPlayer.Captain;
        int celesteAttack = attackerPlayer.IsCelesteActive ? attackerCaptain.Data.CelesteDefense : 0;
        float attackDamage = baseDamage * (1 + attackerCaptain.Data.PassiveAttack) * (1 + celesteAttack);


        int terrainStars = Mm.GetTileData(Mm.Map.WorldToCell(target.transform.position)).Defence;
        Player targetPlayer = Gm.Players[attacker.Owner];
        Captain targetCaptain = targetPlayer.Captain;
        int celesteDefense = targetPlayer.IsCelesteActive ? targetCaptain.Data.CelesteDefense : 0;
        float defenseDamage = (1 - terrainStars * target.Health / 1000) * (1 - targetCaptain.Data.PassiveDefense) * (1 - celesteDefense);


        int chance = (attackerCaptain.Data.Name == ECaptains.Andrew) ? UnityEngine.Random.Range(2, 10) : UnityEngine.Random.Range(1, 10);
        float totalDamage = (float)attacker.Health / 100 * attackDamage * defenseDamage * (1 + (float)chance / 100);
        return totalDamage;
    }



    // scans area for targets in an Intervall [ min range, max range[
    // Assumed that every unit can be in one tile which can be in one grid position
    public List<Unit> ScanTargets()
    {
        var attackerPos = GetGridPosition();
        List<Unit> targets = new();

        foreach (var unit in Um.Units)
        {

            var potentialTargetPos = Mm.Map.WorldToCell(unit.transform.position);

            var currentWeapon = Weapons[CurrentWeaponIndex];// getting the current weapon from the attacker

            bool IsInRange = (L1Distance(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance(attackerPos, potentialTargetPos) < currentWeapon.MaxRange);
            bool IsEnemy = Owner != unit.Owner;
            bool IsDamageable = Weapons[0].DamageList[(int)unit.Type] != 0;

            if (IsInRange && IsEnemy && IsDamageable)
            {
                targets.Add(unit);
            }
        }

        return targets;
    }


    public void HighlightTargets()
    {
        List<Unit> targets = ScanTargets();
        Debug.Log("You can attack " + targets.Count + " enemies");
        foreach (var target in targets)
        {
            // Change the material color of the target to red
            if (target.TryGetComponent<Renderer>(out var renderer))
            {
                MaterialPropertyBlock propBlock = new();
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", Color.blue); // Set the color to red
                renderer.SetPropertyBlock(propBlock);
            }
        }
    }
    public void UnHighlightTargets()
    {
        List<Unit> targets = ScanTargets();
        foreach (var target in targets)
        {
            // Change the material color of the target to red
            if (target.TryGetComponent<Renderer>(out var renderer))
            {
                MaterialPropertyBlock propBlock = new();
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", Color.white); // Set the color to red
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


