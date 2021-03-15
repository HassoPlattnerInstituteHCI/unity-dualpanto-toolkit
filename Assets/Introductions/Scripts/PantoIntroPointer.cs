using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DualPantoFramework;
using SpeechIO;


public class PantoIntroPointer : PantoIntroBase
{
    public bool onUpper;
    public float moveSpeed = 3f;
    public string sayWhileMoving;
    public string sayAfterMoving;

    private SpeechOut speechOut;

    private void Awake()
    {
        speechOut = new SpeechOut();
    }

    public override async Task Introduce()
    {
        if (sayWhileMoving != "")
        {
            await Task.WhenAll(
                speechOut.Speak(sayWhileMoving),
                getHandle(onUpper).SwitchTo(gameObject, moveSpeed)
                );
        } else
        {
            await getHandle(onUpper).SwitchTo(gameObject, moveSpeed);
        }
        if (sayAfterMoving != "")
        {
            await speechOut.Speak(sayAfterMoving);
        }

    }

    public override void SetVisualization(bool visible)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite)
        {
            sprite.enabled = visible;
        }
    }
}
