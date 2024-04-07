using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Godfrey : Captain
{
    public Godfrey(Player player): base(player)
    {
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Godfrey];
    }
}
