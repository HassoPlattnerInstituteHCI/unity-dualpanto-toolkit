using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;

public class FollowMe : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float speed = 0.2f;

    private Vector2 roomCenter = new Vector2(-2.48f, -3.28f);
    private Vector2 roomSize = new Vector2(1.5f, 1.5f);

    private UpperHandle meHandle;

    private Vector3 lastPlayerPosition = Vector3.zero;

    private LowerHandle lowerHandle;

    //Room mesurements
    private float roomMaxX;
    private float roomMinX;
    private float roomMaxZ;
    private float roomMinZ;
    private float roomTolerance = 0.1f;


    void Start()
    {
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
            MoveIt();
        }
    }

    // void OnValidate()
    // {
    //     UpdateRoomBounds();
    // }


    // // calculate room bounds based on center and size
    // public void SetRoomBounds(Vector2 center, Vector2 size)
    // {
    //     roomCenter = center;
    //     roomSize = size;
    //     UpdateRoomBounds();
    // }

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
    private void MoveIt()
    {
        if (meHandle == null) return;

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
    
}
