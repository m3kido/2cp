using UnityEngine;

// Attacking unit scriptable object
[CreateAssetMenu(fileName = "AttackingUnit", menuName = "Unit/Attacking Unit")]
public class AttackingUnitDataSO : UnitDataSO
{
    [SerializeField] private int _minAttackRange;
    [SerializeField] private int _maxAttackRange;
    [SerializeField] private bool _hasTwoWeapons;
    [SerializeField] private int _maxEnergyOrbs;

    public int MinAttackRange => _minAttackRange;
    public int MaxAttackRange => _maxAttackRange;
    public bool HasTwoWeapons => _hasTwoWeapons;
    public int MaxEnergyOrbs => _maxEnergyOrbs;
}
