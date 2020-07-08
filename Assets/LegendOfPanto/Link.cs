using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    public AudioClip nightmareSoundLink;
    public AudioClip nightmareLaughGanon;

    void Start()
    {
        Nightmare();
    }

    void Nightmare()
    {
        Debug.Log("rotate");
        float speed = 20;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 30, 0), speed * Time.deltaTime);
        transform.Rotate(Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, -60, 0), speed * Time.deltaTime);
    }

    void Update()
    {

    }
}
