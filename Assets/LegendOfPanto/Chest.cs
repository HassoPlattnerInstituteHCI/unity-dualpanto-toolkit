using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Manager manager;
    public AudioClip chest_opening;
    public AudioClip fanfare;
    async void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Link")
        {
            await manager.playSound(chest_opening);//('./sounds/OOT_OtherSounds/OOT_Chest_Big.wav')
            await manager.playSound(fanfare);// .then(() => VoiceInteraction.playSound('./sounds/OOT_Fanfare/OOT_Fanfare_SmallItem.wav'))
            await manager.Speak("You got a bow.");
            await manager.NaviSpeak("Let's get back to the old man.");
            //.then(() => device.movePantoTo(1, new Vector(8, -50), naviMoveSpeed))
            //.then(() => device.movePantoTo(1, new Vector(8, -122), naviMoveSpeed))
            //.then(() => device.movePantoTo(1, new Vector(-112, -122, 0), naviMoveSpeed))
            //.then(() => device.movePantoTo(1, new Vector(-112, -65, 0), naviMoveSpeed))
        }
    }
}
