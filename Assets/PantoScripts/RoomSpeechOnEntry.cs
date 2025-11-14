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

namespace DualPantoToolkit
{
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


        [Header("Event triggers when Handle enters the room.")]
        public UnityEvent onEnter;

        [Header("Event triggers when Handle exits the room.")]
        public UnityEvent onExit;

        
        SpeechOut speechOut = new SpeechOut();

        async Task OnTriggerEnter(Collider other)
        {
            if ((other.tag == "MeHandle" && onUpperEnter) || (other.tag == "ItHandle" && onLowerEnter))
            {
                onEnter.Invoke();
                await speechOut.Speak(introductionText);
            }
        }
        async void OnTriggerExit(Collider other)
        {
            
            if ((other.tag == "MeHandle" && onUpperEnter) || (other.tag == "ItHandle" && onLowerEnter))
            {
                onExit.Invoke();
                speechOut.Stop(false);
            }
        }

    }
}