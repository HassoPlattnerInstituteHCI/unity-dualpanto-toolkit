using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : LoPTriggerBehaviour
{
    public AudioClip zeldaGasp;
    protected async override void LinkEntered()
    {
        if (manager.gameState < GameState.OLDMAN_QUEST)
        {
            manager.StopLink();
            await manager.NaviSpeak("Hey Link get back here.");
            manager.FreeLink();
        }
        else if (manager.gameState == GameState.OLDMAN_QUEST)
        {
            manager.gameState = GameState.MONSTER_INTRO;
            manager.StopLink();
            await manager.NaviSpeak("I wonder what the man heard? Let's go inside.");

            await manager.NaviSpeak("There is no one here. The old man must have");
            await manager.playSound(zeldaGasp);
            //.then(() => device.movePantoTo(1, new Vector(82, -166), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100, Math.PI / 4), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100, Math.PI / 2), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100, 3 * Math.PI / 4), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100, Math.PI), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100, 5 * Math.PI / 4), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100, 3 * Math.PI / 2), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100, 7 * Math.PI / 4), 30))
            //.then(() => device.movePantoTo(1, new Vector(67, -100, 2 * Math.PI), 30))
            manager.FreeLink();
        }
    }
}


