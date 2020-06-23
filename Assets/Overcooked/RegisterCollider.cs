using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RegisterCollider : MonoBehaviour
{
    async void Start()
    {
        await Task.Delay(2000);
        PantoBoxCollider collider = GetComponent<PantoBoxCollider>();
        collider.CreateObstacle();
        collider.Enable();
    }
}
