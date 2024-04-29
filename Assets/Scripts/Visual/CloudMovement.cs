using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 0.3f;

    void Update()
    {
        // Move the cloud from right to left
        transform.Translate(_speed * Time.deltaTime * Vector3.left);

        // If the cloud moves off the left side of the screen, destroy it
        if (transform.position.x < -10.0f)
        {
            Destroy(gameObject);
        }
    }
}