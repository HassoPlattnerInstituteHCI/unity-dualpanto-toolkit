using System.Collections;
using System.Collections.Generic;
using DualPantoFramework;
using UnityEngine;

public class Manager : MonoBehaviour
{
    Link link;
    Navi navi;
    UpperHandle upperHandle;
    void Start()
    {
        link = GameObject.Find("Link").GetComponent<Link>();
        navi = GameObject.Find("Navi").GetComponent<Navi>();
        Intro();
    }

    async void Intro()
    {
        await link.Nightmare();
        await navi.WakeLink();
        link.Activate();
    }

    public async void OnGetDressed()
    {
        await navi.ShowToDoor();
    }

    public async void OnTryExitUndressed() { await navi.BerateLink(); }
    public async void OnExitDoor() { await navi.ShowToOldMan(); }
}
