using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melina : Captain
{
    public Melina(Player player) : base(player)
    {
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Melina];
        PassiveDefense = Data.PassiveDefense;
        PassiveAttack = Data.PassiveAttack;
    }
    public override void EnableCeleste()
    {
        base.EnableCeleste();
    }

    public override void DisableCeleste()
    {
        base.DisableCeleste();
    }
    public override bool IsCelesteReady()
    {
        return base.IsCelesteReady();
    }
}
