using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;

public class MeToIt : MonoBehaviour
{
    PantoHandle upperHandle;
    PantoHandle lowerHandle;
    public bool shouldFreeHandle;
    public float speed = 10f;
    bool inTransition = false;
    
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            inTransition = true;
        }

        if (inTransition) {
            // transform.position = lowerHandle.GetPosition();
            await upperHandle.MoveToPosition(lowerHandle.GetPosition(), speed, shouldFreeHandle);
            inTransition = false;
            Debug.Log("upper handle position: " + upperHandle.GetPosition().ToString());
        }
    }
}
