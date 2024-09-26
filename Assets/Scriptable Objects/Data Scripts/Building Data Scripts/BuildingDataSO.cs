using UnityEngine;
using UnityEngine.Tilemaps;

// Scriptable object to get building datas
public abstract class BuildingDataSO : ScriptableObject
{
    // We can assign values to these fields from the inspector
    [SerializeField] private EBuildings _buildingType;
    [SerializeField] private ETeamColors _color;
    [SerializeField] private Tile _buildingTile;

    // Though, they are readonly for other classes
    // That's why we're declaring public properties with getters only
    public EBuildings BuildingType => _buildingType;
    public ETeamColors Color => _color;
    public Tile BuildingTile => _buildingTile;
}
