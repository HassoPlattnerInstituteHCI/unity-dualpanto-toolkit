using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;
using System.Threading.Tasks;

public class SpeedLimit : MonoBehaviour
{
    private Vector3 lastPosition;
    public Vector3 velocity { get; private set; }
    public bool isUpper;
    PantoHandle handle;
    private bool hasTriggered = false;
    public float waitSecond = 1f;

    public int d = 100;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        handle = isUpper
            ? (PantoHandle)GameObject.Find("Panto").GetComponent<UpperHandle>()
            : (PantoHandle)GameObject.Find("Panto").GetComponent<LowerHandle>();

    }

    // Update is called once per frame
    // async void Update()
    // {
    //     // await Task.Delay(d);
    //     // handle.Freeze();
    //     // await Task.Delay(d);
    //     // handle.Free();
    // }

    void Update()
    {
        // velocity = (transform.position - lastPosition) / Time.deltaTime;
        // lastPosition = transform.position;
        //
        // Debug.Log("Velocity: " + velocity);
        // handle.ApplyForce(-velocity, 3);
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trap");
        if (!hasTriggered)
        {
            hasTriggered = true;
            TriggerImmediate();
            StartCoroutine(TriggerDelayed());
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (hasTriggered)
        {
            Debug.Log("Player exited the trap area.");
            // Do something when the player leaves the trigger zone
            hasTriggered = false;
        }
    }

    
    void TriggerImmediate()
    {
        Debug.Log("Trap triggered immediately!");
        // Add immediate trap logic here (e.g. damage, sound, animation)
        handle.Freeze();
    }

    IEnumerator TriggerDelayed()
    {
        yield return new WaitForSeconds(waitSecond);
        Debug.Log("Trap triggered after 1 second!");
        // Add delayed trap logic here (e.g. spawn enemy, close door, etc.)
        handle.Free();
        
    }
}
