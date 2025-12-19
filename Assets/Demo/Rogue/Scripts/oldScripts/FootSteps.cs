using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioSource footstepsSound, sprintSound;

    void Update()
    {
        bool pressingKeys = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        bool running = Input.GetKey(KeyCode.LeftShift);

        if (pressingKeys)
        {
            if (running)
            {
                if (!sprintSound.isPlaying)
                {
                    sprintSound.Play();
                    footstepsSound.Stop();
                }
            }
            else
            {
                if (!footstepsSound.isPlaying)
                {
                    footstepsSound.Play();
                    sprintSound.Stop();
                }
            }
        }
        else
        {
            sprintSound.Stop();
            footstepsSound.Stop();
        }
    }
}