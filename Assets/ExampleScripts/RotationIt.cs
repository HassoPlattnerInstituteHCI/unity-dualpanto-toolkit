using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationIt : MonoBehaviour
{
    LowerHandle lowerHandle;
    async void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        await lowerHandle.SwitchTo(gameObject, 0.2f);
    }

    void FixedUpdate()
    {
       gameObject.transform.RotateAround(gameObject.transform.position, Vector3.up, 0.5f);
    }
}
