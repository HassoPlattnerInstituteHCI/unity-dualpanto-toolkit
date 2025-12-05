using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    
    public GameObject food;

    public GameObject roomPrefab;

    public GameObject enemyPrefab;

    [SerializeField]
    [Range(0, 100)]
    public int enemySpawnProbability = 50; // percentage chance to spawn an enemy in a room

    private List<GameObject> rooms = new List<GameObject>();



    async Task Start()
    {
        await Task.Delay(3000);
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
            int roomId = 1;
            foreach (GameObject room in rooms)
            {
                // add food to each room
                AddFoodToRoom(room);
                if (!room.GetComponent<Room>().isSpawnRoom)
                {
                    // add enemy based on probability
                    if (Random.Range(0, 100) < enemySpawnProbability)
                    {
                        AddEnemyToRoom(room);
                    }
                    // add intro speech to non-spawn rooms
                    room.GetComponent<Room>().AddIntroRoomSpeech(roomId);
                    roomId++;
                }
                else
                {
                    // This is the spawn room
                    string introductionText = "Hello Player! This is your starting room. Find food to heal and avoid enemies!. There are " + (rooms.Count - 1) + " rooms to explore. Good luck!";
                    room.GetComponent<Room>().AddIntroRoomSpeech(roomId, introductionText);
                    var roomSpeech = room.GetComponent<RoomSpeechOnEntry>();
                    if (roomSpeech != null)
                    {
                        //after first visit, change the introduction text
                        roomSpeech.triggerAfterSpeechEnd = true;
                        UnityAction changeTextAction = () =>
                        {
                            room.GetComponent<Room>().ChangeIntroductionText("This is the starting room again. Good luck exploring the dungeon!");
                        };
                        roomSpeech.onEnter.AddListener(changeTextAction);
                    }
                }

            }
        }
    }

    // add food item to the room at a random position
    void AddFoodToRoom(GameObject room)
    {
        var (xPos, zPos, roomSize) = GetRandomPositionInRoom(room);
        Vector3 foodPosition = new Vector3(xPos, room.transform.position.y + 0.5f, zPos);

        Instantiate(food, foodPosition, Quaternion.identity);
    }

    // add an enemy to the room at a random position
    void AddEnemyToRoom(GameObject room)
    {
        var (xPos, zPos, roomSize) = GetRandomPositionInRoom(room);
        Vector3 enemyPosition = new Vector3(xPos, room.transform.position.y + 0.5f, zPos);

        var enemyGO = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        var enemy = enemyGO.GetComponent<Enemy>();
        enemy.SetRoomBounds(new Vector2(room.transform.position.x, room.transform.position.z),
                new Vector2(roomSize.x, roomSize.z));
    }

    // calculate a random position within the room bounds
    (float posX, float posZ, Vector3 roomSize) GetRandomPositionInRoom(GameObject room)
    {
        Vector3 roomSize = room.GetComponent<Renderer>().bounds.size;
        Vector3 roomPosition = room.transform.position;

        float xPos = Random.Range(roomPosition.x - roomSize.x / 2 + 0.5f, roomPosition.x + roomSize.x / 2 - 0.5f);
        float zPos = Random.Range(roomPosition.z - roomSize.z / 2 + 0.5f, roomPosition.z + roomSize.z / 2 - 0.5f);

        return (xPos, zPos, roomSize);
    }

    // find all rooms by checking child objects with "Room" tag
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
