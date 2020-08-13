using UnityEngine;
using DualPantoFramework;
using PathCreation.Examples;
using System.Threading.Tasks;

public class Follower : MonoBehaviour
{
    PantoHandle lowerHandle;
    async void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        await lowerHandle.SwitchTo(gameObject, 0.2f);
        await follow();
        lowerHandle.Free();
    }

    async Task follow()
    {
        PathFollower follower = GetComponent<PathFollower>();
        follower.StartFollowing();
        while (follower.following)
        {
            await Task.Delay(10);
        }
    }
}
