using System;
using System.Collections.Generic;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
public class DxfWallMeshGenerator : MonoBehaviour
{
    [SerializeField, Required] private TextAsset dxfFile;
    [SerializeField] private string wallLayer = "WALLS";
    [SerializeField, Min(0.01f)] private float wallHeight = 2.7f;
    [SerializeField] private bool readUnitsFromDxf = true;
    [SerializeField, Min(0.000001f)] private float unitlessScaleToMeters = 0.001f;
    [SerializeField] private bool createCollider = true;

    [ShowInInspector, ReadOnly] private int wallCount;
    [ShowInInspector, ReadOnly] private int vertexCount;
    [ShowInInspector, ReadOnly] private int triangleCount;

    private Mesh generatedMesh;

    [Button(ButtonSizes.Medium), GUIColor(0.3f, 0.8f, 0.3f)]
    public void Generate()
    {
        if (dxfFile == null)
        {
            Debug.LogError("[DxfWallMeshGenerator] Assign an ASCII DXF file first.");
            return;
        }

        if (!TryParsePairs(dxfFile.text, out List<DxfPair> pairs, out string parseError))
        {
            Debug.LogError($"[DxfWallMeshGenerator] {parseError}");
            return;
        }

        if (!TryGetScale(pairs, out float scale, out string unitError))
        {
            Debug.LogError($"[DxfWallMeshGenerator] {unitError}");
            return;
        }

        var polygons = ReadWallPolylines(pairs);
        if (polygons.Count == 0)
        {
            Debug.LogError($"[DxfWallMeshGenerator] No closed LWPOLYLINE entities found on layer '{wallLayer}'.");
            return;
        }

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        var triangles = new List<int>();
        int validWallCount = 0;

        foreach (List<Vector2> polygon in polygons)
        {
            if (!TryTriangulate(polygon, out List<int> polygonTriangles))
            {
                Debug.LogWarning("[DxfWallMeshGenerator] Skipping a wall outline that cannot be triangulated.");
                continue;
            }

            AddPrism(polygon, polygonTriangles, scale, wallHeight, vertices, normals, uvs, triangles);
            validWallCount++;
        }

        if (validWallCount == 0)
        {
            Debug.LogError("[DxfWallMeshGenerator] No valid wall outlines could be triangulated.");
            return;
        }

        ReleaseGeneratedMesh();
        generatedMesh = new Mesh { name = "DXF Wall Mesh" };
        generatedMesh.indexFormat = vertices.Count > 65535
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;
        generatedMesh.SetVertices(vertices);
        generatedMesh.SetNormals(normals);
        generatedMesh.SetUVs(0, uvs);
        generatedMesh.SetTriangles(triangles, 0);
        generatedMesh.RecalculateBounds();

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = generatedMesh;

        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();

        if (createCollider)
        {
            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null)
                meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = generatedMesh;
        }

        wallCount = validWallCount;
        vertexCount = generatedMesh.vertexCount;
        triangleCount = generatedMesh.triangles.Length / 3;
        Debug.Log($"[DxfWallMeshGenerator] Generated {wallCount} walls ({vertexCount} vertices, {triangleCount} triangles) at {scale} meters per DXF unit.");
    }

    [Button(ButtonSizes.Small), GUIColor(1f, 0.4f, 0.4f)]
    public void ClearMesh()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh == generatedMesh)
            meshFilter.sharedMesh = null;

        var meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null && meshCollider.sharedMesh == generatedMesh)
            meshCollider.sharedMesh = null;

        ReleaseGeneratedMesh();
        wallCount = 0;
        vertexCount = 0;
        triangleCount = 0;
    }

    private List<List<Vector2>> ReadWallPolylines(List<DxfPair> pairs)
    {
        var polygons = new List<List<Vector2>>();

        for (int i = 0; i < pairs.Count; i++)
        {
            if (pairs[i].Code != 0 || pairs[i].Value != "LWPOLYLINE")
                continue;

            string layer = string.Empty;
            int flags = 0;
            float pendingX = 0f;
            bool hasPendingX = false;
            var points = new List<Vector2>();

            for (int j = i + 1; j < pairs.Count && pairs[j].Code != 0; j++)
            {
                DxfPair pair = pairs[j];
                if (pair.Code == 8)
                    layer = pair.Value;
                else if (pair.Code == 70)
                    int.TryParse(pair.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out flags);
                else if (pair.Code == 10)
                {
                    hasPendingX = float.TryParse(pair.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out pendingX);
                }
                else if (pair.Code == 20 && hasPendingX
                    && float.TryParse(pair.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                {
                    points.Add(new Vector2(pendingX, y));
                    hasPendingX = false;
                }
            }

            if (!string.Equals(layer, wallLayer, StringComparison.OrdinalIgnoreCase) || (flags & 1) == 0)
                continue;

            CleanPolygon(points);
            if (points.Count >= 3)
                polygons.Add(points);
        }

        return polygons;
    }

    private bool TryGetScale(List<DxfPair> pairs, out float scale, out string error)
    {
        scale = unitlessScaleToMeters;
        error = null;
        if (!readUnitsFromDxf)
            return true;

        for (int i = 0; i + 1 < pairs.Count; i++)
        {
            if (pairs[i].Code != 9 || pairs[i].Value != "$INSUNITS")
                continue;

            if (pairs[i + 1].Code != 70
                || !int.TryParse(pairs[i + 1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int unitCode))
            {
                error = "Could not read the $INSUNITS value from the DXF header.";
                return false;
            }

            switch (unitCode)
            {
                case 0: scale = unitlessScaleToMeters; return true;
                case 1: scale = 0.0254f; return true;
                case 2: scale = 0.3048f; return true;
                case 3: scale = 1609.344f; return true;
                case 4: scale = 0.001f; return true;
                case 5: scale = 0.01f; return true;
                case 6: scale = 1f; return true;
                case 7: scale = 1000f; return true;
                case 10: scale = 0.9144f; return true;
                default:
                    error = $"Unsupported DXF $INSUNITS code {unitCode}. Set Read Units From Dxf off and choose a manual scale.";
                    return false;
            }
        }

        scale = unitlessScaleToMeters;
        return true;
    }

    private static bool TryParsePairs(string text, out List<DxfPair> pairs, out string error)
    {
        pairs = new List<DxfPair>();
        error = null;
        string[] lines = text.TrimEnd('\r', '\n').Split('\n');
        if ((lines.Length & 1) != 0)
        {
            error = "DXF does not contain complete group-code/value pairs.";
            return false;
        }

        for (int i = 0; i < lines.Length; i += 2)
        {
            string codeText = lines[i].Trim();
            if (!int.TryParse(codeText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int code))
            {
                error = $"Invalid DXF group code at line {i + 1}.";
                return false;
            }

            pairs.Add(new DxfPair(code, lines[i + 1].TrimEnd('\r')));
        }

        return true;
    }

    private static void CleanPolygon(List<Vector2> points)
    {
        const float epsilon = 0.0001f;
        for (int i = points.Count - 1; i > 0; i--)
        {
            if ((points[i] - points[i - 1]).sqrMagnitude <= epsilon * epsilon)
                points.RemoveAt(i);
        }

        if (points.Count > 1 && (points[0] - points[points.Count - 1]).sqrMagnitude <= epsilon * epsilon)
            points.RemoveAt(points.Count - 1);

        bool changed = true;
        while (changed && points.Count >= 3)
        {
            changed = false;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 previous = points[(i + points.Count - 1) % points.Count];
                Vector2 current = points[i];
                Vector2 next = points[(i + 1) % points.Count];
                if (Mathf.Abs(Cross(current - previous, next - current)) <= epsilon)
                {
                    points.RemoveAt(i);
                    changed = true;
                    break;
                }
            }
        }
    }

    private static bool TryTriangulate(List<Vector2> points, out List<int> triangles)
    {
        triangles = new List<int>();
        if (points.Count < 3)
            return false;

        var remaining = new List<int>(points.Count);
        for (int i = 0; i < points.Count; i++)
            remaining.Add(i);

        if (SignedArea(points) < 0f)
            remaining.Reverse();

        int attempts = 0;
        while (remaining.Count > 3 && attempts < points.Count * points.Count)
        {
            bool clippedEar = false;
            for (int i = 0; i < remaining.Count; i++)
            {
                int previous = remaining[(i + remaining.Count - 1) % remaining.Count];
                int current = remaining[i];
                int next = remaining[(i + 1) % remaining.Count];
                if (Cross(points[current] - points[previous], points[next] - points[current]) <= 0.000001f)
                    continue;

                bool containsPoint = false;
                for (int j = 0; j < remaining.Count; j++)
                {
                    int candidate = remaining[j];
                    if (candidate == previous || candidate == current || candidate == next)
                        continue;
                    if (IsInsideTriangle(points[candidate], points[previous], points[current], points[next]))
                    {
                        containsPoint = true;
                        break;
                    }
                }

                if (containsPoint)
                    continue;

                triangles.Add(previous);
                triangles.Add(current);
                triangles.Add(next);
                remaining.RemoveAt(i);
                clippedEar = true;
                break;
            }

            if (!clippedEar)
                return false;
            attempts++;
        }

        if (remaining.Count != 3)
            return false;

        triangles.Add(remaining[0]);
        triangles.Add(remaining[1]);
        triangles.Add(remaining[2]);
        return true;
    }

    private static void AddPrism(
        List<Vector2> polygon,
        List<int> polygonTriangles,
        float scale,
        float height,
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> meshTriangles)
    {
        bool ccw = SignedArea(polygon) > 0f;
        for (int i = 0; i < polygonTriangles.Count; i += 3)
        {
            Vector2 a = polygon[polygonTriangles[i]];
            Vector2 b = polygon[polygonTriangles[i + 1]];
            Vector2 c = polygon[polygonTriangles[i + 2]];
            if (ccw)
            {
                AddTriangle(vertices, normals, uvs, meshTriangles,
                    ToTop(a, scale, height), ToTop(c, scale, height), ToTop(b, scale, height), Vector3.up, a, c, b, scale);
                AddTriangle(vertices, normals, uvs, meshTriangles,
                    ToBottom(a, scale), ToBottom(b, scale), ToBottom(c, scale), Vector3.down, a, b, c, scale);
            }
            else
            {
                AddTriangle(vertices, normals, uvs, meshTriangles,
                    ToTop(a, scale, height), ToTop(b, scale, height), ToTop(c, scale, height), Vector3.up, a, b, c, scale);
                AddTriangle(vertices, normals, uvs, meshTriangles,
                    ToBottom(a, scale), ToBottom(c, scale), ToBottom(b, scale), Vector3.down, a, c, b, scale);
            }
        }

        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % polygon.Count];
            Vector2 edge = b - a;
            Vector3 sideNormal = ccw
                ? new Vector3(edge.y, 0f, -edge.x).normalized
                : new Vector3(-edge.y, 0f, edge.x).normalized;

            Vector3 bottomA = ToBottom(a, scale);
            Vector3 bottomB = ToBottom(b, scale);
            Vector3 topA = ToTop(a, scale, height);
            Vector3 topB = ToTop(b, scale, height);
            if (ccw)
            {
                AddQuad(vertices, normals, uvs, meshTriangles,
                    bottomA, topA, topB, bottomB, sideNormal,
                    new Vector2(0f, 0f), new Vector2(0f, height),
                    new Vector2(edge.magnitude * scale, height), new Vector2(edge.magnitude * scale, 0f));
            }
            else
            {
                AddQuad(vertices, normals, uvs, meshTriangles,
                    bottomB, topB, topA, bottomA, sideNormal,
                    new Vector2(0f, 0f), new Vector2(0f, height),
                    new Vector2(edge.magnitude * scale, height), new Vector2(edge.magnitude * scale, 0f));
            }
        }
    }

    private static void AddTriangle(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> triangles,
        Vector3 a, Vector3 b, Vector3 c,
        Vector3 normal,
        Vector2 uvA, Vector2 uvB, Vector2 uvC,
        float scale)
    {
        int start = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        uvs.Add(new Vector2(uvA.x * scale, uvA.y * scale));
        uvs.Add(new Vector2(uvB.x * scale, uvB.y * scale));
        uvs.Add(new Vector2(uvC.x * scale, uvC.y * scale));
        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
    }

    private static void AddQuad(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> triangles,
        Vector3 a, Vector3 b, Vector3 c, Vector3 d,
        Vector3 normal,
        Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector2 uvD)
    {
        int start = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        for (int i = 0; i < 4; i++)
            normals.Add(normal);
        uvs.Add(uvA);
        uvs.Add(uvB);
        uvs.Add(uvC);
        uvs.Add(uvD);
        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
        triangles.Add(start);
        triangles.Add(start + 2);
        triangles.Add(start + 3);
    }

    private void ReleaseGeneratedMesh()
    {
        if (generatedMesh == null)
            return;

        if (Application.isPlaying)
            Destroy(generatedMesh);
        else
            DestroyImmediate(generatedMesh);
        generatedMesh = null;
    }

    private static Vector3 ToTop(Vector2 point, float scale, float height)
    {
        return new Vector3(point.x * scale, height, point.y * scale);
    }

    private static Vector3 ToBottom(Vector2 point, float scale)
    {
        return new Vector3(point.x * scale, 0f, point.y * scale);
    }

    private static float SignedArea(List<Vector2> points)
    {
        float area = 0f;
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % points.Count];
            area += current.x * next.y - next.x * current.y;
        }
        return area * 0.5f;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    private static bool IsInsideTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
    {
        float ab = Cross(b - a, point - a);
        float bc = Cross(c - b, point - b);
        float ca = Cross(a - c, point - c);
        return ab >= -0.000001f && bc >= -0.000001f && ca >= -0.000001f;
    }

    private readonly struct DxfPair
    {
        public readonly int Code;
        public readonly string Value;

        public DxfPair(int code, string value)
        {
            Code = code;
            Value = value;
        }
    }
}