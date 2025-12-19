using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    [Range(0, 5)]
    public int health;

    [SerializeField]
    [Range(0, 10)]
    public int enemyLevel;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float speed = 0.2f;

    private Vector2 roomCenter;
    private Vector2 roomSize;

    private float lastAttackTime = -Mathf.Infinity;
    private float attackCooldown = 8.0f; // in seconds

    private RogueManager rogueManager;

    private RogueAudioManager rogueAudioManager;

    private UpperHandle meHandle;

    private Vector3 lastPlayerPosition = Vector3.zero;

    private LowerHandle lowerHandle;

    private bool playerInRange = false;

    private float lastEnemyPresent = -Mathf.Infinity;
    private float enemyPresentCooldown = 2.0f;

    private float volumeFactor = 0.1f;

    //Room mesurements
    private float roomMaxX;
    private float roomMinX;
    private float roomMaxZ;
    private float roomMinZ;
    private float roomTolerance = 0.1f;


    void Start()
    {
        rogueManager = FindObjectOfType<RogueManager>();
        rogueAudioManager = FindObjectOfType<RogueAudioManager>();
        if (rogueManager == null)
        {
            Debug.LogWarning("RogueManager not found in scene.");
        }
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        
        // initialize room bounds from serialized values
        UpdateRoomBounds();
    }

    void Update()
    {
        if (meHandle == null)
        {
            meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        }
        else
        {
            // make sure roomSize is valid before moving
            if (roomSize.x <= 0f || roomSize.y <= 0f)
            {
                return;
            }
            MoveEnemy();
        }
    }

    void OnValidate()
    {
        UpdateRoomBounds();
    }
    

    //collision detection with player
    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Enemy collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("MeHandle") && Time.time - lastAttackTime > attackCooldown)
        {
            HitPlayer();
            lastAttackTime = Time.time;
        }
    }


    // calculate room bounds based on center and size
    public void SetRoomBounds(Vector2 center, Vector2 size)
    {
        roomCenter = center;
        roomSize = size;
        UpdateRoomBounds();
    }

    // update min/max bounds based on center and size of the room
    private void UpdateRoomBounds()
    {
        // Only compute if we have a sensible room size
        if (roomSize.x <= 0f || roomSize.y <= 0f)
            return;

        float halfX = roomSize.x * 0.5f;
        float halfZ = roomSize.y * 0.5f;

        roomMaxX = roomCenter.x + halfX;
        roomMinX = roomCenter.x - halfX;
        roomMaxZ = roomCenter.y + halfZ;
        roomMinZ = roomCenter.y - halfZ;
    }

    // move enemy towards player, if player is in range
    private void MoveEnemy()
    {
        if (meHandle == null) return;

        if (!CheckPlayerInRange())
        {
            return; // Player is out of range
        }

        if (Vector3.Distance(meHandle.GetPosition(), lastPlayerPosition) < 0.03f)
        {
            lastPlayerPosition = meHandle.GetPosition();
            return; // Player is not moving
        }
        lastPlayerPosition = meHandle.GetPosition();

        Vector3 direction = (lastPlayerPosition - transform.position).normalized;
        float dt = Time.deltaTime;

        Vector3 nextPos = transform.position + direction * speed * dt;
        if (nextPos.x < roomMinX || nextPos.x > roomMaxX || nextPos.z < roomMinZ || nextPos.z > roomMaxZ)
        {
            // Out of bounds, do not move
            return;
        }
        transform.position = nextPos;
    }

    // check if player is within room bounds plus tolerance
    bool CheckPlayerInRange()
    {

        Vector3 playerPosLocal = meHandle.GetPosition();
        if (playerPosLocal.x >= roomMinX - roomTolerance && playerPosLocal.x <= roomMaxX + roomTolerance &&
            playerPosLocal.z >= roomMinZ - roomTolerance && playerPosLocal.z <= roomMaxZ + roomTolerance)
        {
            PlayEnemyIsPresentSound(playerPosLocal);
            if (playerInRange == false)
            {
                lowerHandle.SwitchTo(this.gameObject, 100.0f);
                playerInRange = true;
            }
            return true;
        }
        playerInRange = false;
        return false;
    }
    
    // play sound indicating enemy is present (volume based on distance to player)
    void PlayEnemyIsPresentSound(Vector3 playerPos)
    {
        if (Time.time - lastEnemyPresent > enemyPresentCooldown)
        {
            var XvolumeFactor = 1.0f - (Vector3.Distance(this.transform.position, playerPos));
            rogueAudioManager.PlayEnemyPresentSound(XvolumeFactor);
            lastEnemyPresent = Time.time;
        }
    }
    
    // enemy attacks player
    void HitPlayer()
    {
        if (rogueManager != null)
        {
            // roll 1..20 inclusive for attack (original rogue like system)
            int attackRoll = Random.Range(1, 21);
            if (attackRoll >= (rogueManager.playerAC + ((0 - enemyLevel) + 10) + 1))
            {
                rogueManager.PlayerHit();
            }
        }
    }

    // enemy takes damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy took damage, current health: " + health);
        if (health <= 0)
        {
            rogueAudioManager.PlayEnemyDeathSound();
            Destroy(this.gameObject);
        }
        rogueAudioManager.PlayEnemyHitSound();
    }

    
}
