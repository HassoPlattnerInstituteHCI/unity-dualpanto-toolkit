using UnityEngine;
using DualPantoToolkit;
public class ObstacleManager : MonoBehaviour
{
    PantoCollider[] pantoColliders;
    public bool createAtStart = true;
    void Start()
    {
        if (createAtStart) {
        Invoke("createObstacles", 1.0f);
        }
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
