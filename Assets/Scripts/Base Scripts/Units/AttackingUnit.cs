using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackingUnit : Unit
{
    [SerializeField] List<Weapon> _weapons = new();
    public List<Weapon> Weapons { get; set; }
    public int EnergyOrbs;
    public int CurrentWeaponIndex;
    public bool isAttacking;
    private bool _hasAttacked;

    public bool HasAttacked
    {
        get
        {
            return _hasAttacked;
        }
        set
        {
            _hasAttacked = value;
            if (_hasAttacked)
            {
                Rend.color = Color.red;
            }
            else
            {
                Rend.color = Color.white;
            }
        }
    }

    private void OnEnable()
    {
        Weapon.OnEnergyRanOut += MoveToNextWeapon;
    }

    private void OnDisable()
    {
        Weapon.OnEnergyRanOut -= MoveToNextWeapon;
    }

    public float CalculateDamage(Unit target, AttackingUnit attacker)
    {
        int baseDamage = _weapons[CurrentWeaponIndex].DamageList[(int)target.UnitType];
        PlayerInGame attackerPlayer = Gm.InGamePlayers[attacker.Owner];
        Captain attackerCaptain = attackerPlayer.PlayerCaptain;
        int celesteAttack = attackerPlayer.IsCPActivated ? attackerCaptain.CelesteDefense : 0;
        float attackDamage = baseDamage * (1 + attackerCaptain.PassiveAttack) * (1 + celesteAttack);


        int terrainStars = Mm.GetTileData(Mm.Map.WorldToCell(target.transform.position)).DefenceStars;
        PlayerInGame targetPlayer = Gm.InGamePlayers[attacker.Owner];
        Captain targetCaptain = targetPlayer.PlayerCaptain;
        int celesteDefense = targetPlayer.IsCPActivated ? targetCaptain.CelesteDefense : 0;
        float defenseDamage = (1 - terrainStars * target.Health / 1000) * (1 - targetCaptain.PassiveDefense) * (1 - celesteDefense);


        int chance = (attackerCaptain.Name == Captains.Andrew) ? UnityEngine.Random.Range(2, 10) : UnityEngine.Random.Range(1, 10);
        float totalDamage = attacker.Health / 100 * attackDamage * defenseDamage * (1 + chance / 100);
        return totalDamage;
    }



    // scans area for targets in an Intervall [ min range, max range[
    // Assumed that every unit can be in one tile which can be in one grid position
    public List<Unit> ScanTargets(AttackingUnit attacker)
    {
        if (attacker == null || attacker.transform == null || Mm == null)
        {
            Debug.LogError("Error: Null reference detected in ScanTargets method.");
            return new List<Unit>();
        }

        var attackerPos = Mm.Map.WorldToCell(attacker.transform.position);
        List<Unit> targets = new();

        foreach (var unit in Um.Units)
        {
            var potentialTargetPos = Mm.Map.WorldToCell(unit.transform.position);

            var currentWeapon = attacker.Weapons[CurrentWeaponIndex]; // Getting the current weapon from the attacker

            bool IsInRange = (L1Distance(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance(attackerPos, potentialTargetPos) < currentWeapon.MaxRange);
            bool IsEnemy = attacker.Owner != unit.Owner;
            bool IsDamageable = attacker._weapons[CurrentWeaponIndex].DamageList[(int)unit.UnitType] != 0;

            if (IsInRange && IsEnemy && IsDamageable)
            {
                targets.Add(unit);
            }
        }
        return targets;
    }

    public void HighlightTargets(AttackingUnit attacker)
    {
        List<Unit> targets = ScanTargets(attacker);
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

    public void UnHighlightTargets(AttackingUnit attacker)
    {
        List<Unit> targets = ScanTargets(attacker);
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

    public bool CanAttack(AttackingUnit attacker)
    {
        List<Unit> targets = ScanTargets(attacker);
        if (attacker == null)
        {
            Debug.LogWarning("Attacker is null. Unable to check if it can attack.");
            return false;
        }
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

    public void InitiateTargetSelection()
    {
        Debug.Log("Initiating target selection from the AU");
        AttackManager.Instance.InitiateTargetSelection(this);
    }

    // Method to handle keyboard input for navigating through targets
    private void HandleTargetSelectionInput()
    {
        AttackManager.Instance.HandleTargetSelectionInput();
    }

    /*
    // Update method to handle target selection input
    private void Update()
    {
        HandleTargetSelectionInput();
    } 
    */

    /// <summary></summary>
    /// <returns>Data to be loaded</returns>
    public AttackingUnitSaveData GetDataToSave()
    {
        // HERE!!! : i set weapon at null because there's an error, try passing Weapons[CurrentWeaponIndex]
        return new AttackingUnitSaveData(UnitType, Health, Provisions, Owner, HasMoved, EnergyOrbs,
            null, CurrentWeaponIndex, GetGridPosition());
    }

    /// <summary>
    /// Sets the loaded Data
    /// </summary>
    public void SetSavedData(AttackingUnitSaveData saveData)
    {
        Health = saveData.Health;
        Provisions = saveData.Provisions;
        Owner = saveData.Owner;
        UnitType = saveData.UnitType;
        HasMoved = saveData.HasMoved;
        CurrentWeaponIndex = saveData.CurrentWeaponIndex;
        // HERE!!! : It also causes a problem
        // Weapons[CurrentWeaponIndex] = saveData.CurrentWeapon;
    }
}


