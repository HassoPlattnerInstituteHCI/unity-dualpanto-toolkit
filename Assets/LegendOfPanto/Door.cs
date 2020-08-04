using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : LoPTriggerBehaviour
{
    protected override void LinkEntered()
    {
        if (manager.gameState == GameState.DOOR) manager.OnExitDoor();
        else if (manager.gameState < GameState.DOOR) manager.OnTryExitUndressed();
    }
}
