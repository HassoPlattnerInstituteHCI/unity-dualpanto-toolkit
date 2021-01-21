using UnityEngine;
using DualPantoFramework;

public class RotationIt : MonoBehaviour
{
    PantoHandle lowerHandle;
    async void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        await lowerHandle.MoveToPosition(transform.position, 1.0f);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            lowerHandle.FreeRotation();
        }
        transform.RotateAround(transform.position, Vector3.up, 0.5f);
        lowerHandle.Rotate(transform.eulerAngles.y);
    }
}
