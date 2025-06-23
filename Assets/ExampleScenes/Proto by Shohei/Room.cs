using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;

public class Room : MonoBehaviour
{
    PantoCollider[] pantoColliders;
    public static event Action<GameObject, Collider> OnPantoTriggerEnter;
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
            OnPantoTriggerEnter?.Invoke(gameObject, other);
        }
    }
}
