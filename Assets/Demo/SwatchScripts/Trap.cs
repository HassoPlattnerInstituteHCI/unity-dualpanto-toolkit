using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using DualPantoToolkit;

public class Trap : MonoBehaviour
{
    PantoHandle upperHandle;
    bool handleFree = true;
    public bool onUpper = true;
    bool inTrap = false;
    SpeechOut speech;
    // Start is called before the first frame update
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        if (upperHandle == null)
        {
            Debug.LogError("[DualPanto] UpperHandle not found on Panto GameObject!");
        }
        
        speech = new SpeechOut();
    }

    private async void OnTriggerEnter(Collider other) {
        if (handleFree && other.tag == "MeHandle" && onUpper) {
            await speech.Speak("trapped!");
            upperHandle.Freeze();
            handleFree = false;
            inTrap = true;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!handleFree && inTrap)
            {
                upperHandle.Free();
                handleFree = true;
                inTrap = false;
            }
        }
    }

    void OnApplicationQuit()
    {
        speech.Stop();
    }
}
