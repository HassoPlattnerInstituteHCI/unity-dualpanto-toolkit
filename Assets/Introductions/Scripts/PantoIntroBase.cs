using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DualPantoFramework;

public abstract class PantoIntroBase : MonoBehaviour
{
    public abstract Task Introduce();

    public virtual void SetVisualization(bool visible)
    {
        
    }

    // helpers

    protected PantoHandle getHandle(bool upper)
    {
        if (upper)
        {
            return FindObjectOfType<UpperHandle>();
        }
        else
        {
            return FindObjectOfType<LowerHandle>();
        }
    }
}

