using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantoCircularCollider : PantoCollider
{
    public int numberOfCorners = 8;
    public override void CreateObstacle()
    {
        CreateCircularCollider(numberOfCorners);
    }
}