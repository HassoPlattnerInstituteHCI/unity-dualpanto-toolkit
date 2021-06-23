using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(LineRenderer))]
public class PantoIntroBackAndForth : PantoIntroWithAudio
{
    public bool onUpper;
    public float moveSpeed = 3f;
    public AudioClipOrText audioOnWayToFirstPoint;
    public AudioClipOrText audioWhileGoingBackAndForth;
    [Tooltip("When 0, this will stop after the text has been spoken, still finishing the current round.")]
    public int minimumNumberOfRounds;
    public bool playScratchSound = true;

    private LineRenderer lr;
    private AudioSource scratchAudioSource;
    private bool shouldCancel = false;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        if (audioWhileGoingBackAndForth.textToSpeech == "" && minimumNumberOfRounds == 0 && !audioWhileGoingBackAndForth.clip)
            Debug.LogWarning("Either audio needs to be specified or minimumNumberOfRounds needs to be above 0.");
    }

    public override async Task Introduce()
    {
        // go to first point
        if (audioOnWayToFirstPoint.IsSpecified())
        {
            await Task.WhenAll(
                PlayAudioOrSpeak(audioOnWayToFirstPoint),
                MoveToLineIndex(0)
            );
        }
        else
        {
            await MoveToLineIndex(0);
        }

        // begin rounds
        _ = PlayAudioOrSpeak(audioWhileGoingBackAndForth);
        if (playScratchSound && !scratchAudioSource)
        {
            scratchAudioSource = gameObject.AddComponent<AudioSource>();
            scratchAudioSource.loop = true;
            scratchAudioSource.clip = Resources.Load<AudioClip>("scratch");
            scratchAudioSource.Play();
        }
        int roundsSoFar = 0;

        while (IsSpeakingOrPlaying() || roundsSoFar < minimumNumberOfRounds)
        {
            // forward
            for (int i = 1; i < lr.positionCount && !shouldCancel; i++)
            {
                if (!IsSpeakingOrPlaying() && roundsSoFar >= minimumNumberOfRounds) return;
                await MoveToLineIndex(i);
            }
            // and backwards
            for (int i = lr.positionCount - 2; i >= 0 && !shouldCancel; i--)
            {
                if (!IsSpeakingOrPlaying() && roundsSoFar >= minimumNumberOfRounds) return;
                await MoveToLineIndex(i);
            }
            roundsSoFar++;
        }

        if (scratchAudioSource) scratchAudioSource.Stop();
    }

    public override void CancelIntro()
    {
        CancelAudio();
        Free(onUpper);
        shouldCancel = true;
    }

    public override void SetVisualization(bool visible)
    {
        GetComponent<LineRenderer>().enabled = visible;
    }

    private async Task MoveToLineIndex(int index)
    {
        Vector3 position = lr.GetPosition(index);
        GameObject newTarget = new GameObject("temporary waypoint for PantoIntroBackAndForth");
        newTarget.transform.position = lr.useWorldSpace ? position : transform.TransformPoint(position);
        await SwitchTo(newTarget, onUpper, moveSpeed);
        Destroy(newTarget);
    }

}
