using UnityEngine;
using DualPantoToolkit;
public class ObstacleManager : MonoBehaviour
{
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
            collider.Enable();
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
