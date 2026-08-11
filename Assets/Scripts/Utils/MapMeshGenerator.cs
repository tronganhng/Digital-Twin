using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapMeshGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D mapTexture;

    [Header("Map")]
    [SerializeField] private float resolution = 0.05f;
    [SerializeField] private float wallHeight = 2f;

    [Header("Occupancy")]
    [SerializeField, Range(0f, 1f)]
    private float occupiedThreshold = 0.2f;

    [Button]
    public void Generate()
    {
        if (mapTexture == null)
        {
            Debug.LogError("Map texture is null.");
            return;
        }

        var mesh = GenerateWallMesh();

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
            meshCollider.sharedMesh = mesh;
    }

    private Mesh GenerateWallMesh()
    {
        int width = mapTexture.width;
        int height = mapTexture.height;

        Color32[] pixels = mapTexture.GetPixels32();

        bool[,] occupied = new bool[width, height];
        bool[,] visited = new bool[width, height];

        // 1. Convert image -> occupancy
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color32 pixel = pixels[y * width + x];

                float brightness = pixel.r / 255f;

                occupied[x, y] = brightness <= occupiedThreshold;
            }
        }

        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        // 2. Find and merge rectangles
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!occupied[x, y] || visited[x, y])
                    continue;

                int rectangleWidth = FindWidth(
                    x,
                    y,
                    width,
                    occupied,
                    visited
                );

                int rectangleHeight = FindHeight(
                    x,
                    y,
                    rectangleWidth,
                    height,
                    occupied,
                    visited
                );

                AddCube(
                    vertices,
                    triangles,
                    x,
                    y,
                    rectangleWidth,
                    rectangleHeight,
                    resolution,
                    wallHeight
                );

                // Mark cells as visited
                for (int yy = y; yy < y + rectangleHeight; yy++)
                {
                    for (int xx = x; xx < x + rectangleWidth; xx++)
                    {
                        visited[xx, yy] = true;
                    }
                }
            }
        }

        var mesh = new Mesh
        {
            name = "MapWalls"
        };

        mesh.indexFormat = vertices.Count > 65535
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private int FindWidth(
    int startX,
    int y,
    int width,
    bool[,] occupied,
    bool[,] visited)
    {
        int result = 0;

        for (int x = startX; x < width; x++)
        {
            if (!occupied[x, y] || visited[x, y])
                break;

            result++;
        }

        return result;
    }

    private int FindHeight(
    int startX,
    int startY,
    int rectangleWidth,
    int height,
    bool[,] occupied,
    bool[,] visited)
    {
        int result = 0;

        for (int y = startY; y < height; y++)
        {
            bool validRow = true;

            for (int x = startX; x < startX + rectangleWidth; x++)
            {
                if (!occupied[x, y] || visited[x, y])
                {
                    validRow = false;
                    break;
                }
            }

            if (!validRow)
                break;

            result++;
        }

        return result;
    }

    private void AddCube(
    List<Vector3> vertices,
    List<int> triangles,
    int x,
    int y,
    int width,
    int height,
    float cellSize,
    float wallHeight)
    {
        float minX = x * cellSize;
        float maxX = (x + width) * cellSize;

        float minZ = y * cellSize;
        float maxZ = (y + height) * cellSize;

        int start = vertices.Count;

        vertices.Add(new Vector3(minX, 0, minZ));
        vertices.Add(new Vector3(maxX, 0, minZ));
        vertices.Add(new Vector3(maxX, wallHeight, minZ));
        vertices.Add(new Vector3(minX, wallHeight, minZ));

        vertices.Add(new Vector3(minX, 0, maxZ));
        vertices.Add(new Vector3(maxX, 0, maxZ));
        vertices.Add(new Vector3(maxX, wallHeight, maxZ));
        vertices.Add(new Vector3(minX, wallHeight, maxZ));

        AddQuad(triangles, start + 0, start + 1, start + 2, start + 3);
        AddQuad(triangles, start + 5, start + 4, start + 7, start + 6);
        AddQuad(triangles, start + 4, start + 0, start + 3, start + 7);
        AddQuad(triangles, start + 1, start + 5, start + 6, start + 2);
        AddQuad(triangles, start + 3, start + 2, start + 6, start + 7);
        AddQuad(triangles, start + 4, start + 5, start + 1, start + 0);
    }

    private void AddQuad(
        System.Collections.Generic.List<int> triangles,
        int a,
        int b,
        int c,
        int d)
    {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);

        triangles.Add(a);
        triangles.Add(c);
        triangles.Add(d);
    }
}