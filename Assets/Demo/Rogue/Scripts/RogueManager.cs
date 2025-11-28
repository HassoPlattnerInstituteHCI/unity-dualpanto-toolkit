using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueManager : MonoBehaviour
{

    public int playerHealth = 5;
    public int playerAC = 3;

    public AudioClip hitSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlayerHit()
    {
        playerHealth--;
        Debug.Log("Player Health: " + playerHealth);
        if(hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}
