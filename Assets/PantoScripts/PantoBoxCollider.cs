using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantoBoxCollider : PantoCollider
{
    public override void CreateObstacle()
    {
        CreateBoxObstacle();
    }
}