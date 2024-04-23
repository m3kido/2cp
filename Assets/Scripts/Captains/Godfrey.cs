using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Godfrey : Captain
{
    public Godfrey(Player player): base(player)
    {
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Godfrey];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
        DefenseMultiplier += 0.05f;  
    }
    public override void EnableCeleste()
    {
        base.EnableCeleste();
        foreach (var unit in CaptainManager.Um.Units)
        {
            if (CaptainManager.Gm.Players[unit.Owner] != Player)
            {
                unit.MoveRange --;
                //REDUCE PROVISION
            }
        }
    }

    public override void DisableCeleste()
    {
        base.DisableCeleste();
        foreach (var unit in CaptainManager.Um.Units)
        {
            if (CaptainManager.Gm.Players[unit.Owner] != Player)
            {
                unit.MoveRange = unit.Data.MoveRange ;
                
            }
        }
    }
    public override bool IsCelesteReady()
    {
        return base.IsCelesteReady();
    }
}
