using UnityEngine;
using DualPantoToolkit;
using System.Collections;
public class ObstacleManager : MonoBehaviour
{
    PantoCollider[] pantoColliders;
    public bool createAtStart = true;
    private IEnumerator coroutine;
    bool collidersEnabled = false;
    
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
            collider.Enable();
            collidersEnabled = true;
            if (!createAtStart) {
                collider.Disable();
                collidersEnabled = false;
            }
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !collidersEnabled)
        {
            foreach (PantoCollider collider in pantoColliders)
            {
                collider.Enable();
                collidersEnabled = true;
                Debug.Log("colliders enabled");
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) && collidersEnabled)
        {
            foreach (PantoCollider collider in pantoColliders)
            {
                collider.Disable();
                collidersEnabled = false;
            }
        }
    }
}
