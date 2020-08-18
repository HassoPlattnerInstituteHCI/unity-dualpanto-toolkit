using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendOfPanto
{
    public class Dresser : LoPTriggerBehaviour
    {
        protected async override void LinkEntered()
        {
            if (manager.gameState == GameState.DRESS)
            {
                manager.gameState = GameState.DOOR;
                await manager.NaviSpeak("Great you've got yourself dressed. Now we can go.");
                await manager.NaviMoveTo(GameObject.Find("Door"));
            }
        }
    }
}
