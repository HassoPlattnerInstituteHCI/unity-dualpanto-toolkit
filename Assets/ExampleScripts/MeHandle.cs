using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeHandle : MonoBehaviour
{
    PantoHandle upperHandle;
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    void FixedUpdate()
    {
        transform.position = (upperHandle.HandlePosition(transform.position));
    }
}
