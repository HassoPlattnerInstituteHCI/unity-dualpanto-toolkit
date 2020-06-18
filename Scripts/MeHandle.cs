using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeHandle : MonoBehaviour
{
    PantoHandle upperHandle;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    void FixedUpdate()
    {
        rb.MovePosition(upperHandle.HandlePosition(transform.position));
        //transform.position = (upperHandle.HandlePosition(transform.position));
    }
}
