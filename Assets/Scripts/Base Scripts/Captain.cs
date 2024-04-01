using UnityEngine;

public class Captain : MonoBehaviour
{
    [SerializeField] int _passiveAttack;
    [SerializeField] int _passiveDefense;
    [SerializeField] int _celesteAttack;
    [SerializeField] int _celesteDefense;
    [SerializeField] Captains _name;

    public int PassiveAttack { get { return _passiveAttack; } }
    public int PassiveDefense { get { return _passiveDefense; } }
    public int CelesteAttack { get { return _celesteAttack; } }
    public int CelesteDefense { get { return _celesteDefense; } }
    public Captains Name { get => _name; }
}
