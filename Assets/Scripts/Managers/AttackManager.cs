using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AttackManager : MonoBehaviour
{
    GameManager _gm;
    UnitManager _um;
    MapManager _mm;
    List<EUnits> directAttacker = new List<EUnits> { EUnits.Catapult, EUnits.Cannoneer, EUnits.Ballista };
    public AttackingUnit Attacker;
    private int selectedTargetIndex = -1;
    private bool _actionTaken = false;
    public bool ActionTaken
    {
        get
        {
            return _actionTaken;
        }
        set
        {
            _actionTaken = value;
            
        }
    }

    private void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _mm = FindAnyObjectByType<MapManager>();
    }

    public bool UnitCanAttack(AttackingUnit attacker)
    {
        if (attacker == null) print("NO ATTACKER FOUND ");
        return attacker.CheckAttack();
    }

    public void ApplyDamage(Unit target, AttackingUnit attacker)
    {
        if (target == null || attacker == null)
        {
            Debug.LogError("Target or attacker is null.");
            return;
        }

        float damageToTarget = CalculateDamage(target, attacker);
        Debug.Log("Damage to target: " + damageToTarget);
        target.Health -= (int)damageToTarget;
        Debug.Log("Target has been damaged!");

        if (target.Health > 0 && target is AttackingUnit && !directAttacker.Contains(attacker.Data.UnitType)) //We need to check if target unit can attack the attacker)
        {
            AttackingUnit newAttacker = target as AttackingUnit;
            if (newAttacker.CanAttackThis(attacker))
            {
                var damageToAttacker = CalculateDamage(attacker, newAttacker);
                attacker.Health -= (int)damageToAttacker;
                Debug.Log("Damage to attacker: " + damageToAttacker);
                Debug.Log("Attacker has been damaged!");
            }

        }
    }

    public void InitiateAttack()
    {
        Debug.Log("Initiating Attack from the AM");
        if (Attacker == null)
        {
            Debug.LogWarning("Cannot initiate attack. Attacker is null.");
            return;
        }

        // Set the game state to Attacking
        _gm.CurrentStateOfPlayer = EPlayerStates.Attacking;
        Attacker.IsAttacking = true;
        Attacker.HasAttacked = false;


        List<Unit> targets = Attacker.ScanTargets();

        if (targets.Count > 0)
        {
            Debug.Log(Attacker + " can attack : " + targets.Count + " targets");
            selectedTargetIndex = 0;

            // Start target selection process
            StartCoroutine(TargetSelectionCoroutine(Attacker, targets));
        }
        else
        {
            Debug.Log("No valid targets found.");
        }
    }

    private IEnumerator TargetSelectionCoroutine(AttackingUnit attacker, List<Unit> targets)
    {
        yield return null;// skip 1 frame so the space clicked to confirm the attack option 
                          // selection doesn't confirm the target selection instantly 
        while (!ActionTaken)
        {
            HandleTargetSelectionInput(attacker, targets);

            // Wait for the next frame
            yield return null;
        }
        ActionTaken = false;


        Debug.Log("Action finished");
    }

    private void HandleTargetSelectionInput(AttackingUnit attacker, List<Unit> targets)
    {

        // Check if there are any targets to select
        if (selectedTargetIndex == -1)
        {
            Debug.LogWarning("No targets available for selection.");
            return;
        }

        if (targets.Count == 1)
        {
            // HighlightSelectedTarget(targets[0]);
            targets[selectedTargetIndex]._rend.color = Color.blue;
        }
        else
        {
            // HighlightSelectedTarget(targets[selectedTargetIndex]);
            targets[selectedTargetIndex]._rend.color = Color.blue;

            // Handle navigation keys
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Move to the next target (circular)
                // attacker.UnHighlightTarget(targets[selectedTargetIndex]);
                targets[selectedTargetIndex]._rend.color=Color.white;
                selectedTargetIndex = (selectedTargetIndex + 1) % targets.Count;
                // HighlightSelectedTarget(targets[selectedTargetIndex]);
                targets[selectedTargetIndex]._rend.color = Color.blue;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // attacker.UnHighlightTarget(targets[selectedTargetIndex]);
                targets[selectedTargetIndex]._rend.color = Color.white;
                selectedTargetIndex = (selectedTargetIndex - 1 + targets.Count) % targets.Count;
                // HighlightSelectedTarget(targets[selectedTargetIndex]);
                targets[selectedTargetIndex]._rend.color = Color.blue;
            }
        }

        // Handle attack confirmation and cancellation
        if (Input.GetKeyDown(KeyCode.Space)) // Assuming "Space" key is used to confirm attack
        {
            // Apply damage to the selected target
            Unit selectedTarget = targets[selectedTargetIndex];
            ApplyDamage(selectedTarget, attacker);
            ActionTaken = true;
            EndAttackPhase();//end attack
            _um.EndMove();//terminate move
        }

        if (Input.GetKeyDown(KeyCode.X)) // Assuming "X" key is used to cancel attack
        {
            attacker.UnHighlightTargets();
            ActionTaken = true;
            EndAttackPhase();//end attack
            _gm.CurrentStateOfPlayer = EPlayerStates.InActionsMenu;//return to action menu

        }
    }

    private void HighlightSelectedTarget(Unit target)
    {
        // Check if the target is null or has been destroyed
        if (target == null)
        {
            Debug.LogWarning("Target is null or has been destroyed.");
            return;
        }

        // Check if the target's GameObject is null or has been destroyed
        if (target.gameObject == null)
        {
            Debug.LogWarning("Target's GameObject is null or has been destroyed.");
            return;
        }

        // Try to get the Renderer component of the target's GameObject
        if (target.gameObject.TryGetComponent<Renderer>(out var renderer))
        {
            MaterialPropertyBlock propBlock = new();
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", Color.blue);
            renderer.SetPropertyBlock(propBlock);
        }
        else
        {
            Debug.LogWarning("Target does not have a Renderer component.");
        }
    }



    public float CalculateDamage(Unit target, AttackingUnit attacker)
    {

        int baseDamage = attacker.Weapons[attacker.CurrentWeaponIndex].DamageList[(int)target.Data.UnitType];
        Captain attackerCaptain = attacker.GetUnitCaptain;
        int celesteAttack = attackerCaptain.IsCelesteActive ? attackerCaptain.Data.CelesteDefense : 0;
        float attackDamage = baseDamage * (1 + attackerCaptain.PassiveAttack) * (1 + celesteAttack) * attackerCaptain.AttackMultiplier; Debug.Log("AttackMultiplier" + attackerCaptain.AttackMultiplier);


        int terrainStars = _mm.GetTileData(_mm.Map.WorldToCell(target.transform.position)).DefenceStars;
        Captain targetCaptain = target.GetUnitCaptain;
        int celesteDefense = targetCaptain.IsCelesteActive ? targetCaptain.Data.CelesteDefense : 0;
        float defenseDamage = (1 - terrainStars * target.Health / 1000) * (1 - targetCaptain.PassiveDefense) * (1 - celesteDefense) * targetCaptain.DefenseMultiplier; Debug.Log("DefenseMultiplier : " + targetCaptain.DefenseMultiplier);

        int chance = (attackerCaptain.Data.Name == ECaptains.Andrew) ? UnityEngine.Random.Range(2, 10) : UnityEngine.Random.Range(1, 10);
        float totalDamage = (float)attacker.Health / 100 * attackDamage * defenseDamage * (1 + (float)chance / 100);
        return totalDamage;
    }



    public void EndAttackPhase()
    {
        Attacker.IsAttacking = false;
        Attacker.HasAttacked = true;
        Attacker.UnHighlightTargets();
        Attacker = null;


    }



}
