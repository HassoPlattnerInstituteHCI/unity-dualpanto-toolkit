using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DualPantoToolkit
{
    /// <summary>
    /// Applies a linear force on any object with a "MeHandle" or "ItHandle" tag within its area.
    /// </summary>
    public class DirectionalForceField : MonoBehaviour
    {

        public float strength = 1;
        public bool onUpper = true;
        public bool onLower = true;
        UpperHandle upperHandle;
        LowerHandle lowerHandle;
        
        Vector3 GetDirectionalForceVector(Transform otherTransform)
        {
            // Vector from the trigger object to the player
            Vector3 directionToPlayer = (otherTransform.position - transform.position).normalized;

            // Get forward direction of the trigger object
            Vector3 forward = transform.forward;

            // Calculate the angle between forward and the directionToPlayer
            float angle = Vector3.SignedAngle(forward, directionToPlayer, Vector3.up);

            if (angle > -45 && angle < 45)
            {
                return Vector3.up;
                Debug.Log("Player entered from the front");
            }
            else if (angle >= 45 && angle <= 135)
            {
                return Vector3.left;
                Debug.Log("Player entered from the right");
            }
            else if (angle < -45 && angle > -135)
            {
                return Vector3.right;
                Debug.Log("Player entered from the left");
            }
            else
            {
                return Vector3.down;
                Debug.Log("Player entered from the back");
            }
        }

        void OnTriggerEnter(Collider other)
        {

            if (other.tag == "MeHandle" && onUpper)
            {
                GameObject.Find("Panto").GetComponent<UpperHandle>().ApplyForce(GetDirectionalForceVector(other.transform), strength);
                //Debug.DrawLine(other.transform.position, other.transform.position + GetCurrentForce(other) * GetCurrentStrength(other), Color.red);
            }
            else if (other.tag == "ItHandle" && onLower)
            {
                GameObject.Find("Panto").GetComponent<LowerHandle>().ApplyForce(GetDirectionalForceVector(other.transform), strength);
                //Debug.DrawLine(other.transform.position, other.transform.position + GetCurrentForce(other) * GetCurrentStrength(other), Color.red);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "MeHandle" && onUpper)
            {
                //GameObject.Find("Panto").GetComponent<UpperHandle>().StopApplyingForce();
                GameObject.Find("Panto").GetComponent<UpperHandle>().Free();
            }
            else if (other.tag == "ItHandle" && onLower)
            {
                //GameObject.Find("Panto").GetComponent<LowerHandle>().StopApplyingForce();
                GameObject.Find("Panto").GetComponent<LowerHandle>().Free();
            }
        }
    }
}