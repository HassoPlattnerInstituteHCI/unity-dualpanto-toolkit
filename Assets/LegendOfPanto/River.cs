using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : LoPTriggerBehaviour
{
    public AudioClip riverSound;
    protected override void LinkEntered()
    {
        if (manager.gameState >= GameState.RIVER)
        {
            manager.playSoundLooping(riverSound);
        }
    }
    protected new void LinkExited()
    {
        manager.stopSoundLooping();
    }
}