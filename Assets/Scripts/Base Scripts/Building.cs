using UnityEngine;

// We have to discuss this 
public class Building
{
    // Auto-properties (the compiler automatically creates private fields for them)
    public EBuildings BuildingType { get; set; }
    public Vector3Int Position { get; set; }
    public int Health { get; set; }
    public int Owner { get; set; }

    // Building constructor
    public Building(EBuildings buildingType, Vector3Int position, int owner)
    {
        BuildingType = buildingType;
        Position = position;
        Owner = owner;
        Health = 200;
    }
}

