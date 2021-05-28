using UnityEngine;
using DualPantoFramework;
public class ObstacleManager : MonoBehaviour
{
    GameObject perceptionCone;
    PantoCollider[] pantoColliders;
    void Start()
    {
        pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in pantoColliders)
        {
            collider.CreateObstacle();
            collider.Enable();
        }
        foreach (GameObject r in GameObject.FindGameObjectsWithTag("Rail"))
        {
            RailPolyline rail = r.GetComponent<RailPolyline>();
            rail.CreateObstacles();
        }
        AddPerceptionCone();
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

    void AddPerceptionCone()
    {
        GameObject parent = GameObject.FindGameObjectWithTag("MeHandle");
        UnityEngine.Object cone = Resources.Load("PerceptionCone");
        perceptionCone = Instantiate(cone, parent.transform) as GameObject;
        perceptionCone.name = "MeHandlePerceptionCone";
    }

}
