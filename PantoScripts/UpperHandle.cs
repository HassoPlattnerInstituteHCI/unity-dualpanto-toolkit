using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperHandle : PantoHandle
{
    new void Awake()
    {
        base.Awake();
        isUpper = true;
        pantoSync.RegisterUpperHandle(this);
    }
}