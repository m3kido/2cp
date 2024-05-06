using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effectUp : MonoBehaviour
{
    public float speed=1;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
    }
}
