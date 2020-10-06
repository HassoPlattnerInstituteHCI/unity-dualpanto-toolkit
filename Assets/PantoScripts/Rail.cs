using UnityEngine;

namespace DualPantoFramework
{
    public class Rail : PantoBoxCollider
    {
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
    }
}