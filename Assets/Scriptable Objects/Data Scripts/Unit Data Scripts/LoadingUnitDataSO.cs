using UnityEngine;
using System.Collections.Generic;

// Subclass for loading units
[CreateAssetMenu(fileName = "LoadingUnit", menuName = "Unit/Loading Unit")]
public class LoadingUnitDataSO : UnitDataSO
{
    // We can assign values to these fields from the inspector
    [SerializeField] private int _numberOfLoadableUnits;
    [SerializeField] private List<EUnits> _loadableUnits;

    // Though, they are readonly for other classes
    // Declaring properties with getters only
    public int NumberOfLoadableUnits => _numberOfLoadableUnits;
    public List<EUnits> LoadableUnits => new List<EUnits>(_loadableUnits);
}