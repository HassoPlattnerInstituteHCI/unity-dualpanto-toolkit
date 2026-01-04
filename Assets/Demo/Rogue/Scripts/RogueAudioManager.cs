using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueAudioManager : MonoBehaviour
{
    public AudioClip playerHitSound;

    public AudioClip enemyHitSound;

    public AudioClip enemyPresentSound;

    public AudioClip enemyDeathSound;

    public AudioClip playerDeathSound;

    public AudioClip itemPickupSound;

    public float defaultVolume = 1f;


    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();

    }

    public void PlayPlayerHitSound()
    {
        if (playerHitSound != null)
        {
            audioSource.PlayOneShot(playerHitSound);
        }
    }
    public void PlayEnemyHitSound()
    {
        if (enemyHitSound != null)
        {
            audioSource.PlayOneShot(enemyHitSound);
        }
    }
    public void PlayEnemyDeathSound()
    {
        if (enemyDeathSound != null)
        {
            audioSource.PlayOneShot(enemyDeathSound);
        }
    }
    public void PlayPlayerDeathSound()
    {
        if (playerDeathSound != null)
        {
            audioSource.PlayOneShot(playerDeathSound);
        }
    }
    public void PlayItemPickupSound()
    {
        if (itemPickupSound != null)
        {
            audioSource.PlayOneShot(itemPickupSound);
        }
    }
    public void PlayEnemyPresentSound(float volume)
    {
        if (enemyPresentSound != null)
        {
            audioSource.PlayOneShot(enemyPresentSound, volume);
        }
    }
}