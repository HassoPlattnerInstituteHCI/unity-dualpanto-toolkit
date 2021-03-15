using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;

public class PantoIntroTracer : PantoIntroBase
{
    public bool onUpper;
    public float moveSpeed = 3f;
    public string[] sayOnWayToPoint;
    public string[] sayWhileStoppingAtPoint;

    private LineRenderer lr;
    private SpeechOut speechOut;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        speechOut = new SpeechOut();
        if (lr.positionCount < sayOnWayToPoint.Length || lr.positionCount < sayWhileStoppingAtPoint.Length)
            Debug.LogWarning("There are more voice lines than path vertices");
    }

    public override async Task Introduce()
    {
        for (int i = 0; i < lr.positionCount; i++)
        {
            // move to point and optionally say something
            Vector3 position = lr.GetPosition(i);
            GameObject newTarget = new GameObject("temporary waypoint for PantoIntroTracer");
            newTarget.transform.position = lr.useWorldSpace ? position : transform.TransformPoint(position);

            if (i < sayOnWayToPoint.Length && sayOnWayToPoint[i] != "")
            {
                await Task.WhenAll(
                    speechOut.Speak(sayOnWayToPoint[i]),
                    getHandle(onUpper).SwitchTo(newTarget, moveSpeed)
                    );
            }
            else
            {
                await getHandle(onUpper).SwitchTo(newTarget, moveSpeed);
            }
            Destroy(newTarget);

            // optionally wait at point to say something
            if (i < sayWhileStoppingAtPoint.Length && sayWhileStoppingAtPoint[i] != "")
            {
                await speechOut.Speak(sayWhileStoppingAtPoint[i]);
            }
        }
    }

    public override void SetVisualization(bool visible)
    {
        GetComponent<LineRenderer>().enabled = visible;
    }
}
