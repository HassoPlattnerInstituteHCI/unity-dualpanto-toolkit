using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DualPantoToolkit;
using System;

public class PlayerRecoil : MonoBehaviour
{
    // Start is called before the first frame update

    private UpperHandle meHandle;

    private Queue<Vector3> lastPositions = new Queue<Vector3>(10);
   
    void Start()
    {
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    // Update is called once per frame
    async void Update()
    {
        UpdateLastPositions();
        await Task.Delay(10);
    }

    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Enemy")){
            ApplyRecoil();
        }
    }

    void UpdateLastPositions(){
        lastPositions.Enqueue(meHandle.GetPosition());
        if (lastPositions.Count > 10)
        {
            lastPositions.Dequeue();
        }
    }

    void ApplyRecoil(){
        if (lastPositions.Count == 0) return;
        
        // Durchschnittsposition der letzten Positionen berechnen
        Vector3 averagePosition = Vector3.zero;
        foreach (Vector3 position in lastPositions)
        {
            averagePosition += position;
        }
        averagePosition /= lastPositions.Count;
        
        // Aktuelle Position
        Vector3 currentPosition = meHandle.GetPosition();
        
        // Vektor von Durchschnittsposition zur aktuellen Position (Richtung Spieler)
        Vector3 directionToPlayer = currentPosition - averagePosition;
        
        // Y-Koordinate ignorieren (nur X und Z)
        directionToPlayer.y = 0;
        
        // Vektor normalisieren und umdrehen (entgegen der Bewegungsrichtung)
        Vector3 recoilDirection = -directionToPlayer.normalized;
        
        // Kraft anwenden
        meHandle.ApplyForce(currentPosition + recoilDirection, 10.0f);
    }
    
}
