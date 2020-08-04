using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dresser : LoPTriggerBehaviour
{
    protected override void LinkEntered()
    {
        if (manager.gameState == GameState.DRESS) manager.DresserEntered();
    }
}
