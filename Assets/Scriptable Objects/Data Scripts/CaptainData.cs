using UnityEngine;

[CreateAssetMenu(menuName = "Captain Data", order = 0)]
public class CaptainData : ScriptableObject
{
    public int PassiveAttack;
    public int PassiveDefense;
    public int CelesteAttack;
    public int CelesteDefense;
    public ECaptains Name;
}
