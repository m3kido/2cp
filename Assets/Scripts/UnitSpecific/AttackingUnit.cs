using System.Collections.Generic;
using UnityEngine;

public class AttackingUnit : Unit
{
    // List of damages that this attacking unit can apply to other units using each weapon 
    [SerializeField] List<Weapon> _weapons;

    public List<Weapon> Weapons { get => _weapons; set => _weapons = value; }
    public int CurrentWeaponIndex = 0;

    private bool _hasAttacked = false;

    private void OnEnable()
    {
        Weapon.OnAmmoRanOut += MoveToNextWeapon;
    }

    private void OnDisable()
    {
        Weapon.OnAmmoRanOut -= MoveToNextWeapon;
    }

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
                rend.color = Color.red;
            }
            else
            {
                rend.color = Color.white;
            }
        }
    }


    public float CalculateDamage(Unit target, AttackingUnit attacker)
    {

        int baseDamage = _weapons[CurrentWeaponIndex].DamageList[(int)target.Type];
        Player attackerPlayer = Gm.Players[attacker.Owner];
        Captain attackerCaptain = attackerPlayer.Captain;
        int celesteAttack = attackerPlayer.IsCelesteActive ? attackerCaptain.CelesteDefense : 0;
        float attackDamage = baseDamage * (1 + attackerCaptain.PassiveAttack) * (1 + celesteAttack);


        int terrainStars = Mm.GetTileData(Mm.Map.WorldToCell(target.transform.position)).Defence;
        Player targetPlayer = Gm.Players[attacker.Owner];
        Captain targetCaptain = targetPlayer.Captain;
        int celesteDefense = targetPlayer.IsCelesteActive ? targetCaptain.CelesteDefense : 0;
        float defenseDamage = (1 - terrainStars * target.Health / 1000) * (1 - targetCaptain.PassiveDefense) * (1 - celesteDefense);


        int chance = (attackerCaptain.Name == Captains.Andrew) ? UnityEngine.Random.Range(2, 10) : UnityEngine.Random.Range(1, 10);
        float totalDamage = attacker.Health / 100 * attackDamage * defenseDamage * (1 + chance / 100);
        return totalDamage;
    }

    void ApplyDamage(Unit target, AttackingUnit attacker)
    {
        var damage = CalculateDamage(target, attacker);

        target.Health -= (int)damage;
        if (target != null)
        {
            damage = CalculateDamage(target, attacker);
            attacker.Health -= (int)damage;
        }


    }

    // scans area for targets in an Intervall [ min range, max range[
    // Assumed that every unit can be in one tile which can be in one grid position
    List<Unit> ScanTargets(AttackingUnit attacker)
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

            var currentWeapon = attacker.Weapons[CurrentWeaponIndex];// getting the current weapon from the attacker

            bool IsInRange = (L1Distance(attackerPos, potentialTargetPos) >= currentWeapon.MinRange) && (L1Distance(attackerPos, potentialTargetPos) < currentWeapon.MaxRange);
            bool IsEnemy = attacker.Owner != unit.Owner;
            bool IsDamageable = attacker._weapons[CurrentWeaponIndex].DamageList[(int)unit.Type] != 0;

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
        foreach (var target in targets)
        {
            // Change the material color of the target to red
            if (target.TryGetComponent<Renderer>(out var renderer))
            {
                MaterialPropertyBlock propBlock = new();
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", Color.red); // Set the color to red
                renderer.SetPropertyBlock(propBlock);
            }
        }
    }

    public bool CanAttack(AttackingUnit attacker)
    {
        // Return true if there are targets available, otherwise return false
        if (attacker == null)
        {
            Debug.LogWarning("Attacker is null. Unable to check if it can attack.");
            return false;
        }
        return ScanTargets(attacker).Count > 0;
    }

    public void MoveToNextWeapon()
    {
        if (CurrentWeaponIndex < Weapons.Count - 1) CurrentWeaponIndex++; // Cannot exceed last weapon index

    }

    public void ResetWeapons()
    {
        CurrentWeaponIndex = 0;
    }


}


