using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DualPantoToolkit
{
    /// <summary>
    /// Singleton scene manager to handle scene loading and unloading.
    /// </summary>

    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Instance { get; private set; }

        public int currentSceneIndex => UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        [Tooltip("If true, all PantoColliders found after loading a scene will be enabled automatically (like the ObstacleManager does on Start).")]
        public bool enableCollidersOnLoad = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            Invoke(nameof(CreatePantoColliders), 1.0f);
        }

        private void CreatePantoColliders()
        {
            PantoCollider[] pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in pantoColliders)
            {
                collider.CreateObstacle();
                if (enableCollidersOnLoad)
                {
                    collider.Enable();
                }
                else
                {
                    collider.Disable();
                }
            }
        }

        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public void LoadScene(int sceneIndex)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }

        public void NextScene()
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            int nextIndex = (currentSceneIndex + 1) % sceneCount;
            LoadScene(nextIndex);
        }

        public void PreviousScene()
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            int previousIndex = (currentSceneIndex - 1 + sceneCount) % sceneCount;
            LoadScene(previousIndex);
        }

        private void DeactivateAllPantoColliders()
        {
            PantoCollider[] pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in pantoColliders)
            {
                collider.Disable();
            }
        }
    }
}