using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private float lastAttackTime = -Mathf.Infinity;
    private float attackCooldown = 3.0f; // in seconds
    private RogueManager rogueManager;

    private RogueAudioManager rogueAudioManager;

    void Start()
    {
        rogueManager = FindObjectOfType<RogueManager>();
        if (rogueManager == null)
            Debug.LogWarning("RogueManager not found in scene.");
    }

    // Handles collisions with enemies, food items, and finish point
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            rogueManager.LevelFinished();
        }
        // Attack enemy if cooldown has elapsed
        if (collision.gameObject.CompareTag("Enemy") && Time.time - lastAttackTime > attackCooldown)
        {
            Debug.Log("Attacking enemy: " + collision.gameObject.name);
            HitEnemy(collision.gameObject.GetComponent<Enemy>());
            lastAttackTime = Time.time;
        }
        if (collision.gameObject.CompareTag("Food"))
        {
            Heal();

            Destroy(collision.gameObject.transform.parent.gameObject);
        }
    }
     
    private void Heal()
    {
        rogueManager.playerHealth++;
    }
    
    public void HitEnemy(Enemy enemyScript)
    {
        if (enemyScript != null)
        {
            enemyScript.TakeDamage(1);
        }
    }
    
}
