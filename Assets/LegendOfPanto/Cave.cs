using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace LegendOfPanto
{
    public class Cave : LoPTriggerBehaviour
    {
        public AudioClip zeldaGasp;
        bool struggling;
        void Update()
        {
            if (struggling)
            {
                manager.NaviStruggle();
            }
        }
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
                manager.StopLink();
                await manager.NaviSpeak("There is no one here. The old man must have");
                await manager.playSound(zeldaGasp);
                struggling = true;
                await Task.Delay(5);
                struggling = false;
                manager.FreeLink();
            }
        }
    }
}
