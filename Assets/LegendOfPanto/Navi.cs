using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using DualPantoFramework;
using System.Threading.Tasks;
using PathCreation.Examples;
using PathCreation;

namespace LegendOfPanto
{
    public class Navi : MonoBehaviour
    {
        public Manager manager;
        public AudioClip flySound;
        LowerHandle lowerHandle;
        void Start()
        {
            lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        }

        public async Task WakeLink()
        {
            await Task.WhenAll(manager.playSound(flySound), lowerHandle.SwitchTo(GameObject.Find("Door"), 0.2f));
            await manager.NaviSpeak("I wonder if he's still sleeping.");

            await Task.WhenAll(
                manager.NaviSpeak("Hey Link wake up. It's okay you were just having a nightmare. I am Navi, your guardian fairy."),
            lowerHandle.SwitchTo(GameObject.Find("BedPosition"), 0.2f)
            );
            await GameObject.Find("Panto").GetComponent<Level>().PlayIntroduction();
            await Task.WhenAll(
                manager.NaviSpeak("Come over here first. Before we leave you have to get dressed."),
                lowerHandle.SwitchTo(GameObject.Find("Dresser"), 0.2f)
            );
        }

        public async Task FollowPath(PathCreator path)
        {
            gameObject.transform.position = path.bezierPath.GetPoint(0);
            await lowerHandle.SwitchTo(gameObject, 0.2f);
            PathFollower follower = GetComponent<PathFollower>();
            follower.pathCreator = path;
            follower.StartFollowing();
            while (follower.following)
            {
                await Task.Delay(10);
            }
        }
    }
}
