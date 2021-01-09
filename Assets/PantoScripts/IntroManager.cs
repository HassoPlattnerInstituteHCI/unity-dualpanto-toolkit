using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;
using UnityEngine;

public class IntroManager : MonoBehaviour
{

    IntroContourStrategy introContourStrategy;
    UpperHandle upper;
    LowerHandle lower;
    public SpeechOut speechOut;
    // Start is called before the first frame update
    void Start()
    {
        speechOut = new SpeechOut();
        lower = GameObject.Find("Panto").GetComponent<LowerHandle>();
        upper = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }
    public async Task MoveToPosition(Vector3 position, bool onUpper)
    {
        if (onUpper) await upper.MoveToPosition(position);
        else await lower.MoveToPosition(position);
    }
    public async Task SwitchTo(GameObject go, bool onUpper, float speed = 3.0f)
    {
        if (onUpper) await upper.SwitchTo(go, speed);
        else await lower.SwitchTo(go, speed);
    }
    public async Task MoveAndSpeak(Vector3 position, string text, bool onUpper = false)
    {
        if (onUpper)
        {
            await Task.WhenAll(
                upper.MoveToPosition(position, 5f, false),
                speechOut.Speak(text)
            );
        }
        else
        {
            await Task.WhenAll(
                lower.MoveToPosition(position, 5f, false),
                speechOut.Speak(text)
            );
        }
    }

    public async Task Speak(string text)
    {
        await speechOut.Speak(text);
    }


}
