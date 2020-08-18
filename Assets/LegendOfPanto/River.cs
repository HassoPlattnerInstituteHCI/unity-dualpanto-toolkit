using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace LegendOfPanto
{
    public class River : LoPTriggerBehaviour
    {
        public AudioClip riverSound;
        protected async override void LinkEntered()
        {
            if (manager.gameState >= GameState.RIVER)
            {
                manager.playSoundLooping(riverSound);
            }
            if (manager.gameState == GameState.RIVER)
            {
                manager.gameState = GameState.FIND_BOW;
                manager.StopLink();
                await manager.NaviSpeak("Ok to get behind the house we have to swim up the river. But be careful with the strong stream.");
                await manager.NaviFollowPath("RiverToChest");
                manager.FreeLink();
            }
            else if (manager.gameState < GameState.RIVER)
            {
                manager.StopLink();
                await manager.NaviSpeak("Link, get back here!");
                manager.FreeLink();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Link")
            {
                manager.stopSoundLooping();
            }
        }
    }
}