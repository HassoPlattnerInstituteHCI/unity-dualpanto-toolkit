using System.Threading.Tasks;
using UnityEngine;
using DualPantoFramework;
using System;

public class MovingObstacleManager : MonoBehaviour
{
    // moving a circular collider at 25Hz.
    // known issue: user can still get into the obstacle
    GameObject obstacle;
    Vector3 direction = new Vector3(1f, 0, 0);
    PantoCollider pantoCollider;
    async void Start()
    {
        obstacle = GameObject.Find("Obstacle");
        pantoCollider = obstacle.GetComponent<PantoCollider>();
        await Task.Delay(1000);

        pantoCollider.CreateObstacle();
        pantoCollider.Enable();
        while (true)
        {
            await Task.Delay(4000);
            Vector3 newPos = obstacle.transform.position + direction;
            if (Math.Abs(obstacle.transform.position.x) <= 5 && Math.Abs(newPos.x) > 5)
            {
                direction = direction * -1;
            }
            MoveObstacle(newPos);
        }
    }

    void MoveObstacle(Vector3 position)
    {
        pantoCollider.Remove();
        obstacle.transform.position = position;
        pantoCollider.CreateObstacle();
        pantoCollider.Enable();
    }
}
