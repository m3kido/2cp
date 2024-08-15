using UnityEngine;

[CreateAssetMenu(menuName = "Captain", order = 0)]
public class CaptainDataSO : ScriptableObject
{
    public ECaptains Name;
    public int PassiveAttack;
    public int PassiveDefense;
    public int CelesteAttack;
    public int CelesteDefense;
}
