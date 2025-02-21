using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ConeView : MonoBehaviour
    {
        [FormerlySerializedAs("height")] [SerializeField] private float _height = 2f;
        [FormerlySerializedAs("radius")] [SerializeField] private float _radius = 1f; 
        [FormerlySerializedAs("segments")] [SerializeField] private int _segments = 36; 

        private void Start() // TODO Construct
        {
            GenerateCone();
        }

        private void GenerateCone() // TODO overload method // TODO magic numbers
        {
            Mesh mesh = new Mesh();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            
            meshFilter.mesh = mesh;
            
            Vector3[] vertices = new Vector3[_segments + 2];
            Vector3[] normals = new Vector3[vertices.Length];
            int[] triangles = new int[_segments * 3 * 2];
            
            vertices[0] = Vector3.zero;
            normals[0] = Vector3.down;
            
            vertices[1] = Vector3.up * _height;
            normals[1] = Vector3.up;
            
            float angleStep = 2 * Mathf.PI / _segments;
            for (int i = 0; i < _segments; i++)
            {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * _radius;
                float z = Mathf.Sin(angle) * _radius;
                vertices[i + 2] = new Vector3(x, 0, z);
                normals[i + 2] = Vector3.down;
            }
            
            for (int i = 0; i < _segments; i++)
            {
                int startIndex = i * 3;
                triangles[startIndex] = 0;
                triangles[startIndex + 1] = (i + 1) % _segments + 2;
                triangles[startIndex + 2] = i + 2;
            }
            
            for (int i = 0; i < _segments; i++)
            {
                int startIndex = _segments * 3 + i * 3;
                triangles[startIndex] = 1;
                triangles[startIndex + 1] = i + 2;
                triangles[startIndex + 2] = (i + 1) % _segments + 2;
            }
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.RecalculateBounds();
        }
    }
}