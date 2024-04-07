using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melina : Captain
{
    public Melina(Player player) : base(player)
    {
        Player = player;
        Data = CaptainManager.CaptainsDict[ECaptains.Melina];
    }
}
