using UnityEngine;

namespace DualPantoToolkit
{
    public class PantoLineCollider : PantoCollider
    {
        public Vector2 start;
        public Vector2 end;
        public override void CreateObstacle()
        {
            UpdateId();
            CreateLineObstacle(start, end);
        }
    }
}