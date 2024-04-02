using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{

    // Method to initiate an attack
    public static AttackManager Instance { get; private set; }
    GameManager Gm;
    public AttackingUnit attacker;
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
            // Handle null references, maybe throw an exception or log an error
            return;
        }

        var damageToTarget = attacker.CalculateDamage(target, attacker);
        target.Health -= (int)damageToTarget;

        if (target.Health <= 0)
        {
            // Target is defeated, handle accordingly (remove from game, trigger events, etc.)
            DefeatUnit(target);
        }
        else
        {
            // If the target is still alive, apply counter-attack to attacker
            var damageToAttacker = attacker.CalculateDamage(target, attacker);
            attacker.Health -= (int)damageToAttacker;

            if (attacker.Health <= 0)
            {
                // Attacker is defeated, handle accordingly (remove from game, trigger events, etc.)
                DefeatUnit(attacker);
            }
        }
    }

    private void DefeatUnit(Unit unit)
    {
        // Meskina
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
    }


    private int selectedTargetIndex = -1;

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

        // Handle keyboard input
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Move to the next target (circular)
            Debug.LogWarning("Z clicked");
            selectedTargetIndex = (selectedTargetIndex + 1) % targets.Count;
            HighlightSelectedTarget(targets[selectedTargetIndex]);
            if (Input.GetKeyDown(KeyCode.A)) // Assuming "A" key is used to confirm attack
            {
                // Apply damage to the selected target
                Unit selectedTarget = targets[selectedTargetIndex];
                ApplyDamage(selectedTarget, attacker);
                Debug.Log("EA SPORTS , IT'S IN THE GAME");
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.X)) // Assuming "A" key is used to confirm attack
            {
                EndAttackPhase();
                return false;


            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // Move to the previous target (circular)
            Debug.LogWarning("D clickeddzd");
            selectedTargetIndex = (selectedTargetIndex - 1 + targets.Count) % targets.Count;
            HighlightSelectedTarget(targets[selectedTargetIndex]);
            if (Input.GetKeyDown(KeyCode.A)) // Assuming "A" key is used to confirm attack
            {
                // Apply damage to the selected target
                Unit selectedTarget = targets[selectedTargetIndex];
                ApplyDamage(selectedTarget, attacker);
                Debug.Log("EA SPORTS , IT'S IN THE GAME");
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.X)) // Assuming "A" key is used to confirm attack
            {
                EndAttackPhase();
                return false;


            }

        }
        return false;
    }

    public void EndAttackPhase()
    {
        // Reset the attacking state to false
        attacker.isAttacking = false;
        Gm.CurrentStateOfPlayer = EPlayerStates.Idle;
    }
    void Update()
    {
        // Check if the game is in attacking state before executing update logic
        if (Gm.CurrentStateOfPlayer == EPlayerStates.Attacking)
        {
            HandleTargetSelectionInput();
        }
    }


}