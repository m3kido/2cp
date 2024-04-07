using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andrew : Captain
{
    
    public Andrew(Player player) : base(player)
    {
        
        Player = player; 
        Data = CaptainManager.CaptainsDict[ECaptains.Andrew];
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
