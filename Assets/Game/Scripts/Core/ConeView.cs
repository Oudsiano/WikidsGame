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
            int modifyNumber = 3;
            int multiplier = 2;
            Mesh mesh = new Mesh();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>(); // TODO not used code
            
            meshFilter.mesh = mesh;
            
            Vector3[] vertices = new Vector3[_segments + multiplier];
            Vector3[] normals = new Vector3[vertices.Length];
            int[] triangles = new int[_segments * modifyNumber * multiplier];
            
            vertices[0] = Vector3.zero;
            normals[0] = Vector3.down;
            
            vertices[1] = Vector3.up * _height;
            normals[1] = Vector3.up;
            
            float angleStep = multiplier * Mathf.PI / _segments;
            
            for (int i = 0; i < _segments; i++)
            {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * _radius;
                float z = Mathf.Sin(angle) * _radius;
                vertices[i + multiplier] = new Vector3(x, 0, z);
                normals[i + multiplier] = Vector3.down;
            }
            
            for (int i = 0; i < _segments; i++)
            {
                int startIndex = i * modifyNumber;
                triangles[startIndex] = 0;
                triangles[startIndex + 1] = (i + 1) % _segments + multiplier;
                triangles[startIndex + multiplier] = i + multiplier;
            }
            
            for (int i = 0; i < _segments; i++)
            {
                int startIndex = _segments * modifyNumber + i * modifyNumber;
                triangles[startIndex] = 1;
                triangles[startIndex + 1] = i + multiplier;
                triangles[startIndex + multiplier] = (i + 1) % _segments + multiplier;
            }
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.RecalculateBounds();
        }
    }
}