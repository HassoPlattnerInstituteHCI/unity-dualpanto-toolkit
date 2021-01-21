using DualPantoFramework;
namespace UnityEngine {


    public class RailPolyline : ColliderPolyline
    {
        public string[] texts;
        public float displacement = 0.3f;
        public RailPolyline(){

            this.gizmoColor = Color.cyan;
        }
        private void CreateObstacle(int i)
        {
            Rail r = this.gameObject.AddComponent<Rail>();
            if (i < texts.Length)
            {
                r.text = texts[i];
            }

            r.CreateObstacle(points[i], points[i + 1], displacement);
            r.Enable();
        }

    }

}