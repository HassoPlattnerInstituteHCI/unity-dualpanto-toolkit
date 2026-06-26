using UnityEngine;
using System.Collections.Generic;

namespace DualPantoToolkit
{
    /// <summary>
    /// Singleton class to manage sound effects in the game. Use SoundManager.Instance.Play("soundName") to play a sound effect by name. Add sound entries in the Unity Inspector by specifying a name, an AudioClip, and an optional volume. The SoundManager will persist across scene loads.
    /// </summary>

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [System.Serializable]
        public class SoundEntry
        {
            public string name;
            public AudioClip clip;

            [Range(0f, 1f)]
            public float volume = 1.0f;

            public SoundEntry(string name, AudioClip clip, float volume = 1.0f)
            {
                this.name = name;
                this.clip = clip;
                this.volume = volume;
            }
        }

        public List<SoundEntry> sounds;

        private AudioSource audioSource;

        void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();

        }

        /// <summary>
        /// Plays a sound effect by name. If the sound is not found, a warning is logged.
        /// </summary>
        /// <param name="name"></param>
        public void Play(string name)
        {
            SoundEntry soundEntry = sounds.Find(s => s.name == name);
            if (soundEntry != null)
            {
                audioSource.PlayOneShot(soundEntry.clip, soundEntry.volume);
            }
            else
            {
                Debug.LogWarning("Sound not found: " + name);
            }
        }

        /// <summary>
        /// Plays a sound effect by name with a specified volume. If the sound is not found, a warning is logged.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        public void Play(string name, float volume)
        {
            SoundEntry soundEntry = sounds.Find(s => s.name == name);
            if (soundEntry != null)
            {
                audioSource.PlayOneShot(soundEntry.clip, volume);
            }
            else
            {
                Debug.LogWarning("Sound not found: " + name);
            }
        }

        /// <summary>
        /// Plays a sound effect directly from an AudioClip. If the clip is null, a warning is logged.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        public void Play(AudioClip clip, float volume = 1.0f)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }
            else
            {
                Debug.LogWarning("AudioClip is null");
            }
        }
    }
}