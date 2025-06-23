using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;

public class OnTriggerSound : MonoBehaviour
{
    public AudioClip triggerSound;      // Assign your sound file in Inspector
     private AudioSource audioSource;
     public string description = "0";
     SpeechOut speechOut = new SpeechOut();
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "MeHandle" || other.tag == "ItHandle")  // Optional: filter by tag
        {
            
            if (triggerSound != null && audioSource != null)
            {
                if (!audioSource.isPlaying)
                {
                    if (IsCompletelyInside(gameObject, other)) ;
                    audioSource.PlayOneShot(triggerSound);
                   // speechOut.Speak(description);
                }
            }
        }
    }
    
    
    bool IsCompletelyInside(GameObject obj, Collider trigger)
    {
        Bounds triggerBounds = trigger.bounds;
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend == null) return false;

        Bounds objBounds = rend.bounds;

        // Check if all corners of the object's bounds are inside the trigger
        Vector3[] corners = new Vector3[]
        {
            objBounds.min,
            objBounds.max,
            new Vector3(objBounds.min.x, objBounds.min.y, objBounds.max.z),
            new Vector3(objBounds.min.x, objBounds.max.y, objBounds.min.z),
            new Vector3(objBounds.max.x, objBounds.min.y, objBounds.min.z),
            new Vector3(objBounds.max.x, objBounds.max.y, objBounds.min.z),
            new Vector3(objBounds.max.x, objBounds.min.y, objBounds.max.z),
            new Vector3(objBounds.min.x, objBounds.max.y, objBounds.max.z)
        };

        foreach (Vector3 corner in corners)
        {
            if (!triggerBounds.Contains(corner))
            {
                return false; 
            }
        }

        return true;
    }
}
