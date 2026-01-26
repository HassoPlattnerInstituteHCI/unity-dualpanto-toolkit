using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;

public class CollectItem : MonoBehaviour
{
    SpeechOut speech;
    // Start is called before the first frame update
    void Start()
    {
        speech = new SpeechOut();
    }

    async void OnTriggerEnter(Collider other) {
        if (other.gameObject == null) return;
        if (other.gameObject.tag == "MeHandle") {
            await speech.Speak("Item found!");
        }
    }

    void OnApplicationQuit()
    {
        speech.Stop();
    }
}
