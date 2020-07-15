using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DualPantoFramework
{
    abstract public class ForceField : MonoBehaviour
    {
        public bool onUpper = true;
        public bool onLower = true;
        protected abstract Vector3 GetCurrentForce(Collider other);
        protected abstract float GetCurrentStrength();

        void OnTriggerStay(Collider other)
        {
            if (other.tag == "MeHandle" && onUpper)
            {
                GameObject.Find("Panto").GetComponent<UpperHandle>().ApplyForce(GetCurrentForce(other), GetCurrentStrength());
                Debug.DrawLine(other.transform.position, other.transform.position + GetCurrentForce(other) * GetCurrentStrength(), Color.red);
            }
            else if (other.tag == "ItHandle" && onLower)
            {
                GameObject.Find("Panto").GetComponent<LowerHandle>().ApplyForce(GetCurrentForce(other), GetCurrentStrength());
                Debug.DrawLine(other.transform.position, other.transform.position + GetCurrentForce(other) * GetCurrentStrength(), Color.red);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "MeHandle" && onUpper)
            {
                GameObject.Find("Panto").GetComponent<UpperHandle>().StopApplyingForce();
            }
            else if (other.tag == "ItHandle" && onLower)
            {
                GameObject.Find("Panto").GetComponent<LowerHandle>().StopApplyingForce();
            }
        }
    }
}
