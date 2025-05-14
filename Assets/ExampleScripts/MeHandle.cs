using UnityEngine;
using DualPantoToolkit;

public class MeHandle : MonoBehaviour
{
    PantoHandle upperHandle;
    bool free = true;
    
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        if (upperHandle == null)
        {
            Debug.LogError("[DualPanto] UpperHandle not found on Panto GameObject!");
        }
    }

    void FixedUpdate()
    {
        transform.position = upperHandle.GetPosition();
        transform.eulerAngles = new Vector3(0, upperHandle.GetRotation(), 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (free)
            {
                upperHandle.Freeze();
            }
            else
            {
                upperHandle.Free();
            }
            free = !free;
        }
    }
}
