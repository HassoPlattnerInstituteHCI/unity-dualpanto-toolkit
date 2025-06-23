using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DualPantoToolkit
{
    public class ObstacleDetector : MonoBehaviour
    {
        public static bool isObstacleActive = true;
        void OnTriggerEnter(Collider collider)
        {
            
            PantoCollider pc = collider.GetComponent<PantoCollider>();
            if (pc != null)
            {
                Debug.Log("wwww");
                isObstacleActive = false;
            }
        }

        void OnTriggerExit(Collider collider)
        {
            PantoCollider pc = collider.GetComponent<PantoCollider>();
            if (pc != null)
            {
                isObstacleActive = true;
            }
        }
    }
}
