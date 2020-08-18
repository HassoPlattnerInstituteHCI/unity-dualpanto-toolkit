using UnityEngine;

namespace LegendOfPanto
{
    abstract public class LoPTriggerBehaviour : MonoBehaviour
    {
        protected Manager manager;
        protected abstract void LinkEntered();

        void Awake()
        {
            manager = GameObject.Find("Game").GetComponent<Manager>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Link")
            {
                LinkEntered();
            }
        }
    }
}