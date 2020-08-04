using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : LoPTriggerBehaviour
{
    public AudioClip chest_opening;
    public AudioClip fanfare;
    protected async override void LinkEntered()
    {
        if (manager.gameState == GameState.FIND_BOW)
        {
            await manager.playSound(chest_opening);
            await manager.playSound(fanfare);
            await manager.Speak("You got a bow.");
            await manager.NaviSpeak("Let's get back to the old man.");
            manager.gameState = GameState.OLDMAN_TARGET;
            //.then(() => device.movePantoTo(1, new Vector(8, -50), naviMoveSpeed))
            //.then(() => device.movePantoTo(1, new Vector(8, -122), naviMoveSpeed))
            //.then(() => device.movePantoTo(1, new Vector(-112, -122, 0), naviMoveSpeed))
            //.then(() => device.movePantoTo(1, new Vector(-112, -65, 0), naviMoveSpeed))
        }
    }
}
