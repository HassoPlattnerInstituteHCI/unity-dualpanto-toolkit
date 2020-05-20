using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOfInterest : MonoBehaviour
{
    public bool traceShape = false;
    public bool isOnUpper = false;
    //public AudioClip introductionSound;
    [TextArea(3,10)]
    public string description;
    public int priority = 0;
}
