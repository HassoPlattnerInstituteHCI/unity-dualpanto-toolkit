using UnityEngine;

namespace DualPantoToolkit
{
    public class PantoPolygonCollider : PantoCollider
    {
        public override void CreateObstacle()
        {
            UpdateId();
            CreatePolygonObstacle();
        }
    }
}