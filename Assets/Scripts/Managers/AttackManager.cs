using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    GameManager Gm;
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
            print($"-----------ActionTaken : {value}");
        }
    }

    private void Awake()
    {
        Gm = FindAnyObjectByType<GameManager>();
        ActionTaken = false;
    }

    public bool UnitCanAttack(AttackingUnit attacker)
    {
        if (attacker == null) print("NO ATTACKER FOUND ");
        return attacker.CanAttack();
    }

    public void ApplyDamage(Unit target, AttackingUnit attacker)
    {
        if (target == null || attacker == null)
        {
            Debug.LogError("Target or attacker is null.");
            return;
        }

        float damageToTarget = attacker.CalculateDamage(target, attacker);
        Debug.Log("Damage to target: " + damageToTarget);
        target.Health -= (int)damageToTarget;
        Debug.Log("Target has been damaged!");

        if (target.Health > 0 && target is AttackingUnit)
        {
            var damageToAttacker = attacker.CalculateDamage(attacker, target as AttackingUnit);
            attacker.Health -= (int)damageToAttacker;
            Debug.Log("Damage to attacker: " + damageToAttacker);
            Debug.Log("Attacker has been damaged!");
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
        Gm.GameState = EGameStates.Attacking;
        Attacker.IsAttacking = true;
        Attacker.HasAttacked = false;


        List<Unit> targets = Attacker.ScanTargets();

        if (targets.Count > 0)
        {
            Debug.Log(Attacker + " can attack");
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
            HighlightSelectedTarget(targets[0]);
        }
        else
        {
            HighlightSelectedTarget(targets[selectedTargetIndex]);

            // Handle navigation keys
            if (Input.GetKeyDown(KeyCode.Z))
            {
                // Move to the next target (circular)
                Debug.LogWarning("Z clicked");
                attacker.UnHighlightTarget(targets[selectedTargetIndex]);
                selectedTargetIndex = (selectedTargetIndex + 1) % targets.Count;
                HighlightSelectedTarget(targets[selectedTargetIndex]);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // Move to the previous target (circular)
                Debug.LogWarning("D clicked");
                attacker.UnHighlightTarget(targets[selectedTargetIndex]);
                selectedTargetIndex = (selectedTargetIndex - 1 + targets.Count) % targets.Count;
                HighlightSelectedTarget(targets[selectedTargetIndex]);
            }
        }

        // Handle attack confirmation and cancellation
        if (Input.GetKeyDown(KeyCode.A)) // Assuming "A" key is used to confirm attack
        {
            // Apply damage to the selected target
            Unit selectedTarget = targets[selectedTargetIndex];
            ApplyDamage(selectedTarget, attacker);
            ActionTaken = true;
        }

        if (Input.GetKeyDown(KeyCode.X)) // Assuming "X" key is used to cancel attack
        {
            attacker.UnHighlightTargets();
            ActionTaken = true;

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



    public void EndAttackPhase()
    {
        Attacker.IsAttacking = false;
        Attacker.HasAttacked = true;
        Gm.GameState = EGameStates.Idle;
        Attacker.UnHighlightTargets();
        Attacker = null;


    }



}
