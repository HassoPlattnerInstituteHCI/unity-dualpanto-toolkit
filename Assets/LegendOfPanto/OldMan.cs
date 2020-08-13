using System.Collections;
using System.Collections.Generic;
using SpeechIO;
using UnityEngine;

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
        await manager.OldManSpeak("Hello my young friend I am the old man. I heard you are going on an adventure. It's dangerous to go alone. I have hidden a surprise for you behind your house. But be careful about the river next it. Do you understand me?");
        oldManIntroListeningLoop();
    }

    async void oldManIntroListeningLoop()
    {
        string answer = await speechIn.Listen(new string[] { "yes", "no" });
        if (answer == "yes")
        {
            await manager.OldManSpeak("Ok great then follow navi.");
            manager.ShowLinkToRiver();
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

        //.then(() => device.movePantoTo(1, new Vector(-112, -122, 0), naviMoveSpeed))
        //.then(() => device.movePantoTo(1, new Vector(-26, -175, 0), naviMoveSpeed))

        //await speechIn.Listen();

    }
    async void oldManQuest()
    {
        await manager.OldManSpeak("Nice shooting son. Now I have an important quest for you. On the right side of the river is a cave. When I was walking the other day I heard a strange noise coming out of the cave. I think it's one of Ganons evil minions. Go to the cave and see if a thread lies in it.");
        await manager.NaviSpeak("Ok Link I will show you the way.");
        //.then(() => device.movePantoTo(1, new Vector(-112, -122, 0), naviMoveSpeed))
        //.then(() => device.movePantoTo(1, new Vector(39, -166, 0), naviMoveSpeed))

    }
    async void endGame()
    {
        await manager.OldManSpeak("Navi told me you saved her from the evil monster. Thank you so much. But now is not the time to celebrate. Ganon wants to take over this pieceful land. You have to go south and visit the princess of hyrule. Her name is Panto.");
        await manager.OldManSpeak("Thanks for playing the demo of The Legend of Panto.");
    }

    protected override void LinkEntered()
    {
        Debug.Log(manager.gameState);
        if (manager.gameState == GameState.OLDMAN_INTRO)
        {
            oldManIntro();
        }
        else if (manager.gameState == GameState.OLDMAN_TARGET)
        {
            oldManTarget();
        }
        else if (manager.gameState == GameState.OLDMAN_INTRO)
        {
            oldManIntro();
        }
        else if (manager.gameState == GameState.END)
        {
            endGame();
        }
    }
}
