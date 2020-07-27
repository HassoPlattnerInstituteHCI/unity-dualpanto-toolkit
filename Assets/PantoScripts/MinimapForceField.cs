using UnityEngine;

namespace DualPantoFramework
{
    class MinimapForceField : ForceField
    {
        public Texture2D heightmap;
        protected override float GetCurrentStrength(Collider other)
        {
            Vector3 relative = transform.InverseTransformPoint(other.transform.position) * heightmap.width + new Vector3(heightmap.width / 2, 0, heightmap.height / 2);
            int x = Mathf.FloorToInt(relative.x);
            int z = Mathf.FloorToInt(relative.z);
            float red = heightmap.GetPixel(x, z).r;
            return red;
        }

        protected override Vector3 GetCurrentForce(Collider other)
        {
            return new Vector3(1, 0, 0);
        }

    }
}