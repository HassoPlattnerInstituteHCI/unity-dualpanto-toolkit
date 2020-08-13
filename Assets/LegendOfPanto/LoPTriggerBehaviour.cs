using UnityEngine;

abstract public class LoPTriggerBehaviour : MonoBehaviour
{
    public Manager manager;
    protected abstract void LinkEntered();
    protected void LinkExited() { return; }

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

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Link")
        {
            LinkExited();
        }
    }
}