using System.Collections;
using System.Collections.Generic;
using PathCreation;
using SpeechIO;
using UnityEngine;

namespace LegendOfPanto
{
    public class OldMan : LoPTriggerBehaviour
    {
        SpeechIn speechIn;
        void Start()
        {
            speechIn = new SpeechIn(onRecognized);
        }

        void onRecognized(string result) { }
        async void oldManIntro()
        {
            await manager.OldManSpeak("Hello my young friend I am the old man. I heard you are going on an adventure. It's dangerous to go alone. I have hidden a surprise for you behind your house. But be careful about the river next to it. Do you understand me?");
            oldManIntroListeningLoop();
        }

        async void oldManIntroListeningLoop()
        {
            string answer = await speechIn.Listen(new string[] { "yes", "no" });
            if (answer == "yes")
            {
                await manager.OldManSpeak("Ok great then follow navi.");
                manager.gameState = GameState.RIVER;
                await manager.NaviSpeak("Ok Link let's go! Come with me to the river.");
                await manager.NaviFollowPath("OldManToRiver");
            }
            else if (answer == "no")
            {
                await manager.OldManSpeak("On the right side of the house is a surprise for you. Do you understand me?");
                oldManIntroListeningLoop();
            }
        }

        async void oldManTarget()
        {
            await manager.OldManSpeak("I see you found the bow. Go outside to the tree and practice shooting");
            await manager.NaviSpeak("You can fire the bow by saying fire. I will show you the way.");
            manager.gameState = GameState.TARGET;
            await manager.NaviFollowPath("OldManToTarget");
            manager.StartShootingLoop();
        }
        async void oldManQuest()
        {
            await manager.OldManSpeak("Nice shooting son. Now I have an important quest for you. On the right side of the river is a cave. When I was walking the other day I heard a strange noise coming out of the cave. I think it's one of Ganons evil minions. Go to the cave and see if a threat lies in it.");
            await manager.NaviSpeak("Ok Link I will show you the way.");
            await manager.NaviFollowPath("OldManToCave");
        }
        async void endGame()
        {
            await manager.OldManSpeak("Navi told me you saved her from the evil monster. Thank you so much. But now is not the time to celebrate. Ganon wants to take over this peaceful land. You have to go south and visit the princess of hyrule. Her name is Panto.");
            await manager.OldManSpeak("Thanks for playing the demo of The Legend of Panto.");
        }

        protected override void LinkEntered()
        {
            if (manager.gameState == GameState.OLDMAN_INTRO)
            {
                oldManIntro();
            }
            else if (manager.gameState == GameState.OLDMAN_TARGET)
            {
                oldManTarget();
            }
            else if (manager.gameState == GameState.OLDMAN_QUEST)
            {
                oldManQuest();
            }
            else if (manager.gameState == GameState.END)
            {
                endGame();
            }
        }
    }
}
