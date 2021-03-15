using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;

public class PantoIntroSpeaker : PantoIntroBase
{
    public string text;
    
    private SpeechOut speechOut;

    private void Awake()
    {
        speechOut = new SpeechOut();
    }

    public override async Task Introduce()
    {
        await speechOut.Speak(text);
    }
}
