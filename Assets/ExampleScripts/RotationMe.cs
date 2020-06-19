using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMe : MonoBehaviour
{
    UpperHandle upperHandle;
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, upperHandle.GetRotation(), 0);
    }
}
