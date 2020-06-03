using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PantoHandle upperHandle;
    void Start()
    {
       upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    void Update()
    {
        transform.position = upperHandle.getPosition();
    }
}
