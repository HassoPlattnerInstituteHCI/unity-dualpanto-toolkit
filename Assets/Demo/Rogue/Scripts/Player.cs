using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private float lastAttackTime = -Mathf.Infinity;
    private float attackCooldown = 3.0f; // in seconds
    private RogueManager rogueManager;

    void Start()
    {
        rogueManager = FindObjectOfType<RogueManager>();
        if (rogueManager == null)
            Debug.LogWarning("RogueManager not found in scene.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Player collided with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy") && Time.time - lastAttackTime > attackCooldown)
        {
            Debug.Log("Attacking enemy: " + collision.gameObject.name);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hitEnemy(collision.gameObject.GetComponent<Enemy>());
                lastAttackTime = Time.time;
            }
        }
        if (collision.gameObject.CompareTag("Food"))
        {
            heal();
            Destroy(collision.gameObject);
        }
    }
    
    private void heal()
    {
        rogueManager.playerHealth++;
    }
    
    public void hitEnemy(Enemy enemyScript)
    {
        if (enemyScript != null)
        {
            enemyScript.TakeDamage(1);
        }
    }
    
}
