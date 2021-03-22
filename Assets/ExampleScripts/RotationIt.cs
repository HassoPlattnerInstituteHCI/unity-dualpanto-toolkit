using UnityEngine;
using DualPantoFramework;

public class RotationIt : MonoBehaviour
{
    PantoHandle lowerHandle;
    void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        // await lowerHandle.MoveToPosition(transform.position, 1.0f);
    }

    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, lowerHandle.GetRotation(), 0);
        transform.RotateAround(transform.position, Vector3.up, 0.5f);
        lowerHandle.Rotate(transform.eulerAngles.y);
    }
}
