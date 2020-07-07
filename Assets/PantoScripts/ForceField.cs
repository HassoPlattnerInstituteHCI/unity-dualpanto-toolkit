using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "MeHandle")
        {
            GameObject.Find("Panto").GetComponent<UpperHandle>().ApplyForce(new Vector3(1, 0, 0), 0.3f);
        }
        else if (other.tag == "ItHandle")
        {
            GameObject.Find("Panto").GetComponent<LowerHandle>().ApplyForce(new Vector3(1, 0, 0), 0.3f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "MeHandle")
        {
            GameObject.Find("Panto").GetComponent<UpperHandle>().StopApplyingForce();
        }
        else if (other.tag == "ItHandle")
        {
            GameObject.Find("Panto").GetComponent<LowerHandle>().StopApplyingForce();
        }
    }
}
