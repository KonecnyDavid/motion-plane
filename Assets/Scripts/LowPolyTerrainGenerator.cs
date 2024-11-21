using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LowPolyTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int width = 10; // Number of vertices along the x-axis
    public int depth = 10; // Number of vertices along the z-axis
    public float cellSize = 1f; // Distance between vertices
    public float heightScale = 2f; // Max height of terrain
    public float noiseScale = 0.3f; // Scale of Perlin noise

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        // Create a new mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Generate vertices and triangles
        GenerateVertices();
        GenerateTriangles();

        // Apply to the mesh
        UpdateMesh();

        // Optional: Recalculate normals for flat shading
        mesh.RecalculateNormals();
    }

    void GenerateVertices()
    {
        int vertexCount = (width + 1) * (depth + 1);
        vertices = new Vector3[vertexCount];

        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightScale;
                vertices[z * (width + 1) + x] = new Vector3(x * cellSize, y, z * cellSize);
            }
        }
    }

    void GenerateTriangles()
    {
        triangles = new int[width * depth * 6];
        int triangleIndex = 0;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int start = z * (width + 1) + x;

                // First triangle
                triangles[triangleIndex++] = start;
                triangles[triangleIndex++] = start + width + 1;
                triangles[triangleIndex++] = start + 1;

                // Second triangle
                triangles[triangleIndex++] = start + 1;
                triangles[triangleIndex++] = start + width + 1;
                triangles[triangleIndex++] = start + width + 2;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Needed for lighting
    }

    private void OnDrawGizmos()
    {
        // Visualize vertices in the editor
        if (vertices == null) return;

        Gizmos.color = Color.red;
        foreach (var vertex in vertices)
        {
            Gizmos.DrawSphere(transform.position + vertex, 0.1f);
        }
    }
}
