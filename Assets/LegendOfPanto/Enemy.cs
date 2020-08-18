using UnityEngine;

namespace LegendOfPanto
{
    public class Enemy : MonoBehaviour
    {
        public AudioClip enemyHit;
        public int health = 3;
        bool isActive = true;
        Manager manager;

        void Start()
        {
            manager = GameObject.Find("Game").GetComponent<Manager>();
        }

        void Update()
        {
            //Follow();
        }

        public async void TakeDamage()
        {
            health -= 1;
            if (health <= 0)
            {
                manager.WinBossFight();
                Destroy(gameObject);
            }
            else
            {
                await manager.playSound(enemyHit);
            }
        }

        //void Attack()
        //{
        //if (this.player.takeDamage())
        //{
        //this.setActive(false);
        //this.player.gameOver();
        //return true;
        //}
        //return false;
        //}

        //void Follow()
        //{
        //const target = this.player.getMePosition();
        //if (!target) return;
        //// Get the direction vector to the player.
        //let dir = target.difference(this.position);
        //// Get the travel distance to the player.
        //const len = Math.min(dir.length(), this.moveSpeed);
        //dir = dir.normalized();
        //// Add the scaled direction vector to the current position.
        //this.position.add(dir.scaled(len));
        //// Set the it-handle to the new position.
        //this.player.device.movePantoTo(1, this.position, this.pantoSpeed)
        //.then(() =>
        //{
        //// Check if the enemy is in range of the player.
        //if (target.difference(this.position).length() <= this.player.radius)
        //{
        //VoiceInteraction.playSound("./sounds/OOT_YoungLink/OOT_YoungLink_Hurt2.wav");
        //// Attack the player.
        //// const gameOver = this.attack();
        //// Give the player a little knockback.
        //this.player.device.movePantoTo(0, target.sum(dir.scale(this.force)), this.pantoSpeed)
        //.then(() =>
        //{
        //this.player.device.unblockHandle(0);
        //})
        //}
        //});
        //}
    }
}