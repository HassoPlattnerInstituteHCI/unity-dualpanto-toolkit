using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PantoParallelIntros : PantoIntroBase
{
    [Space]
    [Header("This Intro simply runs all Intros in child objects in parallel and continues when all have finished.")]
    [Space]
    public bool findChildIntrosRecursively;
    

    public override async Task Introduce()
    {
        List<PantoIntroBase> childIntros = new List<PantoIntroBase>();
        foreach (PantoIntroBase childIntro in GetComponentsInChildren<PantoIntroBase>())
        {
            if (childIntro == this) continue; //ignore self
            if (findChildIntrosRecursively || childIntro.transform.parent == transform) // if not recursive, only use direct children
            {
                childIntros.Add(childIntro);
            }
        }
        if (childIntros.Count < 2)
        {
            Debug.LogWarning("PantoParallelIntros should have at least 2 child intros to run in parallel");
        }

        List<Task> introTasks = new List<Task>();
        foreach (PantoIntroBase childIntro in childIntros)
        {
            introTasks.Add(childIntro.Introduce());
        }
        await Task.WhenAll(introTasks);
    }
}
