using UnityEngine;
using System.Threading.Tasks;
using DualPantoToolkit;

public abstract class PantoIntroBase : MonoBehaviour
{
    public abstract Task Introduce(); // overwrite with the main logic
    public virtual void CancelIntro() { } // optionally overwrite with how to cancel this intro as soon as possible
    public virtual void SetVisualization(bool visible) { } // overwrite if intro has visuals

    PantoHandle upper;
    PantoHandle lower;
    void Start()
    {
        upper = GameObject.Find("Panto").GetComponent<UpperHandle>();
        lower = GameObject.Find("Panto").GetComponent<LowerHandle>();
    }
    protected async Task SwitchTo(GameObject go, bool onUpper, float speed = 3.0f)
    {
        if (onUpper) await upper.SwitchTo(go, speed);
        else await lower.SwitchTo(go, speed);
    }

    protected void Free(bool onUpper)
    {
        if (onUpper) upper.Free();
        else lower.Free();

    }
}
