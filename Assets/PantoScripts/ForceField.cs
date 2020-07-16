using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DualPantoFramework
{
    abstract public class ForceField : MonoBehaviour
    {
        public bool onUpper = true;
        public bool onLower = true;
        UpperHandle upperHandle;
        LowerHandle lowerHandle;
        protected abstract Vector3 GetCurrentForce(Collider other);
        protected abstract float GetCurrentStrength(Collider other);

        void Start()
        {
            upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            //lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        }

        void OnTriggerStay(Collider other)
        {
            Debug.Log(other);
            //if (onUpper && other.GetComponent<UpperHandle>() != null)
            if (onUpper && other.tag == "MeHandle")
            {
                //Debug.Log(GetCu);
                //other.GetComponent<UpperHandle>().ApplyForce(GetCurrentForce(other), GetCurrentStrength(other));
                upperHandle.ApplyForce(GetCurrentForce(other), GetCurrentStrength(other));
                Debug.DrawLine(other.transform.position, other.transform.position + GetCurrentForce(other) * GetCurrentStrength(other), Color.red);
            }
            else if (onLower)
            {
                lowerHandle.ApplyForce(GetCurrentForce(other), GetCurrentStrength(other));
                Debug.DrawLine(other.transform.position, other.transform.position + GetCurrentForce(other) * GetCurrentStrength(other), Color.red);
            }
        }

        void OnTriggerExit(Collider other)
        {
            //if (onUpper && other.GetComponent<UpperHandle>())
            if (onUpper && other.tag == "MeHandle")
            {
                upperHandle.StopApplyingForce();
            }
            else if (onLower)
            {
                GameObject.Find("Panto").GetComponent<LowerHandle>().StopApplyingForce();
            }
        }
    }
}
