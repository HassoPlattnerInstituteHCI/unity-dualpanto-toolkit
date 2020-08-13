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
    public GameState gameState = GameState.INTRO;

    void Start()
    {
        link = GameObject.Find("Link").GetComponent<Link>();
        navi = GameObject.Find("Navi").GetComponent<Navi>();
        speech = new SpeechOut();
        source = GetComponent<AudioSource>();
        Intro();
    }

    public void playSoundLooping(AudioClip clip)
    {
        source.clip = clip;
        source.loop = true;
        source.Play();
    }
    public void stopSoundLooping()
    {
        source.Stop();
    }

    public async Task playSound(AudioClip clip)
    {
        source.clip = clip;
        source.loop = false;
        source.Play();
        await Task.Delay(Mathf.RoundToInt(clip.length * 1000));
    }

    public void FreeLink()
    {
        link.Free();
    }

    public void StopLink()
    {
        link.Stop();
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
    async public Task GanonSpeak(string text)
    {
        await speech.Speak(text);
    }


    async void Intro()
    {
        await link.Nightmare();
        await navi.WakeLink();
        link.Activate();
        gameState = GameState.DRESS;
    }

    public async void DresserEntered()
    {
        link.GetDressed();
        gameState = GameState.DOOR;
        await navi.ShowToDoor();
    }

    public async void OnTryExitUndressed()
    {
        StopLink();
        await navi.BerateLink();
        FreeLink();
    }
    public async void OnExitDoor()
    {
        gameState = GameState.OLDMAN_INTRO;
        await navi.ShowToOldMan();
    }

    public async void ShowLinkToRiver()
    {
        gameState = GameState.RIVER;
        await NaviSpeak("Ok Link let's go! Come with me to the river.");
    }
}
