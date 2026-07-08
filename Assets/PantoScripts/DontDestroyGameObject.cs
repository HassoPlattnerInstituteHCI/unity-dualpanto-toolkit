using UnityEngine;

namespace DualPantoToolkit
{
    /// <summary>
    /// A simple script to mark a GameObject as "Don't Destroy On Load".
    /// </summary>

    public class DontDestroyGameObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}