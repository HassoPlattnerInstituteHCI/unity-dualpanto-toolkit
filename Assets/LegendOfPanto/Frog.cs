using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendOfPanto
{
    public class Frog : LoPTriggerBehaviour
    {
        public AudioClip bossIntro;
        protected async override void LinkEntered()
        {
            if (manager.gameState == GameState.MONSTER_INTRO)
            {
                manager.gameState = GameState.MONSTER_FIGHT;
                //device.movePantoTo(0, new Vector(82, -135), 30)
                // lock link inside
                await manager.GanonSpeak("It's too late I ate your little fairy friend and you're next!");
                await manager.playSound(bossIntro);
                //.then(() => enemyManager.add(67, -100, 25, 25, 0))
                //.then(() => enemyManager.trigger(0))
                //.then(() => VoiceInteraction.beginListening())
                manager.FreeLink();
                //.then(() =>
                //{
                //musicPlayer = VoiceInteraction.playSound("./sounds/OOT_Music/OOT_BossMusic.wav", true);
                //}
            }
        }
    }
}

