using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;
using Task = System.Threading.Tasks.Task;
public class QuizGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    PantoCollider[] pantoColliders;
    void Start()
    {
        // Invoke("createObstacles", 1.0f);
    }
    
    private void OnEnable()
    {
        Room.OnPantoTriggerEnter += StartQuiz;
    }
    
    private void OnDisable()
    {
        Room.OnPantoTriggerEnter -= StartQuiz;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void StartQuiz(GameObject go, Collider other)
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
