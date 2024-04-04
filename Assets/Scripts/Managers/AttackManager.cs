using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{

    // Method to initiate an attack
    public static AttackManager Instance { get; private set; }
    GameManager Gm;
    public AttackingUnit attacker;
    private int selectedTargetIndex = -1;
    private void Awake()
    {
        Gm = FindAnyObjectByType<GameManager>();
        Instance = this;
    }

    // Method to initiate an attack


    // Method to check if a unit can attack
    public bool UnitCanAttack(AttackingUnit attacker)
    {
        if (attacker == null) print("NO ATTACKER FOUND ");
        return attacker.CanAttack(attacker);
    }

    public void ApplyDamage(Unit target, AttackingUnit attacker)
    {
        if (target == null || attacker == null)
        {
            Debug.LogError("Target or attacker is null.");
            return;
        }

        float damageToTarget = attacker.CalculateDamage(target, attacker);
        Debug.Log("Dammage : " + damageToTarget);
        target.Health -= (int)damageToTarget;
        Debug.Log("target has been dammaged!");
        // If the target is still alive, apply counter-attack to attacker
        if (target.Health > 0 && target is AttackingUnit)
        {
            var damageToAttacker = attacker.CalculateDamage(attacker, target as AttackingUnit);
            attacker.Health -= (int)damageToAttacker;
            Debug.Log("Dammage : " + damageToAttacker);
            Debug.Log("attacker has been dammaged!");
        }

    }




    private void DefeatUnit(Unit unit)
    {
        Destroy(unit);
    }

    public void InitiateAttack(AttackingUnit attacker)
    {
        Debug.Log("Initiating Attack from the AM");
        if (attacker == null)
        {
            Debug.LogWarning("Cannot initiate attack. Attacker is null.");
            return;
        }

        // Initiate target selection for the attacker
        attacker.InitiateTargetSelection();
        Debug.Log("Action finished");
    }




    // Call this function to initialize the target selection process
    public void InitiateTargetSelection(AttackingUnit attacker)
    {
        Debug.Log("Initiating target selection from the AM");
        List<Unit> targets = attacker.ScanTargets(attacker);
        if (targets.Count > 0)
        {
            // Start target selection from the first target
            Debug.Log(attacker + " can attack");
            selectedTargetIndex = 0;
            HandleTargetSelectionInput();
        }
        Debug.Log("Finished");
    }

    // Method to highlight the selected target
    private void HighlightSelectedTarget(Unit target)
    {
        if (target.TryGetComponent<Renderer>(out var renderer))
        {
            MaterialPropertyBlock propBlock = new();
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", Color.blue); // Set the color to red
            renderer.SetPropertyBlock(propBlock);
        }
    }

    // Method to handle keyboard input for navigating through targets
    public bool HandleTargetSelectionInput()
    {
        Debug.Log("Handling target selection");
        // Check if there are any targets to select
        if (selectedTargetIndex == -1)
        {
            Debug.LogWarning("No targets available for selection.");
            return false;
        }

        List<Unit> targets = attacker.ScanTargets(attacker);

        HighlightSelectedTarget(targets[selectedTargetIndex]);

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
            Debug.LogWarning("D clickeddzd");
            attacker.UnHighlightTarget(targets[selectedTargetIndex]);
            selectedTargetIndex = (selectedTargetIndex - 1 + targets.Count) % targets.Count;
            HighlightSelectedTarget(targets[selectedTargetIndex]);

        }
        if (Input.GetKeyDown(KeyCode.A)) // Assuming "A" key is used to confirm attack
        {
            // Apply damage to the selected target
            Unit selectedTarget = targets[selectedTargetIndex];
            ApplyDamage(selectedTarget, attacker);
            attacker.UnHighlightTargets(attacker);
            attacker.IsAttacking = false;
            attacker.HasAttacked = true;
            Gm.GameState = EGameStates.Idle;
            return true; // Return true to indicate attack is confirmed
        }
        else if (Input.GetKeyDown(KeyCode.X)) // Assuming "X" key is used to cancel attack
        {
            attacker.UnHighlightTargets(attacker);
            EndAttackPhase();
            return false; // Return false to indicate attack is canceled
        }


        return false;
    }

    public void EndAttackPhase()
    {
        // Reset the attacking state to false
        attacker.IsAttacking = false;
        Gm.GameState = EGameStates.ActionMenu;
    }
    void Update()
    {
        if (Gm.GameState == EGameStates.Attacking)
        {
            HandleTargetSelectionInput();

        }


    }



}