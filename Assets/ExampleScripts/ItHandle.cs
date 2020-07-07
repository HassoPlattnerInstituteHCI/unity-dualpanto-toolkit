using UnityEngine;
using DualPantoFramework;

public class ItHandle : MonoBehaviour
{
    PantoHandle lowerHandle;
    void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
    }

    void FixedUpdate()
    {
        transform.position = lowerHandle.HandlePosition(transform.position);
    }
}
