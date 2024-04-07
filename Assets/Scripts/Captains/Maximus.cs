using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maximus : Captain
{
    public Maximus(Player player) : base(player)
    {
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Maximus];
    }
}
