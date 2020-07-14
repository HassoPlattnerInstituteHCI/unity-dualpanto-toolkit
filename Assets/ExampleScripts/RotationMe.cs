using UnityEngine;
using DualPantoFramework;

public class RotationMe : MonoBehaviour
{
    UpperHandle upperHandle;
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }

    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, upperHandle.GetRotation(), 0);
    }
}
