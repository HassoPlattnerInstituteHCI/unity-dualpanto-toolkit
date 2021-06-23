using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(LineRenderer))]
public class PantoIntroTracer : PantoIntroWithAudio
{

    public bool onUpper;
    public float moveSpeed = 3f;
    [Tooltip("The first point is the start of the arrow-line.")]
    public AudioClipOrText[] audioOnWayToPoint;
    public AudioClipOrText[] audioWhileStoppingAtPoint;
    public bool playScratchSound = true;

    private LineRenderer lr;
    private AudioSource scratchAudioSource;
    private bool shouldCancel = false;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        if (lr.positionCount < audioOnWayToPoint.Length || lr.positionCount < audioWhileStoppingAtPoint.Length)
            Debug.LogWarning("There are more audios than path vertices");
    }

    public override async Task Introduce()
    {
        for (int i = 0; i < lr.positionCount && !shouldCancel; i++)
        {
            if (i == 1 && playScratchSound && !scratchAudioSource)
            {
                scratchAudioSource = gameObject.AddComponent<AudioSource>();
                scratchAudioSource.loop = true;
                scratchAudioSource.clip = Resources.Load<AudioClip>("scratch");
                scratchAudioSource.Play();
            }
            // move to point and optionally say something
            Vector3 position = lr.GetPosition(i);
            GameObject newTarget = new GameObject("temporary waypoint for PantoIntroTracer");
            newTarget.transform.position = lr.useWorldSpace ? position : transform.TransformPoint(position);

            if (i < audioOnWayToPoint.Length && audioOnWayToPoint[i].IsSpecified())
            {
                await Task.WhenAll(
                    PlayAudioOrSpeak(audioOnWayToPoint[i]),
                    SwitchTo(newTarget, onUpper, moveSpeed)
                    );
            }
            else
            {
                await SwitchTo(newTarget, onUpper, moveSpeed);
            }
            Destroy(newTarget);

            // optionally wait at point to say something
            if (i < audioWhileStoppingAtPoint.Length && audioWhileStoppingAtPoint[i].IsSpecified())
            {
                await PlayAudioOrSpeak(audioWhileStoppingAtPoint[i]);
            }
        }
        if (scratchAudioSource)
        {
            scratchAudioSource.Stop();
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
        GetComponent<LineRenderer>().enabled = visible;
    }
}
