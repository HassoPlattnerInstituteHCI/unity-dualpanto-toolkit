using System.Collections;
using System.Collections.Generic;
using SpeechIO;
using UnityEngine;

public class OldMan : MonoBehaviour
{
    SpeechOut speech;
    SpeechIn speechIn;
    void Start()
    {
        speech = new SpeechOut();
        speechIn = new SpeechIn(onRecognized);
    }

    void onRecognized(string result) { }
    async void oldManIntro()
    {
        await speech.Speak("Hello my young friend I am the old man. I heard you are going on an adventure. It's dangerous to go alone. I have hidden a surprise for you behind your house. But be careful about the river next it. Do you understand me?");
        string answer = await speechIn.Listen(new string[] { "yes", "no" });
        if (answer == "yes")
        {
            await speech.Speak("Ok great then follow navi.");
        }
        else if (answer == "no")
        {
            await speech.Speak("On the right side of the house is a surprise for you. Do you understand me?");

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Link")
        {
            oldManIntro();
        }
    }
}
