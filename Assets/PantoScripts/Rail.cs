using SpeechIO;
using UnityEngine;

namespace DualPantoFramework
{
    public class Rail : PantoBoxCollider
    {
        public string text = "";

        public Rail()
        {
            isPassable = true;
        }

        public Rail(string text) : this()
        {
            this.text = text;
        }

        public override void CreateObstacle()
        {
            UpdateId();
            CreateRail();
        }
        public void CreateObstacle(Vector2 start, Vector2 end, float displacement)
        {
            UpdateId();
            CreateRailForLine(start, end, displacement);
        }

        void OnTriggerEnter(Collider collider)
        {
            // When target is hit
            if (collider.tag != "Player") return;
            PantoManager pantoManager = GameObject.Find("PantoManager").GetComponent<PantoManager>();
            if (pantoManager != null)
                pantoManager.speechOut.Speak(text);
        }
    }
}