using UnityEngine;

public class Building
{
    public EBuildings BuildingType { get; set; }
    public Vector3Int Position { get; set; }
    public int Health { get; set; }
    public int Owner { get; set; }
    public Unit CapturingUnit { get; set; }

    // Building constructor
    public Building(EBuildings buildingType, Vector3Int position, int owner)
    {
        BuildingType = buildingType;
        Position = position;
        Owner = owner;
        Health = 200;
    }

    public BuildingSaveData GetDataToSave()
    {
        return new BuildingSaveData(Position, Health, Owner);
    }

    public void SetSaveData(BuildingSaveData data)
    {
        Health = data.Health;
        Owner = data.Owner;
    }
}