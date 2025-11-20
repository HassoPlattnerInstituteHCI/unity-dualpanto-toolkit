using UnityEngine;
using DualPantoToolkit;
using System.Collections;
public class ObstacleManager : MonoBehaviour
{
    PantoCollider[] pantoColliders;
    public bool createAtStart = true;
    private IEnumerator coroutine;
    
    void Start()
    {
        // Invoke("createObstacles", 1.0f);
        coroutine = createObstacles(2.0f, createAtStart);
        StartCoroutine(coroutine);
    }

    // private void createObstacles(bool enableAtStart)
    // {
    //     pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
    //     foreach (PantoCollider collider in pantoColliders)
    //     {
    //         collider.CreateObstacle();
    //         collider.Enable();
    //     }
    // }

    private IEnumerator createObstacles(float waitTime, bool enableAtStart)
    {
        yield return new WaitForSeconds(waitTime);
        pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in pantoColliders)
        {
            collider.CreateObstacle();
            if (enableAtStart) {
            collider.Enable();
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
                Debug.Log("colliders enabled");
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
