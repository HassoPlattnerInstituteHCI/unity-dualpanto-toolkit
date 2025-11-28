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

    public Vector2 roomCenter;
    public Vector2 roomSize;

    private float lastAttackTime = -Mathf.Infinity;
    private float attackCooldown = 4.0f; // in seconds

    private RogueManager rogueManager;

    private UpperHandle meHandle;

    private Vector3 lastPlayerPosition = Vector3.zero;

    private LowerHandle lowerHandle;

    private bool playerInRange = false;


    //Room mesurements
    private float roomMaxX;
    private float roomMinX;
    private float roomMaxZ;
    private float roomMinZ;
    private float roomTolerance = 0.1f;


    void Start()
    {
        rogueManager = FindObjectOfType<RogueManager>();
        if (rogueManager == null)
        {
            Debug.LogWarning("RogueManager not found in scene.");
        }
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        if(lowerHandle != null)
        {
            //lowerHandle.SwitchTo(this.gameObject, 100.0f);
        }
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
        if (collision.gameObject.CompareTag("Player") && Time.time - lastAttackTime > attackCooldown)
        {
            hitPlayer();
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
    

    private void MoveEnemy()
    {
        if (meHandle == null) return;

        if (!checkPlayerInRange())
        {
            return; // Player is out of range
        }

        if (Vector3.Distance(meHandle.GetPosition(), lastPlayerPosition) < 0.01f)
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

    bool checkPlayerInRange()
    {
        
        Vector3 playerPosLocal = meHandle.GetPosition();
        //Vector3 playerPosLocal = this.transform.InverseTransformPoint(playerPos);
        if (playerPosLocal.x >= roomMinX-roomTolerance && playerPosLocal.x <= roomMaxX + roomTolerance &&
            playerPosLocal.z >= roomMinZ-roomTolerance && playerPosLocal.z <= roomMaxZ + roomTolerance)
        {
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

    void hitPlayer()
    {
        if (rogueManager != null)
        {
            // roll 1..20 inclusive
            int attackRoll = Random.Range(1, 21);
            if (attackRoll >= (rogueManager.playerAC + ((0 - enemyLevel) + 10) + 1))
            {
                rogueManager.PlayerHit();
            }
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy took damage, current health: " + health);
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    
}
