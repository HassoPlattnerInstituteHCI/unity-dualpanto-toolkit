using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : LoPTriggerBehaviour
{
    protected async override void LinkEntered()
    {
        if (manager.gameState == GameState.TARGET)
        {
            manager.StopLink();
            await manager.NaviSpeak("That's too close.");
            manager.FreeLink();
        }
        else if (manager.gameState < GameState.TARGET)
        {
            manager.StopLink();
            await manager.NaviSpeak("That looks like a tree.");
            manager.FreeLink();
        }
    }
}
