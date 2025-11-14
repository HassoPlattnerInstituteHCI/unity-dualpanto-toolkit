using UnityEngine;
using DualPantoToolkit;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Data;




namespace DualPantoToolkit
{

    public class RoomEchoManager : MonoBehaviour
    {
        // Start is called before the first frame update
        private class EchoRoomData
        {
            public Collider roomCollider;

            //public Vector3 center;
            public float width;
            public float length;
            public float height = 1.0f;
            //public float area;
            public float volume;
            //public float aspectRatio;
            //public List<Vector2> polygon; // Grundriss

            // Akustische Eigenschaften
            AudioEchoFilter echo;
            //AudioReverbFilter reverb = new AudioReverbFilter();

            //Reverb settings
            float reverbDecayTime;
            float reverbReflectionsLevel;
            float reverbLevel;
            float decayHFRatio;
            float hfReference;
            float lowpassCutoff;



            public float absorption;   // [0..1]
            public float reverbTime;   // Sekunden
            public float echoDelay;    // Sekunden
            public float diffusion;    // [0..1]
            public float wetLevel;     // [0..1]
            public float dryLevel;     // [0..1]

            // Dynamische Werte
            //public bool isPlayerInside;
            //public float distanceToPlayer;
            //public float occlusion;



            // --- Berechnung von abgeleiteten Werten ---
            // echoDelay parameter (optional) is interpreted as milliseconds if > 0.
            // Internally we store echoDelay in seconds.
            public void ComputeAcousticParameters(Collider col, float absorption, float wetLevel, float echoDelayMs = 0f)
            {

                //Boxcollider of Room
                this.roomCollider = col;


                this.absorption = absorption;
                this.wetLevel = wetLevel;

                //calculate room dimensions
                var sizeV3 = col.bounds.size;
                this.width = sizeV3.x;
                this.length = sizeV3.z;
                this.volume = width * length * height;


                // Beispielhafte Näherung über Sabine-Formel:
                // RT60 = 0.161 * (V / A), wobei A = α * Oberfläche
                float surfaceArea = 2f * (width * length + width * height + length * height);
                float A = Mathf.Max(0.0001f, absorption * surfaceArea);
                float sabineRT = 0.161f * (volume / A);

                // Skaliere RT mit Raumlinearmaß, damit sehr kleine Räume deutlich kürzer klingen
                // Verwende kubikwurzel(Volume) als charakteristische Raumgröße (in m)
                float linearSize = Mathf.Pow(Mathf.Max(0.001f, volume), 1f / 3f);
                // Map linearSize in [2m..20m] auf [0..1] für Interpolation
                float sizeNorm = Mathf.Clamp01(Mathf.InverseLerp(2f, 20f, linearSize));

                // kombiniere Sabine-RT und heuristische Größe: small rooms -> much shorter RT; large rooms -> amplified RT
                this.reverbTime = Mathf.Lerp(0.08f, Mathf.Max(0.5f, sabineRT * 1.0f), sizeNorm);
                this.reverbDecayTime = Mathf.Clamp(this.reverbTime, 0.05f, 30f);

                // Early reflections level (in mB): kleine Räume -> relativ starke frühe Reflexionen (perceptual)
                // Map sizeNorm so that small rooms (sizeNorm ~0) get near 0..-400 mB; large rooms get more negative (weaker early reflections)
                this.reverbReflectionsLevel = Mathf.Lerp(-200f, -4000f, sizeNorm) + Mathf.Lerp(0f, -1000f, absorption);

                // Late reverb (reverb level), louder for larger RT60/rooms
                this.reverbLevel = Mathf.Lerp(-7000f, -200f, Mathf.Clamp01(sizeNorm * 1.2f));

                // Determine narrowness: small ratio -> narrow room (e.g. corridor)
                float minDim = Mathf.Min(width, length);
                float maxDim = Mathf.Max(width, length);
                float horizontalRatio = (maxDim > 0f) ? (minDim / maxDim) : 1f; // 0..1

                // narrowFactor: 1 = very narrow, 0 = square/open
                float narrowFactor = 1f - Mathf.Clamp01(horizontalRatio);

                // High-frequency behavior: combine absorption and narrowness to produce stronger HF attenuation in small/narrow rooms
                float baseCutoff = Mathf.Lerp(1200f, 9000f, 1f - Mathf.Clamp01(this.absorption));
                // Narrow rooms (horizontalRatio small) reduce cutoff strongly
                this.lowpassCutoff = Mathf.Clamp(baseCutoff * Mathf.Lerp(0.25f, 1f, Mathf.Clamp01(horizontalRatio)), 150f, 9000f);

                // decayHFRatio: <1 means HF decays faster than LF -> muffled
                // Make small rooms and high absorption produce lower decayHFRatio
                this.decayHFRatio = Mathf.Clamp(Mathf.Lerp(0.25f, 1.4f, Mathf.Clamp01(sizeNorm + (1f - this.absorption) * 0.5f)), 0.2f, 1.5f);

                // hfReference set to lowpass cutoff to express characteristic HF
                this.hfReference = this.lowpassCutoff;

                // Echo delay (erste erkennbare Reflexion) in Sekunden:
                if (echoDelayMs > 1f)
                {
                    // caller supplied a value in milliseconds -> convert to seconds
                    this.echoDelay = Mathf.Max(0.001f, echoDelayMs / 1000f);
                }
                else
                {
                    // estimate from the smallest room dimension (path to nearest wall and back)
                    this.echoDelay = 2f * Mathf.Min(width, length, height) / 343f;
                }
            }

            public void ApplyToAudioSource(AudioSource source)
            {
                // Beispielhafte Anpassung
                //source.volume = Mathf.Lerp(0.7f, 1.0f, 1f - absorption);
                AudioEchoFilter echo = source.GetComponent<AudioEchoFilter>();
                if (echo != null)
                {
                    // AudioEchoFilter.delay is in milliseconds
                    echo.delay = Mathf.Clamp(this.echoDelay * 1000f, 1f, 5000f);
                    echo.decayRatio = Mathf.Clamp01(0.5f);
                    echo.wetMix = Mathf.Clamp01(this.wetLevel);
                }

                // Configure AudioReverbFilter so it conveys room size and muffling WITHOUT using a separate low-pass.
                AudioReverbFilter reverb = source.GetComponent<AudioReverbFilter>();
                if (reverb != null)
                {
                    // basic timing/level
                    reverb.decayTime = this.reverbDecayTime;
                    reverb.reflectionsLevel = this.reverbReflectionsLevel;
                    reverb.reflectionsDelay = Mathf.Clamp(this.echoDelay, 0.001f, 0.5f);
                    reverb.reverbLevel = this.reverbLevel;
                    reverb.reverbDelay = Mathf.Clamp(this.echoDelay * 0.5f, 0.02f, 1f);

                    // HF behaviour: control high-frequency decay and reference frequency
                    reverb.decayHFRatio = Mathf.Clamp(this.decayHFRatio, 0.2f, 1.5f);
                    reverb.hfReference = Mathf.Clamp(this.hfReference, 100f, 20000f);

                    // room and roomHF control overall room coloration; map absorption/narrowness to roomHF
                    // room: overall room effect (mB like -10000..0) -> smaller rooms slightly quieter
                    reverb.room = Mathf.Lerp(-8000f, 0f, Mathf.Clamp01(1f - this.absorption));

                    // roomHF: negative = more high-frequency attenuation -> muffled
                    // Map lowpassCutoff (200..8000 Hz) to roomHF (-12000 .. 0 mB)
                    float roomHFMilliBel = Mathf.Lerp(-12000f, 0f, Mathf.InverseLerp(200f, 8000f, this.lowpassCutoff));
                    reverb.roomHF = Mathf.Clamp(roomHFMilliBel, -12000f, 0f);

                    // diffusion/density control tail texture
                    reverb.density = Mathf.Clamp01(Mathf.Lerp(0.6f, 1f, this.diffusion));
                    reverb.diffusion = Mathf.Clamp01(Mathf.Lerp(0.5f, 1f, this.diffusion));
                }

                //source.reverbZoneMix = wetLevel;
                //source.spatialBlend = 1f; // 3D Sound
                                          // Wenn du kein Unity-Audio nutzt: sende Parameter an externes Audio-System
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

        Collider[] childColliders;

        List<EchoRoomData> echoRooms = new List<EchoRoomData>();

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
            // Prüfe alle Raum-Collider und entscheide danach, ob Sound gespielt werden soll.
            bool handleChangedDetected = false;
            var handleDistance = 0.0f;

            if (childColliders == null || childColliders.Length == 0) return;

            for (int i = 0; i < childColliders.Length; i++)
            {
                var col = childColliders[i];
                if (col == null || !col.enabled) continue;

                // ÜberlapBox mit QueryTriggerInteraction.Collide, damit Trigger-Collider (z.B. Handles) gefunden werden
                Collider[] hits = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, ~0, QueryTriggerInteraction.Collide);

                Debug.Log("Checking collider: " + col.name + " with " + hits.Length + " hits.");

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
            else
            {
                //audioSource?.Stop();
            }
        }

        void OnValidate()
        {
            if (audioSource != null)
                audioSource.volume = volume;
        }

        void OnTriggerStay(Collider other)
        {
            //UnityEngine.Debug.Log("Trigger Stay detected with " + other.tag);

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
                    // Finde den Raum, in dem sich der Handle befindet
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
                //audioSource.loop = true;
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

        // public void CreateCompoundObstacle()
        // {
        //     Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        //     Collider coll = colliders[0];

        //     Paths solution = new Paths();
        //     solution.Add(PathFromBounds(colliders[0].bounds));

        //     for (int i = 1; i < colliders.Length; i++)
        //     {
        //         Paths newPath = new Paths(1);
        //         newPath.Add(PathFromBounds(colliders[i].bounds));

        //         Clipper c = new Clipper();
        //         c.AddPaths(solution, PolyType.ptSubject, true);
        //         c.AddPaths(newPath, PolyType.ptClip, true);
        //         c.Execute(ClipType.ctUnion, solution);
        //     }
        //     CreateFromCorners(Vector2ArrayFromPath(solution[0]));
        // }

        // void playRoomEchoSound()
        // {
        //     if (audioSource != null && walkSound != null && !audioSource.isPlaying)
        //     {
        //         audioSource.clip = walkSound;
        //         audioSource.loop = true;
        //         audioSource.Play();
        //     }
        // }

    }
}