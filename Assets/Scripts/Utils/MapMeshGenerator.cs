using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapMeshGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D mapTexture;

    [Header("Map Settings")]
    [SerializeField] private float resolution = 0.05f;
    [SerializeField] private float wallHeight = 2f;
    [SerializeField] private Vector2 origin = new Vector2(-14.3f, -22f);

    [Header("Occupancy")]
    [SerializeField, Range(0f, 1f)]
    private float occupiedThreshold = 0.2f;

    [Tooltip("Check if dark pixels represent occupied walls")]
    [SerializeField] private bool darkIsOccupied = true;

    [Header("Mesh Options")]
    [SerializeField] private bool generateTop = true;
    [SerializeField] private bool generateBottom = false;
    [SerializeField] private float uvScale = 1f;
    [SerializeField] private bool updateCollider = true;

    [Header("Mesh Stats")]
    [ShowInInspector, ReadOnly] private int vertexCount;
    [ShowInInspector, ReadOnly] private int triangleCount;

    [Button(ButtonSizes.Medium), GUIColor(0.3f, 0.8f, 0.3f)]
    public void Generate()
    {
        if (mapTexture == null)
        {
            Debug.LogError("[MapMeshGenerator] Map texture is null.");
            return;
        }

        var mesh = GenerateWallMesh();

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.sharedMesh = mesh;

        transform.position = new Vector3(origin.x, 0f, origin.y);

        if (updateCollider)
        {
            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
            }
        }

        vertexCount = mesh.vertexCount;
        triangleCount = mesh.triangles.Length / 3;
        Debug.Log($"[MapMeshGenerator] Generated wall mesh successfully! Vertices: {vertexCount}, Triangles: {triangleCount}");
    }

    [Button(ButtonSizes.Small), GUIColor(1f, 0.4f, 0.4f)]
    public void ClearMesh()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.sharedMesh = null;
        }

        var meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
        }

        vertexCount = 0;
        triangleCount = 0;
    }

    private Mesh GenerateWallMesh()
    {
        int width = mapTexture.width;
        int height = mapTexture.height;

        Color32[] pixels = mapTexture.GetPixels32();
        bool[,] occupied = new bool[width, height];

        // 1. Convert image pixels to occupancy grid
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color32 pixel = pixels[y * width + x];
                float brightness = pixel.r / 255f;

                occupied[x, y] = darkIsOccupied
                    ? brightness <= occupiedThreshold
                    : brightness > occupiedThreshold;
            }
        }

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        var triangles = new List<int>();

        // Helper to check cell occupancy with bounds checking
        bool IsCellOccupied(int cx, int cy)
        {
            if (cx < 0 || cx >= width || cy < 0 || cy >= height)
                return false;
            return occupied[cx, cy];
        }

        // 2. Generate Combined Top & Bottom Faces (Greedy 2D Rectangles)
        if (generateTop || generateBottom)
        {
            bool[,] topVisited = new bool[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!occupied[x, y] || topVisited[x, y])
                        continue;

                    int rectW = FindWidth(x, y, width, occupied, topVisited);
                    int rectH = FindHeight(x, y, rectW, height, occupied, topVisited);

                    float minX = x * resolution;
                    float maxX = (x + rectW) * resolution;
                    float minZ = y * resolution;
                    float maxZ = (y + rectH) * resolution;

                    if (generateTop)
                    {
                        // Top Face (Clockwise looking from +Y down)
                        AddQuad(
                            vertices, normals, uvs, triangles,
                            new Vector3(minX, wallHeight, minZ),
                            new Vector3(minX, wallHeight, maxZ),
                            new Vector3(maxX, wallHeight, maxZ),
                            new Vector3(maxX, wallHeight, minZ),
                            Vector3.up,
                            new Vector2(minX, minZ) / uvScale,
                            new Vector2(minX, maxZ) / uvScale,
                            new Vector2(maxX, maxZ) / uvScale,
                            new Vector2(maxX, minZ) / uvScale
                        );
                    }

                    if (generateBottom)
                    {
                        // Bottom Face (Clockwise looking from -Y up)
                        AddQuad(
                            vertices, normals, uvs, triangles,
                            new Vector3(minX, 0f, maxZ),
                            new Vector3(minX, 0f, minZ),
                            new Vector3(maxX, 0f, minZ),
                            new Vector3(maxX, 0f, maxZ),
                            Vector3.down,
                            new Vector2(minX, maxZ) / uvScale,
                            new Vector2(minX, minZ) / uvScale,
                            new Vector2(maxX, minZ) / uvScale,
                            new Vector2(maxX, maxZ) / uvScale
                        );
                    }

                    // Mark as visited
                    for (int yy = y; yy < y + rectH; yy++)
                    {
                        for (int xx = x; xx < x + rectW; xx++)
                        {
                            topVisited[xx, yy] = true;
                        }
                    }
                }
            }
        }

        // 3. Generate Combined Boundary Side Walls (Greedy 1D Wall Segments)
        // A. South Walls (Facing -Z / Front, where cell is occupied and (y-1) is empty)
        for (int y = 0; y < height; y++)
        {
            int startX = -1;
            for (int x = 0; x < width; x++)
            {
                bool isSouthEdge = occupied[x, y] && !IsCellOccupied(x, y - 1);
                if (isSouthEdge)
                {
                    if (startX == -1) startX = x;
                }
                else
                {
                    if (startX != -1)
                    {
                        AddSouthWall(vertices, normals, uvs, triangles, startX, x, y);
                        startX = -1;
                    }
                }
            }
            if (startX != -1)
            {
                AddSouthWall(vertices, normals, uvs, triangles, startX, width, y);
            }
        }

        // B. North Walls (Facing +Z / Back, where cell is occupied and (y+1) is empty)
        for (int y = 0; y < height; y++)
        {
            int startX = -1;
            for (int x = 0; x < width; x++)
            {
                bool isNorthEdge = occupied[x, y] && !IsCellOccupied(x, y + 1);
                if (isNorthEdge)
                {
                    if (startX == -1) startX = x;
                }
                else
                {
                    if (startX != -1)
                    {
                        AddNorthWall(vertices, normals, uvs, triangles, startX, x, y);
                        startX = -1;
                    }
                }
            }
            if (startX != -1)
            {
                AddNorthWall(vertices, normals, uvs, triangles, startX, width, y);
            }
        }

        // C. West Walls (Facing -X / Left, where cell is occupied and (x-1) is empty)
        for (int x = 0; x < width; x++)
        {
            int startY = -1;
            for (int y = 0; y < height; y++)
            {
                bool isWestEdge = occupied[x, y] && !IsCellOccupied(x - 1, y);
                if (isWestEdge)
                {
                    if (startY == -1) startY = y;
                }
                else
                {
                    if (startY != -1)
                    {
                        AddWestWall(vertices, normals, uvs, triangles, x, startY, y);
                        startY = -1;
                    }
                }
            }
            if (startY != -1)
            {
                AddWestWall(vertices, normals, uvs, triangles, x, startY, height);
            }
        }

        // D. East Walls (Facing +X / Right, where cell is occupied and (x+1) is empty)
        for (int x = 0; x < width; x++)
        {
            int startY = -1;
            for (int y = 0; y < height; y++)
            {
                bool isEastEdge = occupied[x, y] && !IsCellOccupied(x + 1, y);
                if (isEastEdge)
                {
                    if (startY == -1) startY = y;
                }
                else
                {
                    if (startY != -1)
                    {
                        AddEastWall(vertices, normals, uvs, triangles, x, startY, y);
                        startY = -1;
                    }
                }
            }
            if (startY != -1)
            {
                AddEastWall(vertices, normals, uvs, triangles, x, startY, height);
            }
        }

        // 4. Construct Final Mesh
        var mesh = new Mesh
        {
            name = "MapWalls"
        };

        mesh.indexFormat = vertices.Count > 65535
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateBounds();

        return mesh;
    }

    private void AddSouthWall(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, int startX, int endX, int y)
    {
        float x0 = startX * resolution;
        float x1 = endX * resolution;
        float z = y * resolution;
        float segLen = x1 - x0;

        // Facing -Z: Bottom-Left (x0, 0), Top-Left (x0, H), Top-Right (x1, H), Bottom-Right (x1, 0)
        AddQuad(
            verts, norms, uvs, tris,
            new Vector3(x0, 0f, z),
            new Vector3(x0, wallHeight, z),
            new Vector3(x1, wallHeight, z),
            new Vector3(x1, 0f, z),
            Vector3.back,
            new Vector2(0f, 0f),
            new Vector2(0f, wallHeight / uvScale),
            new Vector2(segLen / uvScale, wallHeight / uvScale),
            new Vector2(segLen / uvScale, 0f)
        );
    }

    private void AddNorthWall(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, int startX, int endX, int y)
    {
        float x0 = startX * resolution;
        float x1 = endX * resolution;
        float z = (y + 1) * resolution;
        float segLen = x1 - x0;

        // Facing +Z: Bottom-Left (x1, 0), Top-Left (x1, H), Top-Right (x0, H), Bottom-Right (x0, 0)
        AddQuad(
            verts, norms, uvs, tris,
            new Vector3(x1, 0f, z),
            new Vector3(x1, wallHeight, z),
            new Vector3(x0, wallHeight, z),
            new Vector3(x0, 0f, z),
            Vector3.forward,
            new Vector2(0f, 0f),
            new Vector2(0f, wallHeight / uvScale),
            new Vector2(segLen / uvScale, wallHeight / uvScale),
            new Vector2(segLen / uvScale, 0f)
        );
    }

    private void AddWestWall(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, int x, int startY, int endY)
    {
        float xPos = x * resolution;
        float z0 = startY * resolution;
        float z1 = endY * resolution;
        float segLen = z1 - z0;

        // Facing -X: Bottom-Left (z1, 0), Top-Left (z1, H), Top-Right (z0, H), Bottom-Right (z0, 0)
        AddQuad(
            verts, norms, uvs, tris,
            new Vector3(xPos, 0f, z1),
            new Vector3(xPos, wallHeight, z1),
            new Vector3(xPos, wallHeight, z0),
            new Vector3(xPos, 0f, z0),
            Vector3.left,
            new Vector2(0f, 0f),
            new Vector2(0f, wallHeight / uvScale),
            new Vector2(segLen / uvScale, wallHeight / uvScale),
            new Vector2(segLen / uvScale, 0f)
        );
    }

    private void AddEastWall(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris, int x, int startY, int endY)
    {
        float xPos = (x + 1) * resolution;
        float z0 = startY * resolution;
        float z1 = endY * resolution;
        float segLen = z1 - z0;

        // Facing +X: Bottom-Left (z0, 0), Top-Left (z0, H), Top-Right (z1, H), Bottom-Right (z1, 0)
        AddQuad(
            verts, norms, uvs, tris,
            new Vector3(xPos, 0f, z0),
            new Vector3(xPos, wallHeight, z0),
            new Vector3(xPos, wallHeight, z1),
            new Vector3(xPos, 0f, z1),
            Vector3.right,
            new Vector2(0f, 0f),
            new Vector2(0f, wallHeight / uvScale),
            new Vector2(segLen / uvScale, wallHeight / uvScale),
            new Vector2(segLen / uvScale, 0f)
        );
    }

    private void AddQuad(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> triangles,
        Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3,
        Vector3 normal,
        Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        int start = vertices.Count;

        vertices.Add(v0);
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        uvs.Add(uv0);
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);

        // Clockwise winding order: 0 -> 1 -> 2 and 0 -> 2 -> 3
        triangles.Add(start + 0);
        triangles.Add(start + 1);
        triangles.Add(start + 2);

        triangles.Add(start + 0);
        triangles.Add(start + 2);
        triangles.Add(start + 3);
    }

    private int FindWidth(int startX, int y, int width, bool[,] occupied, bool[,] visited)
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

    private int FindHeight(int startX, int startY, int rectangleWidth, int height, bool[,] occupied, bool[,] visited)
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
}