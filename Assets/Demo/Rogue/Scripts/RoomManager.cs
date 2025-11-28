using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject food;

    public GameObject roomPrefab;

    public GameObject enemyPrefab;

    [SerializeField]
    [Range(0, 100)]
    public int enemySpawnProbability = 50; // percentage chance to spawn an enemy in a room

    private List<GameObject> rooms = new List<GameObject>();



    void Start()
    {
        AddRooms();
        Debug.Log("Number of rooms: " + rooms.Count);
        CreateRoomObsticles();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void CreateRoomObsticles()
    {
        if (rooms.Count != 0)
        {
            foreach (GameObject room in rooms)
            {
                AddFoodToRoom(room);
                if (room.GetComponent<Room>().isSpawnRoom == false && Random.Range(0, 100) < enemySpawnProbability)
                {
                    AddEnemyToRoom(room);
                }
            }
        }
    }

    void AddFoodToRoom(GameObject room)
    {
        Vector3 roomSize = room.GetComponent<Renderer>().bounds.size;
        Vector3 roomPosition = room.transform.position;

        float xPos = Random.Range(roomPosition.x - roomSize.x / 2 + 0.5f, roomPosition.x + roomSize.x / 2 - 0.5f);
        float zPos = Random.Range(roomPosition.z - roomSize.z / 2 + 0.5f, roomPosition.z + roomSize.z / 2 - 0.5f);

        Vector3 foodPosition = new Vector3(xPos, roomPosition.y + 0.5f, zPos);

        Instantiate(food, foodPosition, Quaternion.identity);
    }

    void AddEnemyToRoom(GameObject room)
    {
        Vector3 roomSize = room.GetComponent<Renderer>().bounds.size;
        Vector3 roomPosition = room.transform.position;

        float xPos = Random.Range(roomPosition.x - roomSize.x / 2 + 0.5f, roomPosition.x + roomSize.x / 2 - 0.5f);
        float zPos = Random.Range(roomPosition.z - roomSize.z / 2 + 0.5f, roomPosition.z + roomSize.z / 2 - 0.5f);

        Vector3 foodPosition = new Vector3(xPos, roomPosition.y + 0.5f, zPos);

        var enemyGO = Instantiate(enemyPrefab, foodPosition, Quaternion.identity);
        var enemy = enemyGO.GetComponent<Enemy>();
        enemy.SetRoomBounds(new Vector2(roomPosition.x, roomPosition.z),
                new Vector2(roomSize.x, roomSize.z));
    }


    void AddRooms()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("Room"))
            {
                rooms.Add(transform.GetChild(i).gameObject);
            }

        }
    }


    void createRandomRoom()
    {

    }
}
