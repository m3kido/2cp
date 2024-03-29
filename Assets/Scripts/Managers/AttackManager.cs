using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{

    // Method to initiate an attack
    public void InitiateAttack(AttackingUnit attacker, Unit target)
    {
        
    }

    // Method to check if a unit can attack
    public bool UnitCanAttack(AttackingUnit attacker)
    {
        if (attacker == null) print("kizbi");
        return attacker.CanAttack(attacker); 
    }
    //till now hada li fih 
    //dok mbed ndir fih handling te3 Attack Action 

}