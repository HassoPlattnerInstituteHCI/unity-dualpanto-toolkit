using UnityEngine;
using ClipperLib;
using System.Linq;
using System.Collections.Generic;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace DualPantoToolkit
{
    /// <summary>
    /// Generates a 2D PantoCollider (in the xz plane) from the meshes of the GameObject
    /// this script is attached to. Every MeshFilter found on the object or its children
    /// (one per component of the 3D model) is projected onto the xz plane and the union of
    /// all projected triangles forms the obstacle outline.
    /// </summary>
    public class PantoMeshCollider : PantoCollider
    {
        // Clipper works on integers, so world coordinates are scaled by this factor before clipping.
        private const float Scale = 1000f;

        // Distance (in world units) below which nearby outline points are merged, reducing the
        // number of segments sent to the Panto. Set to 0 to disable simplification.
        public float simplificationTolerance = 0.005f;

        public override void CreateObstacle()
        {
            UpdateId();
            CreateMeshObstacle();
        }

        private IntPoint IntPointFromVector2(Vector2 vector)
        {
            return new IntPoint(Mathf.RoundToInt(vector.x * Scale), Mathf.RoundToInt(vector.y * Scale));
        }

        private Vector2[] Vector2ArrayFromPath(Path path)
        {
            // Mirrors PantoCompoundCollider: duplicate the last point so CreateFromCorners closes
            // the loop cleanly.
            Vector2[] value = new Vector2[path.Count + 1];
            for (int i = 0; i < path.Count; i++)
            {
                value[i] = new Vector2(path[i].X / Scale, path[i].Y / Scale);
            }
            value[path.Count] = value[path.Count - 1];
            return value;
        }

        /// <summary>
        /// Collects the world-space, xz-projected triangles of every MeshFilter on this object
        /// and its children, unions them, and creates the obstacle from the resulting outlines.
        /// </summary>
        public void CreateMeshObstacle()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length == 0)
            {
                Debug.LogWarning("[DualPanto] PantoMeshCollider found no MeshFilter to build a collider from");
                return;
            }

            Paths triangles = new Paths();
            foreach (MeshFilter meshFilter in meshFilters)
            {
                Mesh mesh = meshFilter.sharedMesh;
                if (mesh == null) continue;

                Vector3[] vertices = mesh.vertices;
                int[] meshTriangles = mesh.triangles;
                Transform t = meshFilter.transform;

                for (int i = 0; i < meshTriangles.Length; i += 3)
                {
                    Vector3 a = t.TransformPoint(vertices[meshTriangles[i]]);
                    Vector3 b = t.TransformPoint(vertices[meshTriangles[i + 1]]);
                    Vector3 c = t.TransformPoint(vertices[meshTriangles[i + 2]]);

                    Path triangle = new Path(3)
                    {
                        IntPointFromVector2(new Vector2(a.x, a.z)),
                        IntPointFromVector2(new Vector2(b.x, b.z)),
                        IntPointFromVector2(new Vector2(c.x, c.z))
                    };

                    // Skip triangles that project to zero area (e.g. vertical faces seen edge-on).
                    if (Clipper.Area(triangle) == 0) continue;

                    // Orient every triangle consistently so the NonZero union fills overlaps correctly.
                    if (!Clipper.Orientation(triangle))
                    {
                        triangle.Reverse();
                    }

                    triangles.Add(triangle);
                }
            }

            if (triangles.Count == 0)
            {
                Debug.LogWarning("[DualPanto] PantoMeshCollider could not project any triangles onto the xz plane");
                return;
            }

            Paths solution = new Paths();
            Clipper clipper = new Clipper();
            clipper.AddPaths(triangles, PolyType.ptSubject, true);
            clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            if (simplificationTolerance > 0f)
            {
                solution = Clipper.CleanPolygons(solution, simplificationTolerance * Scale);
            }

            List<Vector2[]> cornersList = solution
                .Where(s => s.Count >= 3)
                .Select(s => Vector2ArrayFromPath(s))
                .ToList();

            if (cornersList.Count == 0)
            {
                Debug.LogWarning("[DualPanto] PantoMeshCollider produced an empty outline");
                return;
            }

            CreateFromCorners(cornersList);
        }
    }
}