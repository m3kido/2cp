using System;
using UnityEngine;

[Serializable]
public class CaptainData
{
    public int passiveAttack;
    public int passiveDefense;
    public int celesteAttack;
    public int celesteDefense;
    public Captains name;
}

public enum Captains
{
    Andrew,
    Godfrey,
    Maximus,
    Melina
}

public class Captain : MonoBehaviour
{
    [SerializeField] private CaptainData captainData;

    public int PassiveAttack { get { return captainData.passiveAttack; } }
    public int PassiveDefense { get { return captainData.passiveDefense; } }
    public int CelesteAttack { get { return captainData.celesteAttack; } }
    public int CelesteDefense { get { return captainData.celesteDefense; } }
    public Captains Name { get { return captainData.name; } }
}
