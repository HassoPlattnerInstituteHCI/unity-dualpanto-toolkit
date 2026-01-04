using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DualPantoToolkit;
using System;

public class PlayerRecoil : MonoBehaviour
{
   
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float recoilStrength = 3f;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float recoilSpeed = 5f;

    private UpperHandle meHandle;

    void Start()
    {
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    // checks for collision with enemy and applies recoil
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var collisionPoint = collision.contacts[0].normal;
            ApplyRecoil(collisionPoint);
        }
    }
    
    // applies recoil to the player away from the collision point
    async void ApplyRecoil(Vector3 collisionPoint){
       
        Vector3 currentPosition = meHandle.GetPosition();
        // calculate the direction of the recoil
        Vector3 recoilDirection = (collisionPoint.normalized - currentPosition.normalized).normalized;    
        // apply the recoil to meHandle
        await meHandle.MoveToPosition(currentPosition + (recoilDirection * recoilStrength), recoilSpeed);
        
    }
    
}
