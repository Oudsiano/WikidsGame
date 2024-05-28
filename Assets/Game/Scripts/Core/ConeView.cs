using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConeView : MonoBehaviour
{
    public float height = 2f; // Высота конуса
    public float radius = 1f; // Радиус основания конуса
    public int segments = 36; // Количество сегментов (чем больше, тем более гладкий конус)

    void Start()
    {
        GenerateCone();
    }

    void GenerateCone()
    {
        // Создаем новый Mesh
        Mesh mesh = new Mesh();

        // Получаем компоненты MeshFilter и MeshRenderer
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        // Присваиваем новый Mesh компоненту MeshFilter
        meshFilter.mesh = mesh;

        // Определяем массивы вершин, нормалей и треугольников
        Vector3[] vertices = new Vector3[segments + 2];
        Vector3[] normals = new Vector3[vertices.Length];
        int[] triangles = new int[segments * 3 * 2];

        // Центральная вершина основания
        vertices[0] = Vector3.zero;
        normals[0] = Vector3.down;

        // Верхняя вершина конуса
        vertices[1] = Vector3.up * height;
        normals[1] = Vector3.up;

        // Вершины по краю основания
        float angleStep = 2 * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i + 2] = new Vector3(x, 0, z);
            normals[i + 2] = Vector3.down;
        }

        // Треугольники для основания
        for (int i = 0; i < segments; i++)
        {
            int startIndex = i * 3;
            triangles[startIndex] = 0;
            triangles[startIndex + 1] = (i + 1) % segments + 2;
            triangles[startIndex + 2] = i + 2;
        }

        // Треугольники для боковых граней
        for (int i = 0; i < segments; i++)
        {
            int startIndex = segments * 3 + i * 3;
            triangles[startIndex] = 1;
            triangles[startIndex + 1] = i + 2;
            triangles[startIndex + 2] = (i + 1) % segments + 2;
        }

        // Присваиваем вершины, треугольники и нормали мешу
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        // Обновляем меш
        mesh.RecalculateBounds();
    }
}