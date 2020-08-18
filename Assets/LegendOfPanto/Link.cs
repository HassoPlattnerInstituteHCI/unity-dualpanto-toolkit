using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualPantoFramework;
using PathCreation;
using UnityEngine;

namespace LegendOfPanto
{
    public class Link : MonoBehaviour
    {
        public AudioClip nightmareSoundLink1;
        public AudioClip nightmareSoundLink2;
        public AudioClip nightmareLaughGanon;
        public AudioClip shootArrow;
        public AudioClip arrowMissed;
        public AudioClip arrowHitTarget;
        UpperHandle upperHandle;
        bool dreaming = false;
        bool userControlled = false;
        int direction = 1;
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
            if (userControlled) transform.eulerAngles = new Vector3(90, upperHandle.GetRotation(), 0);
        }

        public async void Shoot()
        {
            await manager.playSound(shootArrow);
            RaycastHit hit;
            Quaternion currentRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            if (Physics.SphereCast(transform.position, 1, currentRotation * Vector3.forward, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, currentRotation * Vector3.forward * 100, Color.yellow, 0.5f);
                if (hit.collider.tag == "Enemy")
                {
                    hit.collider.GetComponent<Enemy>().TakeDamage();
                }
                else if (hit.collider.tag == "Target" && manager.gameState == GameState.TARGET)
                {
                    manager.gameState = GameState.OLDMAN_QUEST;
                    await manager.playSound(arrowHitTarget);
                    manager.StopLink();
                    await manager.NaviSpeak("Great you hit the target. Let's get back to the old man and see if he has to say anything.");
                    await manager.NaviFollowPath("TargetToOldMan");
                    manager.FreeLink();
                }
                else
                {
                    await manager.playSound(arrowMissed);
                    await manager.NaviSpeak("You missed!");
                }
            }
        }
    }
}
