using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using DualPantoFramework;
using System.Threading.Tasks;

public class Navi : LegendBehaviour
{
    public AudioClip flySound;
    SpeechOut speech;
    //LowerHandle lowerHandle;
    void Start()
    {
        speech = new SpeechOut();
        //lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
    }

    public async Task WakeLink()
    {
        await Task.WhenAll(playSound(flySound)); //lowerHandle.SwitchTo(GameObject.Find("DoorPosition"), 0.2f));
        await speech.Speak("I wonder if he's still sleeping.");

        await Task.WhenAll(
            speech.Speak("Hey Link wake up. It's okay you were just having a nightmare. I am Navi, your guardian fairy.")
        //lowerHandle.SwitchTo(GameObject.Find("BedPosition"), 0.2f)
        );

        //TODO show room
    }
}
