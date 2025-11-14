using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RoomEcho : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audioSource;
    private bool onItHandle = false;
    private bool onMeHandle = true;

    public RoomEcho(AudioSource source, bool onItHandle, bool onMeHandle)
    {
        this.audioSource = source;
        this.onItHandle = onItHandle;
        this.onMeHandle = onMeHandle;
    }
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerEnter(Collider other){
        
    }
}
