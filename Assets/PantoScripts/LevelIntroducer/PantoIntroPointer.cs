using System.Threading.Tasks;
using UnityEngine;

public class PantoIntroPointer : PantoIntroWithAudio
{
    public bool onUpper;
    public float moveSpeed = 3f;
    public AudioClipOrText audioWhileMoving;
    public AudioClipOrText audioAfterMoving;
    private bool shouldCancel = false;

    public override async Task Introduce()
    {
        if (audioWhileMoving.IsSpecified())
        {
            await Task.WhenAll(
                PlayAudioOrSpeak(audioWhileMoving),
                SwitchTo(gameObject, onUpper, moveSpeed)
                );
        }
        else
        {
            await SwitchTo(gameObject, onUpper, moveSpeed);
        }
        if (audioAfterMoving.IsSpecified() && !shouldCancel)
        {
            await PlayAudioOrSpeak(audioAfterMoving);
        }
    }

    public override void CancelIntro()
    {
        CancelAudio();
        Free(onUpper);
        shouldCancel = true;
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
