using UnityEngine;

abstract public class LoPTriggerBehaviour : MonoBehaviour
{
    public Manager manager;
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