using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DualPantoToolkit;
using SpeechIO;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Plays speech when the PantoHandle enters a room. 
/// !!! Room should have a collider with "Is Trigger" enabled.
/// </summary>
public class RoomSpeechOnEntry : MonoBehaviour
{
    public bool onUpperEnter = true;
    public bool onLowerEnter = false;

    [Multiline]
    [Tooltip("Text to be spoken when entering the room.")]
    public string introductionText;

    [Tooltip("When both AudioClip and introductionText are set, the AudioClip will be played.")]
    public AudioClip introductionClip;

    [Header("Event triggers:")]

    [Header("trigger onEnter() event after speech end")]
    public bool triggerAfterSpeechEnd = false;

    [Header("Event triggers when Handle enters the room.")]
    public UnityEvent onEnter = new UnityEvent();

    [Header("Event triggers when Handle exits the room.")]
    public UnityEvent onExit = new UnityEvent();


    SpeechOut speechOut = new SpeechOut();

    private AudioSource audioSource;

    void Awake()
    {
        if (introductionClip != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = introductionClip;
        }
    }


    async Task OnCollisionEnter(Collision other)
    {
        if(other.gameObject == null) return;
        if ((other.gameObject.tag == "MeHandle" && onUpperEnter) || (other.gameObject.tag == "ItHandle" && onLowerEnter))
        {
            if (triggerAfterSpeechEnd)
            {
                await EntrySpeechSound();
                onEnter.Invoke();
                return;
            }
            onEnter.Invoke();
            await EntrySpeechSound();
        }
    }

    async void OnCollisionExit(Collision other)
    {
        if(other.gameObject == null) return;
        if ((other.gameObject.tag == "MeHandle" && onUpperEnter) || (other.gameObject.tag == "ItHandle" && onLowerEnter))
        {
            onExit.Invoke();
            speechOut.Stop(false);
        }
    }

    // wait method for audio source
    public async Task WaitSoundFinished()
    {
        if (!audioSource.isPlaying)
        {
            return;
        }
        // compute remaining time
        double remaining = audioSource.clip.length - audioSource.time;
        remaining = remaining / System.Math.Max(0.0001, audioSource.pitch);
        double endDsp = AudioSettings.dspTime + remaining;

        while (AudioSettings.dspTime < endDsp)
        {
            // avoid blocking the thread
            await Task.Yield();
            if (audioSource == null || !audioSource.isPlaying) break;
        }
    }

    private async Task EntrySpeechSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
            await WaitSoundFinished();
        }
        else
        {
            await speechOut.Speak(introductionText);
        }
    }
}

