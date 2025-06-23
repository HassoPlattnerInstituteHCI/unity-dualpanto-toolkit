using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using DualPantoToolkit;
using Task = System.Threading.Tasks.Task;

public class TrashCompactorManager : MonoBehaviour
{
    SpeechOut speechOut = new SpeechOut();
    // Start is called before the first frame update
    public GameObject room;
    public GameObject switchObj;
    public GameObject movingWall;
    public GameObject followingWall;
    private bool pMoving = false;
    private bool movingWallRendered = false;
    private bool wallRendered = false;
    
    PantoCollider[] pantoColliders;
    
    Vector3 direction = Vector3.right;
    bool movementStarted = true;
    public float speed = 0.001f;
    
        
    void Start()
    {
        pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
        Introduction();
    }

    async void Introduction()
    {
        await speechOut.Speak("you are in 1977 movie ’star wars’, in the trash compactor room");
        Level level = GetComponent<Level>();
        await level.PlayIntroduction(0.2f, 3000);
        await Task.Delay(1000);
    }

    // Update is called once per frame
    void Update()
    {
        if(pMoving)  MovingWall();
    }
    
    private void OnEnable()
    {
        Room.OnPantoTriggerEnter += StartCompacting;
    }

    private void OnDisable()
    {
        Room.OnPantoTriggerEnter -= StartCompacting;
    }

    void StartCompacting(GameObject go, Collider other)
    {
        EnablePantoCollider();
        if (!movingWallRendered)
        {
            Instantiate(movingWall);
            movingWallRendered = true;
        }
       
        pMoving = true;
    }

    void MovingWall()
    {
        if (movingWall.transform.position.x > 4 || movingWall.transform.position.x < -4)
        {
            direction *= 0;
            pMoving = false;
            if (!wallRendered)
            {
                Instantiate(followingWall);
                EnablePantoCollider();
                wallRendered = true;
            }

        }
        movingWall.transform.position += direction * speed;
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
    
    void OnApplicationQuit()
    {
        speechOut.Stop();
    }

}
