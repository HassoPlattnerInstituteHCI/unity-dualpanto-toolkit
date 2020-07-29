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
    LowerHandle lowerHandle;
    void Start()
    {
        speech = new SpeechOut();
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
    }

    public async Task WakeLink()
    {
        await Task.WhenAll(playSound(flySound), lowerHandle.SwitchTo(GameObject.Find("DoorPosition"), 0.2f));
        await speech.Speak("I wonder if he's still sleeping.");

        await Task.WhenAll(
            speech.Speak("Hey Link wake up. It's okay you were just having a nightmare. I am Navi, your guardian fairy."),
        lowerHandle.SwitchTo(GameObject.Find("BedPosition"), 0.2f)
        );
        await GameObject.Find("Panto").GetComponent<Level>().PlayIntroduction();
        await Task.WhenAll(
            speech.Speak("Come over here first. Before we leave you have to get dressed."),
            lowerHandle.SwitchTo(GameObject.Find("Dresser"), 0.2f)
        );
    }
    public async Task BerateLink()
    {
        await speech.Speak("Hey you're still in your underwear! Get back here!");
    }
    public async Task ShowToDoor()
    {
        await speech.Speak("Great you've got yourself dressed. Now we can go.");
        await lowerHandle.SwitchTo(GameObject.Find("DoorPosition"), 0.2f);
    }
    public async Task ShowToOldMan()
    {
        await speech.Speak("Come Link we have to talk to the old man. He lives in his house next door. Follow me.");
        await lowerHandle.SwitchTo(GameObject.Find("OldMan"), 0.2f);
    }
}
