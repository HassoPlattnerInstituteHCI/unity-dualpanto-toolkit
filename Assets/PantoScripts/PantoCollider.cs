using System;
using UnityEngine;

namespace DualPantoFramework
{
    public abstract class PantoCollider : PantoBehaviour
    {
        protected ushort id;
        protected bool pantoEnabled = false;
        public bool onUpper = true;
        public bool onLower = true;
        public bool isPassable = false;
        private bool registered = false;

        public ushort GetId()
        {
            return id;
        }

        public bool IsEnabled()
        {
            return pantoEnabled;
        }
        protected byte getPantoIndex()
        {
            if (onUpper && onLower) return 0xff;
            if (onUpper) return 0;
            if (onLower) return 1;
            return 2;
        }

        protected Vector2[] CornersFromRotatedRectangle(Vector3 center, float angle, Vector2 dimensions)
        {
            angle *= -Mathf.Deg2Rad;

            Vector2 v1 = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 v2 = new Vector2(-v1.y, v1.x);
            v1 *= (dimensions.x / 2);
            v2 *= (dimensions.y / 2);

            Vector2 center2 = new Vector2(center.x, center.z);
            return new Vector2[] {
            center2 + v1 + v2,
            center2 - v1 + v2,
            center2 - v1 - v2,
            center2 + v1 - v2,
        };
        }

        protected Vector2[] CornersFromBounds(Bounds bounds)
        {
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            Vector2 topRight = new Vector2(center.x + size.x / 2, center.z + size.z / 2);
            Vector2 bottomRight = new Vector2(center.x + size.x / 2, center.z - size.z / 2);
            Vector2 bottomLeft = new Vector2(center.x - size.x / 2, center.z - size.z / 2);
            Vector2 topLeft = new Vector2(center.x - size.x / 2, center.z + size.z / 2);
            return new Vector2[] { topRight, bottomRight, bottomLeft, topLeft };
        }

        /// <summary>
        /// Registers the obstacle on the Panto, the shape depends on its type. Don't forget to call Enable()
        /// </summary>
        public abstract void CreateObstacle();

        protected void UpdateId()
        {
            id = pantoSync.GetNextObstacleId();
            if (!registered)
            {
                registered = true;
                ColliderRegistry.AddCollider(this);
            }
        }

        protected void CreateBoxObstacle()
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            Vector2 size = new Vector2(collider.size.x * transform.localScale.x, collider.size.z * transform.localScale.z);
            CreateFromCorners(CornersFromRotatedRectangle(transform.position, transform.eulerAngles.y, size));
        }

        protected void CreateCircularCollider(int numberOfCorners)
        {
            Vector3 center = GetComponent<SphereCollider>().center + transform.position;
            Vector3 radius = GetComponent<SphereCollider>().radius * transform.lossyScale;

            Vector2[] corners = new Vector2[numberOfCorners];
            for (var i = 0; i < numberOfCorners; i++)
            {
                float angle = i * Mathf.PI * 2 / numberOfCorners;
                float x = Mathf.Cos(angle) * radius.x;
                float z = Mathf.Sin(angle) * radius.z;
                corners[i] = new Vector2(x + transform.position.x, z + transform.position.z);
            }
            CreateFromCorners(corners);
        }

        public void CreateFromCorners(Vector2[] corners)
        {
            byte index = getPantoIndex();
            if (index == 2)
            {
                Debug.LogWarning("[DualPanto] Skipping creation for object with no handles");
            }
            pantoSync.CreateObstacle(index, id, corners[0], corners[1]);
            for (int i = 1; i < corners.Length - 1; i++)
            {
                pantoSync.AddToObstacle(index, id, corners[i], corners[i + 1]);
            }
            pantoSync.AddToObstacle(index, id, corners[corners.Length - 1], corners[0]);
        }

        /// <summar>
        /// Disables the obstacle
        /// </summary>
        public void Disable()
        {
            pantoEnabled = false;
            GetPantoSync().DisableObstacle(getPantoIndex(), id);
        }

        /// <summar>
        /// Removes the obstacle.
        /// </summary>
        public void Remove()
        {
            GetPantoSync().RemoveObstacle(getPantoIndex(), id);
            pantoEnabled = false;
        }

        /// <summar>
        /// Enables the obstacle. This needs to be called after creating the obstacle.
        /// </summary>
        public void Enable()
        {
            pantoEnabled = true;
            GetPantoSync().EnableObstacle(getPantoIndex(), id);
        }
    }
}