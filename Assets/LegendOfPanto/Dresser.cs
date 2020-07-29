using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dresser : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.tag == "Link")
        {
            other.GetComponent<Link>().GetDressed();
        }
    }
}
