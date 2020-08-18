using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace LegendOfPanto
{
    public class Door : LoPTriggerBehaviour
    {
        protected async override void LinkEntered()
        {
            if (manager.gameState == GameState.DOOR)
            {
                manager.gameState = GameState.OLDMAN_INTRO;
                await manager.NaviSpeak("Come Link we have to talk to the old man. He lives in his house next door. Follow me.");
                await manager.NaviFollowPath("DoorToOldMan");
            }
            else if (manager.gameState < GameState.DOOR)
            {
                manager.StopLink();
                await manager.NaviSpeak("Hey you're still in your underwear! Get back here!");
                manager.FreeLink();
            }
        }
    }
}
