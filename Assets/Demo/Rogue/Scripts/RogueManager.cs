using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueManager : MonoBehaviour
{

    public int playerHealth = 5;
    public int playerAC = 3;

    private RogueAudioManager rogueAudioManager;

    void Start()
    {
        rogueAudioManager = GetComponent<RogueAudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlayerHit()
    {
        playerHealth--;
        Debug.Log("Player Health: " + playerHealth);
        rogueAudioManager.PlayPlayerHitSound();
        if (playerHealth <= 0)
        {
            Debug.Log("Player has died!");
            rogueAudioManager.PlayPlayerDeathSound();
            // Handle player death (e.g., end game, respawn, etc.)
        }
    }
}
