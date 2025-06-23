using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;

public class MovingWall : MonoBehaviour
{
    PantoHandle itHandle;
    // Start is called before the first frame update
    async void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 
    }
    async void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ItHandle")
        {
            itHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
            await itHandle.MovingWall(gameObject, 20f);
            
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ItHandle")
        {
            GameObject.Find("Panto").GetComponent<LowerHandle>().Free();
        }
    }
}
