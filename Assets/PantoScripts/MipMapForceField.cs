using UnityEngine;

namespace DualPantoFramework
{
    class MipMapForceField : ForceField
    {
        public Texture2D heightmap;

        Color GetPixel(Vector3 position)
        {
            Vector3 relative = transform.InverseTransformPoint(position) * heightmap.width + new Vector3(heightmap.width / 2, 0, heightmap.height / 2);
            int x = Mathf.FloorToInt(relative.x);
            int z = Mathf.FloorToInt(relative.z);
            return heightmap.GetPixel(x, z);
        }

        protected override float GetCurrentStrength(Collider other)
        {
            float red = GetPixel(other.transform.position).r;
            float green = GetPixel(other.transform.position).g;
            float transparency = GetPixel(other.transform.position).a;
            return (red + green) * transparency;
        }

        protected override Vector3 GetCurrentForce(Collider other)
        {
            float red = GetPixel(other.transform.position).r;
            float green = GetPixel(other.transform.position).g;
            return ((red * Vector3.forward) + (green * Vector3.right)).normalized;
        }
    }
}