using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;

public class PantoIntroBackAndForth : PantoIntroBase
{

    public bool onUpper;
    public float moveSpeed = 3f;
    public string sayOnWayToFirstPoint;
    public string sayWhileGoingBackAndForth;
    [Tooltip("When 0, this will stop after the text has been spoken, still finishing the current round.")]
    public int minimumNumberOfRounds;


    private LineRenderer lr;
    private SpeechOut speechOut;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        speechOut = new SpeechOut();
        if (sayWhileGoingBackAndForth == "" && minimumNumberOfRounds == 0)
            Debug.LogWarning("Either text needs to be specified or minimumNumberOfRounds needs to be above 0.");
    }

    public override async Task Introduce()
    {
        // go to first point
        if (sayOnWayToFirstPoint != "")
        {
            await Task.WhenAll(
                speechOut.Speak(sayOnWayToFirstPoint),
                MoveToLineIndex(0)
            );
        } else
        {
            await MoveToLineIndex(0);
        }

        // begin rounds
        _ = speechOut.Speak(sayWhileGoingBackAndForth);
        int roundsSoFar = 0;
        while (speechOut.IsSpeaking() || roundsSoFar < minimumNumberOfRounds)
        {
            // forward
            for (int i = 1; i < lr.positionCount; i++)
            {
                if (!speechOut.IsSpeaking() && roundsSoFar >= minimumNumberOfRounds) return;
                await MoveToLineIndex(i);
            }
            // and backwards
            for (int i = lr.positionCount - 2; i >= 0; i--)
            {
                if (!speechOut.IsSpeaking() && roundsSoFar >= minimumNumberOfRounds) return;
                await MoveToLineIndex(i);
            }
            roundsSoFar++;
        }
        
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
        await getHandle(onUpper).SwitchTo(newTarget, moveSpeed);
        Destroy(newTarget);
    }

}
