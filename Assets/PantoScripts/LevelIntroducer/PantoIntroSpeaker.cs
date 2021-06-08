using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;

public class PantoIntroSpeaker : PantoIntroWithAudio
{
    public AudioClipOrText audioClipOrText;

    public override async Task Introduce()
    {
        await PlayAudioOrSpeak(audioClipOrText);
    }

    public override void CancelIntro()
    {
        CancelAudio();
    }
}
