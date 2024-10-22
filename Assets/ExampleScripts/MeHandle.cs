﻿using UnityEngine;
using DualPantoToolkit;

public class MeHandle : MonoBehaviour
{
    bool free = true;
    PantoHandle upperHandle;
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    void FixedUpdate()
    {
        transform.position = (upperHandle.HandlePosition(transform.position));
        transform.eulerAngles = new Vector3(0, upperHandle.GetRotation(), 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (free)
            {
                upperHandle.Freeze();
            }
            else
            {
                upperHandle.Free();
            }
            free = !free;
        }
    }
}
