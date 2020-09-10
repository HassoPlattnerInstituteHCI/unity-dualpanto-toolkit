using UnityEngine;
using DualPantoFramework;

public class MoveToPosition : MonoBehaviour
{
    public bool isUpper;
    public bool shouldFreeHandle;
    public float speed = 10f;
    async void Start()
    {
        PantoHandle handle = isUpper
            ? (PantoHandle)GameObject.Find("Panto").GetComponent<UpperHandle>()
            : (PantoHandle)GameObject.Find("Panto").GetComponent<LowerHandle>();

        await handle.MoveToPosition(gameObject.transform.position, speed, shouldFreeHandle);
    }
}
