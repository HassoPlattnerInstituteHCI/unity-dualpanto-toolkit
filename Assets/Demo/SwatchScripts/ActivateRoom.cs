using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using DualPantoToolkit;

public class ActivateRoom : MonoBehaviour
{
    public string introductionText;
    SpeechOut speech;
    public GameObject[] objectsInRoom;
    private int playersInRoom = 0;
    
    void Start()
    {
        speech = new SpeechOut();
    }

    async void OnTriggerEnter(Collider other) {
        if (other.gameObject == null) return;
        if (other.gameObject.tag == "MeHandle") {
            playersInRoom++;
            if (playersInRoom == 1) {
                foreach (GameObject currentObject in objectsInRoom) {
                    currentObject.SetActive(true);
                }
                await speech.Speak(introductionText);
            }
            
        }
    }
    
    async void OnTriggerExit(Collider other) {
        if (other.gameObject == null) return;
        if (other.gameObject.tag == "MeHandle") {
            playersInRoom--;
            if (playersInRoom == 0) {
                await speech.Speak("You left the room.");
                foreach (GameObject currentObject in objectsInRoom)
                {
                    currentObject.SetActive(false);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        speech.Stop();
    }
}
