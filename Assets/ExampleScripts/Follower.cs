using UnityEngine;
using DualPantoFramework;
using PathCreation.Examples;

public class Follower : MonoBehaviour
{
    PantoHandle lowerHandle;
    async void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        await lowerHandle.SwitchTo(gameObject, 0.2f);
        GetComponent<PathFollower>().StartFollowing();
    }
}
