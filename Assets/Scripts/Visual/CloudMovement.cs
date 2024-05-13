using UnityEngine;
using UnityEngine.Tilemaps;

// Script attached to every cloud
public class CloudMovement : MonoBehaviour
{
    private float _speed;
    private Tilemap _map; // Reference to the tilemap

    private void Start()
    {
        _map = FindAnyObjectByType<MapManager>().Map;
        _speed = Random.Range(0.35f, 0.50f);
    }

    private void Update()
    {
        // Move the cloud from right to left
        transform.Translate(_speed * Time.deltaTime * Vector3.left);

        // If the cloud moves off the left side of the tilemap, destroy it
        float leftEdge = _map.transform.position.x - (_map.size.x * _map.cellSize.x) / 2 - 3.0f;
        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}