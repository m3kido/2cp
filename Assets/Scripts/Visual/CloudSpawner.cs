using UnityEngine;
using UnityEngine.Tilemaps;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _cloudPrefabs; // Array to hold different cloud prefabs
    private Tilemap _map; // Reference to the tilemap

    private void Start()
    {
        _map = FindAnyObjectByType<MapManager>().Map;
        _map.RefreshAllTiles();

        // Start spawning clouds
        InvokeRepeating(nameof(SpawnCloud), 0.0f, 3.5f); // Adjust the third parameter to control spawn rate
    }

    public void SpawnCloud()
    {
        // Select a random cloud prefab from the array
        GameObject cloudPrefab = _cloudPrefabs[Random.Range(0, _cloudPrefabs.Length)];

        // Get the bounds of the tilemap and calculate the world positions of the boundaries
        BoundsInt bounds = _map.cellBounds;
        Vector3 mapMin = _map.CellToWorld(bounds.min);
        Vector3 mapMax = _map.CellToWorld(bounds.max);

        // Calculate the right edge of the tilemap
        float rightEdge = mapMax.x;

        // Calculate the vertical center and extent from the minimum and maximum Y positions
        float verticalCenter = (mapMin.y + mapMax.y) / 2;
        float verticalExtent = (mapMax.y - mapMin.y) / 2;

        Debug.Log($"Right Edge: {rightEdge}, Vertical Center: {verticalCenter}, Vertical Extent: {verticalExtent}");

        // Calculate a random Y position within the vertical bounds
        float randomY = Random.Range(verticalCenter - verticalExtent, verticalCenter + verticalExtent);

        // Calculate the spawn position with an offset to ensure clouds appear to the right of the map
        Vector3 spawnPosition = new(rightEdge + 3.0f, randomY, 0.0f);

        // Instantiate the cloud at the calculated position and set its parent to this transform
        Instantiate(cloudPrefab, spawnPosition, Quaternion.identity, transform);
    }
}
