using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;
using UnityEngine;

public enum GameState
{
    DEFAULT,
    INTRO,
    DRESS,
    DOOR,
    OLDMAN_INTRO,
    RIVER,
    FIND_BOW,
    OLDMAN_TARGET,
    TARGET,
    OLDMAN_QUEST,
    MONSTER_INTRO,
    MONSTER_FIGHT,
    END
}
public class Manager : MonoBehaviour
{
    Link link;
    Navi navi;
    UpperHandle upperHandle;
    AudioSource source;
    SpeechOut speech;
    public GameState gameState = GameState.DEFAULT;

    void Start()
    {
        link = GameObject.Find("Link").GetComponent<Link>();
        navi = GameObject.Find("Navi").GetComponent<Navi>();
        speech = new SpeechOut();
        source = GetComponent<AudioSource>();
        Intro();
    }

    public async Task playSound(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
        await Task.Delay(Mathf.RoundToInt(clip.length * 1000));
    }

    async public Task Speak(string text)
    {
        await speech.Speak(text);
    }
    async public Task OldManSpeak(string text)
    {
        await speech.Speak(text);
    }
    async public Task NaviSpeak(string text)
    {
        await speech.Speak(text);
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
