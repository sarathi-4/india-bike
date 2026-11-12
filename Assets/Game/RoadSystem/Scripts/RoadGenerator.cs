using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.RoadSystem
{
    /// <summary>
    /// Procedurally extrudes 3D road geometry from a RoadSpline.
    /// Creates optimized road meshes and mesh colliders with physical surface properties.
    /// </summary>
    [RequireComponent(typeof(RoadSpline))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class RoadGenerator : MonoBehaviour
    {
        [Header("Mesh Resolution")]
        [SerializeField] private int _segmentsPerCurve = 30;
        [SerializeField] private Material _roadMaterial;
        [SerializeField] private bool _generateCollider = true;
        [SerializeField] private float _curbHeight = 0.15f;

        private RoadSpline _spline;

        private void Awake()
        {
            _spline = GetComponent<RoadSpline>();
            GenerateRoadMesh();
        }

        [ContextMenu("Regenerate Road Mesh")]
        public void GenerateRoadMesh()
        {
            if (_spline == null) _spline = GetComponent<RoadSpline>();
            if (_spline.Nodes.Count < 2) return;

            int totalSegments = _spline.Nodes.Count * _segmentsPerCurve;
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            float halfWidth = (_spline.LaneCount * _spline.LaneWidth) * 0.5f;

            for (int i = 0; i <= totalSegments; i++)
            {
                float t = (float)i / totalSegments;
                Vector3 center = _spline.EvaluatePosition(t);
                Vector3 tangent = _spline.EvaluateTangent(t);
                Vector3 normal = Vector3.up;
                Vector3 right = Vector3.Cross(normal, tangent).normalized;

                // Left edge, center, right edge
                Vector3 leftPos = center - right * halfWidth;
                Vector3 rightPos = center + right * halfWidth;

                // Local space conversion
                vertices.Add(transform.InverseTransformPoint(leftPos));
                vertices.Add(transform.InverseTransformPoint(rightPos));

                uvs.Add(new Vector2(0f, t * totalSegments * 0.5f));
                uvs.Add(new Vector2(1f, t * totalSegments * 0.5f));

                if (i < totalSegments)
                {
                    int root = i * 2;
                    // Triangle 1
                    triangles.Add(root);
                    triangles.Add(root + 2);
                    triangles.Add(root + 1);
                    // Triangle 2
                    triangles.Add(root + 1);
                    triangles.Add(root + 2);
                    triangles.Add(root + 3);
                }
            }

            Mesh mesh = new Mesh
            {
                name = $"RoadMesh_{gameObject.name}"
            };
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            GetComponent<MeshFilter>().sharedMesh = mesh;

            if (_roadMaterial != null)
            {
                GetComponent<MeshRenderer>().sharedMaterial = _roadMaterial;
            }

            // Generate physical collider
            if (_generateCollider)
            {
                MeshCollider col = GetComponent<MeshCollider>();
                if (col == null) col = gameObject.AddComponent<MeshCollider>();
                col.sharedMesh = mesh;

                RoadPhysicsSurface physSurface = GetComponent<RoadPhysicsSurface>();
                if (physSurface == null) physSurface = gameObject.AddComponent<RoadPhysicsSurface>();
            }
        }
    }
}

