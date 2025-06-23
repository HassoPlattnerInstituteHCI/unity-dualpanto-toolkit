using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 direction = Vector3.right;
    bool movementStarted = true;
    public float speed = 0.05f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!movementStarted) return;
        if (transform.position.x > 4 || transform.position.x < -4)
        {
            direction *= -1;
        }
        transform.position += direction * speed;
    }
}
