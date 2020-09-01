using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendOfPanto
{
    public class CaveEntrance : LoPTriggerBehaviour
    {
        protected async override void LinkEntered()
        {
            if (manager.gameState == GameState.OLDMAN_QUEST)
            {
                manager.gameState = GameState.MONSTER_INTRO;
                manager.StopLink();
                await manager.NaviSpeak("I wonder what the man heard? Let's go inside.");
                await manager.NaviMoveTo(GameObject.Find("CaveEntered"));
                manager.FreeLink();
            }
        }
    }
}
