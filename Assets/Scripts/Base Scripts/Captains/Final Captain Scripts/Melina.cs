using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melina : Captain
{
    private int selectedUnitIndex = -1;
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

    List<Unit> _tiredUnits = new();
    public Melina(Player player) : base(player)
    {
        CaptainName = ECaptains.Melina;

        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Melina];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
        PriceMultiplier -= 0.1f;
        SuperMeter = 40000;
        maxSuperMeter = 40000;
    }

    public override void EnableCeleste()
    {
        if (!IsCelesteReady())
            return;

        base.EnableCeleste();
        SuperMeter -= maxSuperMeter;
        foreach (var unit in CaptainManager.Um.Units)
        {
            if (CaptainManager.Gm.Players[unit.Owner] == Player)
            {
                UnityEngine.Debug.Log("zebi");
                if (unit is AttackingUnit)
                {
                    var attacker = unit as AttackingUnit;
                    foreach(var weapon in attacker.Weapons)
                    {
                        weapon.EnergyOrbs += 2;
                        CaptainManager.Instance.EnergySpr(unit); 

                    }
                    attacker.CurrentWeaponIndex = 0;
                }
                if (unit.HasMoved)
                {
                    UnityEngine.Debug.Log(unit.HasMoved.ToString());
                    _tiredUnits.Add(unit);
                }
            }

        }
        if (_tiredUnits.Count > 0)
        {
            selectedUnitIndex = 0;
        }
        //Starting a coroutine via the captin manager ; 

        Debug.Log("Melina");
    }

    public override void DisableCeleste()
    {
        base.DisableCeleste();

    }



    public IEnumerator ReviveSelectionCoroutine(GameManager gm)
    {
        yield return null;// skip 1 frame so the space clicked to confirm the attack option 
                          // selection doesn't confirm the target selection instantly 
        gm.CurrentStateOfPlayer = EPlayerStates.SuperPower;
        UnityEngine.Debug.Log("Couroutine started ");
        while (!ActionTaken)
        {
            HandleInput();

            // Wait for the next frame
            yield return null;
        }
        ActionTaken = false;
        gm.CurrentStateOfPlayer = EPlayerStates.Idle;
        Debug.Log("Action finished");
        ActionTaken = true;
    }

    private void HandleInput()
    {

        // Check if there are any targets to select
        if (selectedUnitIndex == -1)
        {
            Debug.LogWarning("No units available for selection.");
            ActionTaken = true;
            return;
        }

        if (_tiredUnits.Count == 1)
        {
            HighlightSelectedTarget(_tiredUnits[0], Color.green);
        }
        else
        {
            HighlightSelectedTarget(_tiredUnits[selectedUnitIndex], Color.green);

            // Handle navigation keys
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Move to the next target (circular)
                HighlightSelectedTarget(_tiredUnits[selectedUnitIndex], Color.grey);
                selectedUnitIndex = (selectedUnitIndex + 1) % _tiredUnits.Count;
                HighlightSelectedTarget(_tiredUnits[selectedUnitIndex], Color.green);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                HighlightSelectedTarget(_tiredUnits[selectedUnitIndex], Color.grey);
                selectedUnitIndex = (selectedUnitIndex - 1 + _tiredUnits.Count) % _tiredUnits.Count;
                HighlightSelectedTarget(_tiredUnits[selectedUnitIndex], Color.green);
            }
        }

        // Handle attack confirmation and cancellation
        if (Input.GetKeyDown(KeyCode.Space)) // Assuming "Space" key is used to confirm attack
        {
            // Apply damage to the selected target
            Unit selectedTarget = _tiredUnits[selectedUnitIndex];
            if (selectedTarget != null)
            {
                HighlightSelectedTarget(_tiredUnits[selectedUnitIndex], Color.white);
                selectedTarget.HasMoved = false;
            }
            ActionTaken = true;

        }

        if (Input.GetKeyDown(KeyCode.X)) // Assuming "X" key is used to cancel attack
        {
            HighlightSelectedTarget(_tiredUnits[selectedUnitIndex], Color.gray);
            
            ActionTaken = true;

        }
    }
    private void HighlightSelectedTarget(Unit unit, Color color)
    {

        // Try to get the Renderer component of the target's GameObject
        unit._rend.color = color;
    }


}


