using UnityEngine;
using DualPantoToolkit;

public class ItHandle : MonoBehaviour
{
    PantoHandle lowerHandle;
    bool free = true;

    void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        if (lowerHandle == null)
        {
            Debug.LogError("[DualPanto] LowerHandle not found on Panto GameObject!");
        }
    }

    void FixedUpdate()
    {
        transform.position = lowerHandle.GetPosition();
        transform.eulerAngles = new Vector3(0, lowerHandle.GetRotation(), 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (free)
            {
                lowerHandle.Freeze();
            }
            else
            {
                lowerHandle.Free();
            }
            free = !free;
        }
    }
}
