using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : PantoBehaviour
{
    AudioSource audioSource;
    SpeechOut speechOut = new SpeechOut();
    protected override void Awake()
    {
        base.Awake();
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        //audioSource = GetComponent<AudioSource>();
    }
    public IEnumerator playIntroduction()
    {
        ObjectOfInterest[] gos = UnityEngine.Object.FindObjectsOfType<ObjectOfInterest>();
        Array.Sort(gos, ((go1, go2) => go2.priority.CompareTo(go1.priority)));

        for (int index = 0; index < gos.Length; index++)
        {
            yield return introduceObject(gos[index]);
        }
    }

    public IEnumerator introduceObject(ObjectOfInterest objectOfInterest)
    {
        //audioSource.clip = objectOfInterest.introductionSound;
        //audioSource.Play();
        speechOut.Speak(objectOfInterest.description);

        PantoHandle pantoHandle = objectOfInterest.isOnUpper
            ? (PantoHandle)GetPantoGameObject().GetComponent<UpperHandle>()
            : (PantoHandle)GetPantoGameObject().GetComponent<LowerHandle>();

        if (objectOfInterest.traceShape)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in objectOfInterest.transform)
            {
                children.Add(child.gameObject);
            }
            children.Sort((GameObject g1, GameObject g2) => g1.name.CompareTo(g2.name));
            yield return pantoHandle.TraceObjectByPoints(children, 0.2f);
        }
        else
        {
            yield return pantoHandle.SwitchTo(objectOfInterest.gameObject, 0.2f);
        }
        GetPantoGameObject().GetComponent<LowerHandle>().Free();
        GetPantoGameObject().GetComponent<UpperHandle>().Free();
    }
}

