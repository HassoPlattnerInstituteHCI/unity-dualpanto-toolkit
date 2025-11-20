using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;
using SpeechIO;

public class MenuSelection : MonoBehaviour
{
    PantoHandle upperHandle;
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
    }

    // Update is called once per frame
    async void Update()
    {
        if (upperHandle.GetRotation() > 10 && rotatedFromZero && !disableSelection) {
            selectedMenuOption += 1;
            rotatedFromZero = false;
            if (selectedMenuOption > numberOfOptions) {
                selectedMenuOption = 1;
            }
            upperHandle.Rotate(0.0f);
            rotatedFromZero = true;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            disableSelection = true;
            await speech.Speak("Selected option number " + selectedMenuOption.ToString());
            Debug.Log("Selected option number " + selectedMenuOption.ToString());
            disableSelection = false;
        }
    }

    void OnApplicationQuit()
    {
        speech.Stop();
    }
}
