using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;
public class PacManManager : MonoBehaviour
{
    // Start is called before the first frame update
    PantoCollider[] pantoColliders;
    void Start()
    {
        
    }
    
    private void OnEnable()
    {
        Room.OnPantoTriggerEnter += StartGame;
    }
    
    private void OnDisable()
    {
        Room.OnPantoTriggerEnter -= StartGame;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void StartGame(GameObject go, Collider other)
    {
        EnablePantoCollider();
    }

    
    private void EnablePantoCollider()
    {
        pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in pantoColliders)
        {
            collider.CreateObstacle();
            collider.Enable();
        }
    }
}
