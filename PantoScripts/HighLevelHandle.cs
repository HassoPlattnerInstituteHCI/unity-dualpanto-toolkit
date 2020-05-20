using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLevelHandle : PantoBehaviour
{
    protected PantoHandle pantoHandle;
    public bool isUpper = true;
    public bool isActive = false;

    protected override void Awake()
    {
        base.Awake();
        InitializePantoHandle();
    }

    void InitializePantoHandle()
    {
        if (isUpper)
        {
            pantoHandle = GetPantoGameObject().GetComponent<UpperHandle>();
        }
        else
        {
            pantoHandle = GetPantoGameObject().GetComponent<LowerHandle>();
        }
    }

    public bool GetIsUpper()
    {
        return isUpper;
    }

    public void SetIsUpper(bool newIsUpper)
    {
        isUpper = newIsUpper;
        InitializePantoHandle();
    }

    protected bool getIsActive()
    {
        return isActive;
    }
    public void activate()
    {
        isActive = true;
    }
    public void deactivate()
    {
        isActive = false;
    }
}
