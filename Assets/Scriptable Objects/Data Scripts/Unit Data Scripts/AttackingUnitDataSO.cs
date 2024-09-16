using UnityEngine;

// Subclass for attacking units
[CreateAssetMenu(fileName = "AttackingUnit", menuName = "Unit/Attacking Unit")]
public class AttackingUnitDataSO : UnitDataSO
{
    // We can assign values to these fields from the inspector
    [SerializeField] private int _minAttackRange;
    [SerializeField] private int _maxAttackRange;
    [SerializeField] private bool _hasTwoWeapons;
    [SerializeField] private int _maxEnergyOrbs;

    // Though, they are readonly for other classes
    // Declaring properties with getters only
    public int MinAttackRange => _minAttackRange;
    public int MaxAttackRange => _maxAttackRange;
    public bool HasTwoWeapons => _hasTwoWeapons;
    public int MaxEnergyOrbs => _maxEnergyOrbs;
}
