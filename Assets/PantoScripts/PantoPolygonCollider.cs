using UnityEngine;

namespace DualPantoFramework
{
    public class PantoPolygonCollider : PantoCollider
    {
        public override void CreateObstacle()
        {
            UpdateId();
            CreatePolygonObstacle();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Vector2[] points = GetComponent<PolygonCollider2D>().points;
            Transform parent = this.transform.parent;
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector3 point = transform.TransformPoint(points[i].x, points[i].y, 0);
                Vector3 nextPoint = transform.TransformPoint(points[i + 1].x, points[i + 1].y, 0);
                Gizmos.DrawLine(point, nextPoint);
            }
            // close loop
            Gizmos.DrawLine(
                transform.TransformPoint(points[points.Length - 1].x, points[points.Length - 1].y, 0),
                transform.TransformPoint(points[0].x, points[0].y, 0));
        }
    }
}