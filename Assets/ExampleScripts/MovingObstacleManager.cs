using System.Threading.Tasks;
using UnityEngine;
using DualPantoFramework;
using System;

public class MovingObstacleManager : MonoBehaviour
{
    // moving a circular collider at 25Hz.
    // known issue: user can still get into the obstacle
    GameObject obstacle;
    PantoCollider collider;
    PantoCollider oldCollider;
    Vector3 direction = new Vector3(0.1f, 0, 0);
    async void Start()
    {
        // if we register obstacles too early, the device will not work any longer (only sync debug logs will be printed
        // I am working on fixing this, but for now just add a wait
        obstacle = GameObject.Find("Obstacle");
        collider = obstacle.GetComponent<PantoCollider>();
        collider.onUpper = false;
        await Task.Delay(1000);
        
        collider.CreateObstacle();
        collider.Enable();
        int moves = 1000;
        for (int i = 0; i < moves;i++) {
            Vector3 newPos = obstacle.transform.position + direction;
            if (Math.Abs(obstacle.transform.position.x) <= 5 && Math.Abs(newPos.x) > 5)
            {
                direction = direction * -1;
            }
            await MoveObstacle(newPos);
        }
    }

    async Task MoveObstacle(Vector3 position)
	{
        oldCollider = obstacle.GetComponent<PantoCollider>();
        GameObject newObs = Instantiate(obstacle);
        Destroy(obstacle);
        obstacle = newObs;
        obstacle.transform.position = position;
        collider = obstacle.GetComponent<PantoCollider>();
        collider.CreateObstacle();

        // first enable the new collider before removing the old one to make sure the user is not accidentally getting into the obstacle
        collider.Enable();
        await Task.Delay(20);
        oldCollider.Remove();
        await Task.Delay(20);

    }
}
