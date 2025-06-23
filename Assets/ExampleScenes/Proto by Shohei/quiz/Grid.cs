using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;

public class Grid : MonoBehaviour
{
    // Start is called before the first frame update
    private bool initialEnterFlag = false;
    public bool activateMe = false;
    public bool activateIt = true;
    
    public bool shouldFreeHandle;
    public float speed = 20f;
    PantoHandle handle;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MeHandle" || other.tag == "ItHandle")
        {
            initialEnterFlag = true;
            Debug.Log("Enter");
        }
        GameObject.Find("Panto").GetComponent<LowerHandle>().Free();
            

    }

    async void OnTriggerExit(Collider other)
    {
        if (initialEnterFlag)
        {
            if (other.tag == "MeHandle" && activateMe)
            {
                GameObject.Find("Panto").GetComponent<UpperHandle>().Freeze();
                
                await GameObject.Find("Panto").GetComponent<UpperHandle>().MoveToPosition(transform.position, speed, shouldFreeHandle);
            }
            else if (other.tag == "ItHandle" && activateIt)
            {
                GameObject.Find("Panto").GetComponent<LowerHandle>().Freeze();
    
                //await GameObject.Find("Panto").GetComponent<LowerHandle>().MoveToPosition(transform.position, speed, shouldFreeHandle);
            }
        }
    }
}
