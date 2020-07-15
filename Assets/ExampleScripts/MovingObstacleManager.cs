using System.Threading.Tasks;
using UnityEngine;
using DualPantoFramework;
using System;

public class MovingObstacleManager : MonoBehaviour
{
    // moving a circular collider at 25Hz.
    // known issue: user can still get into the obstacle
    GameObject obstacle;
    Vector3 direction = new Vector3(0.1f, 0, 0);
    async void Start()
    {
        obstacle = GameObject.Find("Obstacle");
        PantoCollider collider = obstacle.GetComponent<PantoCollider>();
        await Task.Delay(1000);

        collider.CreateObstacle();
        collider.Enable();
        while (true)
        {
            await Task.Delay(50);
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
        PantoCollider oldCollider = obstacle.GetComponent<PantoCollider>();
        GameObject newObs = Instantiate(obstacle);
        Destroy(obstacle);
        obstacle = newObs;
        obstacle.transform.position = position;
        PantoCollider collider = obstacle.GetComponent<PantoCollider>();
        collider.CreateObstacle();

        // first enable the new collider before removing the old one to make sure the user is not accidentally getting into the obstacle
        collider.Enable();
        await Task.Delay(20);
        oldCollider.Remove();
        await Task.Delay(20);
    }
}
