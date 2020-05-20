using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerHandle : PantoHandle
{
    new void Awake()
    {
        base.Awake();
        isUpper = false;
        pantoSync.RegisterLowerHandle(this);
    }
}
