using UnityEngine;
using DualPantoToolkit;

namespace DualPantoToolkit
{
    /// <summary>
    /// Creates and enables all PantoColliders in the scene. 
    /// Attach this script on an empty GameObject in the scene. It will find all PantoColliders in the scene and create obstacles for them. 
    /// You can also enable or disable all obstacles at runtime by pressing the E or D keys.
    /// </summary>
    public class ObstacleManager : MonoBehaviour
    {
        public bool enableObstaclesOnStart = true;
        PantoCollider[] pantoColliders;

        void Start()
        {
            Invoke("createObstacles", 1.0f);
        }

        private void createObstacles()
        {
            pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in pantoColliders)
            {
                collider.CreateObstacle();
                if (enableObstaclesOnStart)
                {
                    collider.Enable();
                }
                else
                {
                    collider.Disable();
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (PantoCollider collider in pantoColliders)
                {
                    collider.Enable();
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                foreach (PantoCollider collider in pantoColliders)
                {
                    collider.Disable();
                }
            }
        }
    }
}
