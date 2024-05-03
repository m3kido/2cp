using System.Collections.Generic;
using UnityEngine;

public abstract class UnitDataSO : ScriptableObject
{
    // We can assign values to these fields from the inspector
    [SerializeField] private EUnits _unitType;
    [SerializeField] private int _moveRange;
    [SerializeField] private int _maxProvisions;
    [SerializeField] private int _lineOfSight;
    [SerializeField] private int _cost;
    [SerializeField] private List<ETerrains> _walkableTerrains;

    // Though, they are readonly for other classes
    // Declaring properties with getters only
    public EUnits UnitType => _unitType;
    public int MoveRange => _moveRange;
    public int MaxProvisions => _maxProvisions;
    public int LineOfSight => _lineOfSight;
    public int Cost => _cost;
    public List<ETerrains> WalkableTerrains => _walkableTerrains;

    public bool IsWalkable(ETerrains terrain)
    {
        return _walkableTerrains.Contains(terrain);
    }
}
