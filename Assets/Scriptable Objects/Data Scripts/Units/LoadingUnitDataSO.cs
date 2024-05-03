using UnityEngine;
using System.Collections.Generic;

// Loading Units
[CreateAssetMenu(fileName = "LoadingUnit", menuName = "Unit/Loading Unit")]
public class LoadingUnitDataSO : UnitDataSO
{
    // We can assign values to these fields from the inspector
    
    [SerializeField] private List<EUnits> _loadableUnits;

    // Though, they are readonly for other classes
    // Declaring properties with getters only
    
    public List<EUnits> LoadableUnits => new List<EUnits>(_loadableUnits);
}