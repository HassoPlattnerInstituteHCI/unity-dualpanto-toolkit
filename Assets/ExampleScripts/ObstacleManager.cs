using System.Threading.Tasks;
using UnityEngine;
using DualPantoFramework;

public class ObstacleManager : MonoBehaviour
{
    async void Start()
    {
        // if we register obstacles too early, the device will not work any longer (only sync debug logs will be printed
        // I am working on fixing this, but for now just add a wait
        await Task.Delay(1000);
        PantoCollider[] pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in pantoColliders)
        {
            collider.CreateObstacle();
            collider.Enable();
        }
    }
}
