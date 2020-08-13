using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualPantoFramework;
using UnityEngine;

namespace LegendOfPanto
{
    public class Link : MonoBehaviour
    {
        public AudioClip nightmareSoundLink1;
        public AudioClip nightmareSoundLink2;
        public AudioClip nightmareLaughGanon;
        UpperHandle upperHandle;
        bool dreaming = false;
        bool userControlled = false;
        int direction = 1;
        bool dressed = false;
        public Manager manager;

        void Awake()
        {
            upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        }

        public async Task Nightmare()
        {
            await upperHandle.SwitchTo(gameObject, 0.3f);
            dreaming = true;
            await manager.playSound(nightmareLaughGanon);
            await manager.playSound(nightmareSoundLink1);
            await manager.playSound(nightmareSoundLink2);
            dreaming = false;
        }

        void TossAndTurn()
        {
            transform.RotateAround(transform.position, Vector3.up, 90 * Time.deltaTime * direction);
            float rot = transform.rotation.eulerAngles.y;
            if (rot > 40 && rot < 320) direction *= -1;
        }

        public void Activate()
        {
            userControlled = true;
        }

        public void Free()
        {
            upperHandle.Free();
        }

        public void Stop()
        {
            //upperHandle.StopMovement();
        }

        void Update()
        {
            if (dreaming) TossAndTurn();
            if (userControlled) transform.position = upperHandle.HandlePosition(transform.position);
        }

        public void GetDressed()
        {
            if (dressed) return;
            dressed = true;
        }
    }
}
