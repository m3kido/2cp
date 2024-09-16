using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _cloudPrefabs; // Array to hold different cloud prefabs

    void Start()
    {
        // Start spawning clouds
        InvokeRepeating(nameof(SpawnCloud), 5.0f, 3.0f); // Adjust the second parameter to control spawn rate
    }

    void SpawnCloud()
    {
        // Randomly select a cloud prefab from the array
        GameObject cloudPrefab = _cloudPrefabs[Random.Range(0, _cloudPrefabs.Length)];

        // Calculate vertical bounds based on screen aspect ratio (16:9)
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Camera.main.aspect;
        float verticalOffset = screenHeight / 2;

        // Instantiate the selected cloud prefab at a random position on the right side of the screen
        float randomY = Random.Range(-verticalOffset, verticalOffset);
        Vector3 spawnPosition = new(screenWidth / 2, randomY, 0.0f);
        GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);

        // Set the cloud's parent to the spawner for organization
        cloud.transform.parent = transform;
    }

}
