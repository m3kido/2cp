using System.Collections.Generic;
using UnityEngine;

// Subclass for unit spawning buildings
[CreateAssetMenu(fileName = "SpawnerBuilding", menuName = "Building/Spawner Building")]
public class SpawnerBuildingDataSO : BuildingDataSO
{
    // Can't be captured
    // Doesn't provide gold
    // Can Spawn units

    // We can assign values to this field from the inspector

    // List of units that can be deployed from this building
    [SerializeField] private List<EUnits> _deployableUnits;

    // Though, it is readonly for other classes
    // That's why we're declaring a public property with a getter only
    public List<EUnits> DeployableUnits => _deployableUnits;
}
