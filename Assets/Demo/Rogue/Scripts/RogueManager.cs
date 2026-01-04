using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;

public class RogueManager : MonoBehaviour
{

    [SerializeField]
    [Range(0, 10)]
    public int playerHealth = 5;

    [SerializeField]
    [Range(0, 10)]
    public int playerAC = 3;

    [SerializeField]
    public Transform spawnPosition;

    private RogueAudioManager rogueAudioManager;

    private UpperHandle upperHandle;

    void Start()
    {
        rogueAudioManager = GetComponent<RogueAudioManager>();
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        Introduction();
    }

    void Introduction()
    {
        TransformPlayerToSpawn();
    }

    public void LevelFinished(){
        UnityEditor.EditorApplication.isPlaying = false;
    }

    async void TransformPlayerToSpawn()
    {
        if (upperHandle != null)
        {
            await upperHandle.MoveToPosition(spawnPosition.position, 1f);
        }
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
