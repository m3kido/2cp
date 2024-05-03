using UnityEngine;
using UnityEngine.Tilemaps;

// Scriptable object to get terrain datas
[CreateAssetMenu(fileName = "Terrain", menuName = "Terrain")]
public class TerrainDataSO : ScriptableObject
{
    // We can assign values to these fields from the inspector
    [SerializeField] private ETerrains _terrainType;
    [SerializeField] private Tile[] _terrainsOfSameType;
    [SerializeField] private int _provisionsCost;
    [SerializeField] private int _defenceStars;

    // Though, they are readonly for other classes
    // Declaring properties with getters only
    public ETerrains TerrainType => _terrainType;
    public Tile[] TerrainsOfSameType => _terrainsOfSameType;
    public int ProvisionsCost => _provisionsCost;
    public int DefenceStars => _defenceStars;
}
