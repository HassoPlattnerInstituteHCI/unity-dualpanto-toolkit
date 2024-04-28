using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;

public abstract class PantoIntroWithAudio : PantoIntroBase
{
    // lazy initialization, creating only what's actually needed
    protected SpeechOut speechOut
    { get { if (_speechOut == null) _speechOut = new SpeechOut(); return _speechOut; } }
    private SpeechOut _speechOut;

    protected AudioSource audioSrc
    { get { if (!_audioSrc) _audioSrc = gameObject.AddComponent<AudioSource>(); return _audioSrc; } }
    private AudioSource _audioSrc;


    protected async Task PlayAudioOrSpeak(AudioClipOrText audio)
    {
        if (audio.clip) // clip is preferred
        {
            audioSrc.clip = audio.clip;
            audioSrc.Play();
            while (audioSrc.isPlaying)
            {
                await Task.Delay(10);
            }
        }
        else if (audio.textToSpeech != "")
        {
            await speechOut.Speak(audio.textToSpeech);
        }
    }

    protected bool IsSpeakingOrPlaying()
    { // even more avoiding of unnecessary initialization
        if (_speechOut != null)
            // if (_speechOut.IsSpeaking())
                return true;
        if (_audioSrc)
            if (_audioSrc.isPlaying)
                return true;
        return false;
    }

    protected void CancelAudio()
    {
        if (IsSpeakingOrPlaying())
        {
            if (_audioSrc)
            {
                _audioSrc.Stop();
            }
        }
    }

    [System.Serializable]
    public class AudioClipOrText
    {
        public AudioClip clip;
        [Tooltip("only a clip or a textToSpeech has to be entered")]
        public string textToSpeech;

        public bool IsSpecified()
        {
            return clip || (textToSpeech != "");
        }
    }
}

