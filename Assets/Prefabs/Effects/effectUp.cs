using UnityEngine;

// Script to make an effect icon go up
public class EffectUp : MonoBehaviour
{
    public float speed = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
    }
}
