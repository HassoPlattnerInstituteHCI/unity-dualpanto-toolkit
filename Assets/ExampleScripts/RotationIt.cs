using UnityEngine;
using DualPantoFramework;

public class RotationIt : MonoBehaviour
{
    PantoHandle lowerHandle;
    async void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        await lowerHandle.SwitchTo(gameObject, 10f);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            lowerHandle.FreeRotation();
        }
        gameObject.transform.RotateAround(gameObject.transform.position, Vector3.up, 0.5f);
    }
}
