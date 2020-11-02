using System.Collections.Generic;
using UnityEngine;

namespace DualPantoFramework
{
    public class ColliderRegistry
    {
        static List<PantoCollider> ColliderList = new List<PantoCollider>();

        public static void RegisterObstacles()
        {
            foreach (PantoCollider collider in ColliderList)
            {
                collider.CreateObstacle();
                if (collider.IsEnabled()) collider.Enable();
            }
        }

        public static void AddCollider(PantoCollider collider)
        {
            ColliderList.Add(collider);
        }

        public static void RemoveCollider(PantoCollider collider)
        {
            ColliderList.RemoveAll((c) => c.GetId() == collider.GetId());
        }
    }
}