using UnityEngine;
using DualPantoToolkit;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace DualPantoToolkit
{

    public class RoomEchoManager : MonoBehaviour
    {
        private class EchoRoomData
        {
            public Collider roomCollider;
            public float width;
            public float length;
            public float height = 1.0f;
            public float volume;

            // Acoustic properties
            float reverbDecayTime;
            float reverbReflectionsLevel;
            float reverbLevel;
            float decayHFRatio;
            float hfReference;
            float lowpassCutoff;

            public float absorption;
            public float reverbTime;
            public float echoDelay;
            public float diffusion;
            public float wetLevel;
            public float dryLevel;

            // Computes acoustic parameters based on room dimensions and absorption
            public void ComputeAcousticParameters(Collider col, float absorption, float wetLevel, float echoDelayMs = 0f)
            {
                this.roomCollider = col;
                this.absorption = absorption;
                this.wetLevel = wetLevel;

                // Calculate room dimensions
                var sizeV3 = col.bounds.size;
                this.width = sizeV3.x;
                this.length = sizeV3.z;
                this.volume = width * length * height;

                // Sabine formula approximation: RT60 = 0.161 * (V / A)
                float surfaceArea = 2f * (width * length + width * height + length * height);
                float A = Mathf.Max(0.0001f, absorption * surfaceArea);
                float sabineRT = 0.161f * (volume / A);

                // Scale RT with room size
                float linearSize = Mathf.Pow(Mathf.Max(0.001f, volume), 1f / 3f);
                float sizeNorm = Mathf.Clamp01(Mathf.InverseLerp(2f, 20f, linearSize));

                // Small rooms get shorter RT, large rooms amplified RT
                this.reverbTime = Mathf.Lerp(0.08f, Mathf.Max(0.5f, sabineRT * 1.0f), sizeNorm);
                this.reverbDecayTime = Mathf.Clamp(this.reverbTime, 0.05f, 30f);

                this.reverbReflectionsLevel = Mathf.Lerp(-200f, -4000f, sizeNorm) + Mathf.Lerp(0f, -1000f, absorption);
                this.reverbLevel = Mathf.Lerp(-7000f, -200f, Mathf.Clamp01(sizeNorm * 1.2f));

                // Calculate narrowness factor (1 = very narrow, 0 = square)
                float minDim = Mathf.Min(width, length);
                float maxDim = Mathf.Max(width, length);
                float horizontalRatio = (maxDim > 0f) ? (minDim / maxDim) : 1f;
                float narrowFactor = 1f - Mathf.Clamp01(horizontalRatio);

                // High-frequency attenuation for narrow/small rooms
                float baseCutoff = Mathf.Lerp(1200f, 9000f, 1f - Mathf.Clamp01(this.absorption));
                this.lowpassCutoff = Mathf.Clamp(baseCutoff * Mathf.Lerp(0.25f, 1f, Mathf.Clamp01(horizontalRatio)), 150f, 9000f);

                this.decayHFRatio = Mathf.Clamp(Mathf.Lerp(0.25f, 1.4f, Mathf.Clamp01(sizeNorm + (1f - this.absorption) * 0.5f)), 0.2f, 1.5f);
                this.hfReference = this.lowpassCutoff;

                // Echo delay calculation
                if (echoDelayMs > 1f)
                {
                    this.echoDelay = Mathf.Max(0.001f, echoDelayMs / 1000f);
                }
                else
                {
                    this.echoDelay = 2f * Mathf.Min(width, length, height) / 343f;
                }
            }

            public void ApplyToAudioSource(AudioSource source)
            {
                AudioEchoFilter echo = source.GetComponent<AudioEchoFilter>();
                if (echo != null)
                {
                    echo.delay = Mathf.Clamp(this.echoDelay * 1000f, 1f, 5000f);
                    echo.decayRatio = Mathf.Clamp01(0.5f);
                    echo.wetMix = Mathf.Clamp01(this.wetLevel);
                }

                AudioReverbFilter reverb = source.GetComponent<AudioReverbFilter>();
                if (reverb != null)
                {
                    reverb.decayTime = this.reverbDecayTime;
                    reverb.reflectionsLevel = this.reverbReflectionsLevel;
                    reverb.reflectionsDelay = Mathf.Clamp(this.echoDelay, 0.001f, 0.5f);
                    reverb.reverbLevel = this.reverbLevel;
                    reverb.reverbDelay = Mathf.Clamp(this.echoDelay * 0.5f, 0.02f, 1f);

                    reverb.decayHFRatio = Mathf.Clamp(this.decayHFRatio, 0.2f, 1.5f);
                    reverb.hfReference = Mathf.Clamp(this.hfReference, 100f, 20000f);

                    reverb.room = Mathf.Lerp(-8000f, 0f, Mathf.Clamp01(1f - this.absorption));

                    float roomHFMilliBel = Mathf.Lerp(-12000f, 0f, Mathf.InverseLerp(200f, 8000f, this.lowpassCutoff));
                    reverb.roomHF = Mathf.Clamp(roomHFMilliBel, -12000f, 0f);

                    reverb.density = Mathf.Clamp01(Mathf.Lerp(0.6f, 1f, this.diffusion));
                    reverb.diffusion = Mathf.Clamp01(Mathf.Lerp(0.5f, 1f, this.diffusion));
                }
            }

        }

        public AudioClip walkSound;
        public bool onMeHandle = true;
        public bool onItHandle = false;
        public bool applyEcho = true;

        [SerializeField]
        [Range(0f, 1f)]
        private float volume = 1f;

        [SerializeField]
        [Range(0f, 20f)]
        private float pitch = 1f;

        [SerializeField]
        [Range(0f, 2000f)]
        private float echoDelay = 500f;

        [SerializeField]
        [Range(0f, 1f)]
        private float absorption = 0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float wetLevel = 0.5f;

        [SerializeField]
        [Description("Sensitivity for detecting movement. Lower values make it more sensitive.")]
        [Range(0f, 1f)]
        private float movementDetection = 1f;

        private Vector3 lastPositionItHandle;
        private Vector3 lastPositionMeHandle;
        private Collider[] childColliders;
        private List<EchoRoomData> echoRooms = new List<EchoRoomData>();
        private AudioSource audioSource;

        void Awake()
        {
            this.audioSource = gameObject.AddComponent<AudioSource>();
            if (audioSource != null)
                audioSource.volume = volume;
            audioSource.clip = walkSound;
            audioSource.gameObject.AddComponent<AudioEchoFilter>();
            audioSource.gameObject.AddComponent<AudioReverbFilter>();

            childColliders = GetComponentsInChildren<Collider>();
            configureAudioRooms();
            
        }

        void Start()
        {
            this.lastPositionMeHandle = Vector3.zero;
            this.lastPositionItHandle = Vector3.zero;
        }

        void Update()
        {
            bool handleChangedDetected = false;
            var handleDistance = 0.0f;

            if (childColliders == null || childColliders.Length == 0) return;

            for (int i = 0; i < childColliders.Length; i++)
            {
                var col = childColliders[i];
                if (col == null || !col.enabled) continue;

                Collider[] hits = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, ~0, QueryTriggerInteraction.Collide);

                if (hits.Any(h => (h.CompareTag("MeHandle") && onMeHandle)))
                {
                    Vector3 handlePos = GameObject.Find("Panto").GetComponent<UpperHandle>().GetPosition();
                    handleDistance = UnityEngine.Vector3.Distance(handlePos, lastPositionMeHandle);

                    if (handleDistance > movementDetection)
                    {
                        lastPositionMeHandle = handlePos;
                        handleChangedDetected = true;
                    }
                }
                if (hits.Any(h => (h.CompareTag("ItHandle") && onItHandle)))
                {
                    Vector3 handlePos = GameObject.Find("Panto").GetComponent<LowerHandle>().GetPosition();
                    handleDistance = UnityEngine.Vector3.Distance(handlePos, lastPositionItHandle);

                    if (handleDistance > movementDetection)
                    {
                        lastPositionItHandle = handlePos;
                        handleChangedDetected = true;
                    }
                }
            }

            if (handleChangedDetected)
            {
                PlayWalkSound(handleDistance * pitch);
            }
        }

        void OnValidate()
        {
            if (audioSource != null)
                audioSource.volume = volume;
        }


        async void PlayWalkSound(float speed = 1)
        {
            audioSource.pitch = speed;
            if (audioSource != null && walkSound != null && !audioSource.isPlaying)
            {
                if (!applyEcho)
                {
                    audioSource.Play();
                }
                else
                {
                    // Find the room containing the handle
                    foreach (var room in echoRooms)
                    {
                        if (room.roomCollider.bounds.Contains(lastPositionMeHandle) || room.roomCollider.bounds.Contains(lastPositionItHandle))
                        {
                            room.ApplyToAudioSource(audioSource);
                            break;
                        }
                    }
                    audioSource.Play();
                }
            }
        }

        void configureAudioRooms()
        {
            foreach (var col in childColliders)
            {
                EchoRoomData room = new EchoRoomData();
                room.ComputeAcousticParameters(col, this.absorption, this.wetLevel, this.echoDelay);
                echoRooms.Add(room);
            }
        }
    }
}