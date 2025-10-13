using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoToolkit;
using Task = System.Threading.Tasks.Task;
using SpeechIO;

namespace Rogue
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance; // what's this?
        
        public GameObject player;
        public Transform position;
        private UpperHandle upperHandle;
        private LowerHandle lowerHandle;
        private bool lowerFree = true;
        private bool upperrFree = true;
        private SpeechOut speechOut;
        PantoCollider[] pantoColliders;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();
            Introduction();
        }
        async void Introduction()
        {
            await Task.Delay(1000);
            await StartGame();
        }

        async Task StartGame()
        {
            await RenderObstacle();
            upperHandle.Free();
            lowerHandle.Free();
        }

        async Task RenderObstacle()
        {
            pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in pantoColliders)
            {
                collider.CreateObstacle();
                collider.Enable();
            }
        }
        
        public async Task DestroyObstacle()
        {
            pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in pantoColliders)
            {
                collider.Remove();
            }
        }
        
        void Update()
        {
            CommandForDebug();
        }

        void CommandForDebug()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (PantoCollider collider in pantoColliders)
                {
                    collider.Enable();
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                foreach (PantoCollider collider in pantoColliders)
                {
                    collider.Disable();
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (upperrFree)
                {
                    upperHandle.Freeze();
                }
                else
                {
                    upperHandle.Free();
                }
                upperrFree = !upperrFree;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (lowerFree)
                {
                    lowerHandle.Freeze();
                }
                else
                {
                    lowerHandle.Free();
                }
                lowerFree = !lowerFree;
            }
        }
    }
}

