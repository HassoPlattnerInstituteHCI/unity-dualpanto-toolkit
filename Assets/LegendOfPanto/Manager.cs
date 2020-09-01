using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualPantoFramework;
using PathCreation;
using SpeechIO;
using UnityEngine;
namespace LegendOfPanto
{
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
        LowerHandle lowerHandle;
        AudioSource source;
        SpeechOut speech;
        public GameState gameState = GameState.INTRO;
        public AudioClip bossDie;
        public AudioClip bossClear;
        int direction = 1;
        SpeechIn speechIn;
        void onRecognized(string result) { }
        async void Start()
        {
            link = GameObject.Find("Link").GetComponent<Link>();
            navi = GameObject.Find("Navi").GetComponent<Navi>();
            speech = new SpeechOut();
            speechIn = new SpeechIn(onRecognized);
            source = GetComponent<AudioSource>();
            lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
            if (gameState == GameState.INTRO) Intro();
            //link.Activate();
            //if (gameState == GameState.TARGET) StartShootingLoop();
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

        public void NaviStruggle()
        {
            transform.RotateAround(transform.position, Vector3.up, 90 * Time.deltaTime * direction);
            float rot = transform.rotation.eulerAngles.y;
            if (rot > 40 && rot < 320) direction *= -1;
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

        public async Task NaviFollowPath(string name)
        {
            await navi.FollowPath(GameObject.Find(name).GetComponent<PathCreator>());
        }

        public async Task NaviMoveTo(GameObject go)
        {
            await lowerHandle.SwitchTo(go, 0.2f);
        }

        public async void WinBossFight()
        {
            //door.remove();
            gameState = GameState.END;
            //musicPlayer.stop();
            await playSound(bossDie);
            await playSound(bossClear);
            await NaviSpeak("Oh Link thanks for saving me. That was pretty close. Come, let us go talk to the old man.");
            StopLink();
            await NaviFollowPath("CaveToOldMan");
            FreeLink();
        }

        public async void StartShootingLoop()
        {
            while (gameState == GameState.TARGET || gameState == GameState.MONSTER_FIGHT)
            {
                Dictionary<string, KeyCode> myDict = new Dictionary<string, KeyCode> { { "shoot", KeyCode.S } };
                string msg = await speechIn.Listen(myDict);
                if (msg == "shoot")
                {
                    link.Shoot();
                }
            }
        }
    }
}
