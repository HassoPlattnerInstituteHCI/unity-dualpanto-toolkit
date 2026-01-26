using UnityEngine;
using DualPantoToolkit;
using System.Threading.Tasks;

public class MoveToRoom : MonoBehaviour
{
    public bool isUpper;
    public bool shouldFreeHandle;
    public float speed = 10f;
    PantoHandle handle;
    async void OnEnable()
    {
        await Task.Delay(500);
        handle = isUpper
            ? (PantoHandle)GameObject.Find("Panto").GetComponent<UpperHandle>()
            : (PantoHandle)GameObject.Find("Panto").GetComponent<LowerHandle>();
        
        Debug.Log("game object position: " + gameObject.transform.position.ToString());

        await handle.MoveToPosition(gameObject.transform.position, speed, shouldFreeHandle);

        Debug.Log("handle position: " + handle.GetPosition().ToString());
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("game object position: " + gameObject.transform.position.ToString());

            await handle.MoveToPosition(gameObject.transform.position, speed, shouldFreeHandle);

            Debug.Log("handle position: " + handle.GetPosition().ToString());
        }
    }
}
