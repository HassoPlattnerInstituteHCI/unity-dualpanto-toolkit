using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DualPantoFramework;

public class PantoIntroducer : MonoBehaviour
{
    [Tooltip("Note: Unity does not explicitly guarantee an order when iterating through children.")]
    public bool useActiveChildObjects = true;
    public List<PantoIntroBase> introductions;

    public KeyCode keyToStart = KeyCode.None;
    private bool isIntroducing = false;

    private bool cancelIntros = false;

    private void Awake()
    {
        if (useActiveChildObjects)
        {
            introductions = new List<PantoIntroBase>();
            foreach (PantoIntroBase childIntro in GetComponentsInChildren<PantoIntroBase>())
                if (childIntro.transform.parent == transform) introductions.Add(childIntro);
        }
    }

    private void Start()
    {
        HideAllIntroVisuals();
    }

    private async void Update()
    {
        if (Input.GetKeyDown(keyToStart) && !isIntroducing)
        {
            await RunIntros();
        }
        if (Input.GetKeyDown(KeyCode.X) && isIntroducing)
        {
            Debug.Log("Current Intro skipped!");
            cancelIntros = true; // stops any further Sub-intros from being started
        }
    }


    public async Task RunIntros()
    {
        FindObjectOfType<LowerHandle>().Free();
        isIntroducing = true;
        ShowAllIntroVisuals();
        for (int i = 0; i < introductions.Count; i++)
        {
            if (cancelIntros) break;
            Debug.LogWarning(introductions[i].name);
            await introductions[i].Introduce();
        }
        HideAllIntroVisuals();
        FindObjectOfType<UpperHandle>().Free();
        isIntroducing = false;
    }

    public void HideAllIntroVisuals()
    {
        foreach (PantoIntroBase childIntro in GetComponentsInChildren<PantoIntroBase>())
        {
            childIntro.SetVisualization(false);
        }
    }

    public void ShowAllIntroVisuals()
    {
        foreach (PantoIntroBase childIntro in GetComponentsInChildren<PantoIntroBase>())
        {
            childIntro.SetVisualization(true);
        }
    }
}
