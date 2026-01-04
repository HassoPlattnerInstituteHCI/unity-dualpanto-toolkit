using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    public bool isSpawnRoom = false;

    public int countEnemies = 0;

    // add class <RoomSpeechOnEntry> to this room and set introduction text
    public void AddIntroRoomSpeech(int roomId, string introductionText = null)
    {
        var roomSpeech = this.gameObject.AddComponent<RoomSpeechOnEntry>();
        roomSpeech.introductionText = introductionText ?? "You have entered room number " + roomId + ".";
    }
    
    // change the introduction text of the RoomSpeechOnEntry component
    public void ChangeIntroductionText(string newText)
    {
        var roomSpeech = this.gameObject.GetComponent<RoomSpeechOnEntry>();
        if (roomSpeech != null)
        {
            roomSpeech.introductionText = newText;
        }
    }
}
