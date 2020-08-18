using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace LegendOfPanto
{
    public class Chest : LoPTriggerBehaviour
    {
        public AudioClip chest_opening;
        public AudioClip fanfare;
        protected async override void LinkEntered()
        {
            if (manager.gameState == GameState.FIND_BOW)
            {
                manager.StopLink();
                await manager.playSound(chest_opening);
                await manager.playSound(fanfare);
                await manager.Speak("You got a bow.");
                await manager.NaviSpeak("Let's get back to the old man.");
                manager.gameState = GameState.OLDMAN_TARGET;
                await manager.NaviFollowPath("ChestToOldMan");
                manager.FreeLink();
            }
        }
    }
}
