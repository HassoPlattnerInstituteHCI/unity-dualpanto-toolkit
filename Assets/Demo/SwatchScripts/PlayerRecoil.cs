using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DualPantoToolkit;
using System;
using SpeechIO;

public class PlayerRecoil : MonoBehaviour
{
   
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float recoilStrength = 3f;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float recoilSpeed = 5f;

    private UpperHandle meHandle;
    public GameObject handleObject;
    private SpeechOut speech;


    void Start()
    {
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        speech = new SpeechOut();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            speech.Speak("Shot fired.");
            ApplyRecoil(); 
        }
    }

    
    // applies recoil to the player away from the collision point
    async void ApplyRecoil(){
       
        Vector3 currentPosition = meHandle.GetPosition();
        // calculate the direction of the recoil
        Vector3 recoilDirection = -((handleObject.transform.forward).normalized);    
        // apply the recoil to meHandle
        await meHandle.MoveToPosition(currentPosition + (recoilDirection * recoilStrength), recoilSpeed);
        
    }

    void OnApplicationQuit()
    {
        speech.Stop();
    }
    
}
