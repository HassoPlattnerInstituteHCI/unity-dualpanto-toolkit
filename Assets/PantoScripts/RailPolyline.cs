using UnityEngine;

namespace DualPantoFramework
{
    public class RailPolyline : MonoBehaviour
    {
        public Vector2[] points;
        public string[] texts;
        public float displacement = 0.3f;

        public void CreateRails()
        {
            for (int i = 0; i < points.Length - 1; i++)
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
        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < points.Length - 1; i++)
            {
                Gizmos.DrawLine(new Vector3(points[i].x, 5, points[i].y), new Vector3(points[i + 1].x, 5, points[i + 1].y));
            }

        }
    }
}