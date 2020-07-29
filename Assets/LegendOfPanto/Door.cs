using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Link")
        {
            other.GetComponent<Link>().EnterDoor();
        }
    }
}
