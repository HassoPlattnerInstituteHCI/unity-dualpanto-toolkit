using SpeechIO;
using UnityEngine;

namespace DualPantoFramework
{
    public class Rail : PantoBoxCollider
    {
        public string text;
        
        public Rail()
        {
            isPassable = true;
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

        void OnCollisionEnter(Collision col)
        {
            // When target is hit
            if (col.gameObject.name == "MeHandle")
            {
                PantoManager pantoManager = GameObject.Find("PantoManager").GetComponent<PantoManager>();
                pantoManager.speechOut.Speak(text);

            }
        }
    }
}