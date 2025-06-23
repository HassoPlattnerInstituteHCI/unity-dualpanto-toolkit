using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;

public class Solid : MonoBehaviour
{
    PantoCollider[] pantoColliders;
    // Start is called before the first frame update
    void Start()
    {
        pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MeHandle" || other.tag == "ItHandle")
        {
            foreach (PantoCollider collider in pantoColliders)
            {
                collider.Enable();
            }
        }
    }
}
