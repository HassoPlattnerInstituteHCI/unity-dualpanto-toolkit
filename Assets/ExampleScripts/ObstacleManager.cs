using UnityEngine;
using DualPantoFramework;
public class ObstacleManager : MonoBehaviour
{
    PantoCollider[] pantoColliders;
    void Start()
    {
        pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in pantoColliders)
        {
            Debug.Log("Enabling obstacle");
            collider.CreateObstacle();
            collider.Enable();
            //await Task.Delay(100);
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
