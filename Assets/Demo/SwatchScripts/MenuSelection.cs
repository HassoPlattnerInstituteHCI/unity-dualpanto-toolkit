using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;
using SpeechIO;

public class MenuSelection : MonoBehaviour
{
    PantoHandle upperHandle;
    PantoHandle lowerHandle;
    int selectedMenuOption = 0;
    bool rotatedFromZero = true;
    SpeechOut speech;
    bool disableSelection = false;
    public int numberOfOptions = 3;
    // Start is called before the first frame update
    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        upperHandle.Rotate(0.0f);

        // lowerHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        // lowerHandle.Rotate(0.0f);
    }

    float NormalizeAngle(float angle) {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }

    // Update is called once per frame
    async void Update()
    {
        // Debug.Log(NormalizeAngle(upperHandle.GetRotation()).ToString());
        // Debug.Log(NormalizeAngle(lowerHandle.GetRotation()).ToString());

        if (Mathf.Abs(NormalizeAngle(upperHandle.GetRotation())) > 50 && !disableSelection) {
            selectedMenuOption += 1;
            if (selectedMenuOption > numberOfOptions) {
                selectedMenuOption = 1;
            }
            upperHandle.Rotate(0.0f);
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            disableSelection = true;
            await speech.Speak("Selected option number " + selectedMenuOption.ToString());
            Debug.Log("Selected option number " + selectedMenuOption.ToString());
            disableSelection = false;
        }

        // if (Input.GetKeyDown(KeyCode.G)) {
        //     Debug.Log("Rotation: " + upperHandle.GetRotation().ToString());
        // }
    }

    void OnApplicationQuit()
    {
        speech.Stop();
    }
}
