using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class GridDrawer : MonoBehaviour
{
    [Header("Grid")]
    [Min(0.01f)]
    public float size = 1f;

    [Min(1)]
    public int row = 5;

    [Min(1)]
    public int column = 5;

    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Vector3 origin = transform.position;
        origin.x -= column * size * 0.5f;
        origin.z -= row * size * 0.5f;

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                Vector3 center = origin + new Vector3(
                    c * size + size * 0.5f,
                    0f,
                    r * size + size * 0.5f);

                Gizmos.DrawWireCube(center, new Vector3(size, 0f, size));
            }
        }
    }

    public List<Vector3> GetAllPositions()
    {
        List<Vector3> positions = new List<Vector3>(row * column);

        Vector3 origin = transform.position;
        origin.x -= column * size * 0.5f;
        origin.z -= row * size * 0.5f;

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                positions.Add(origin + new Vector3(
                    c * size + size * 0.5f,
                    0f,
                    r * size + size * 0.5f));
            }
        }

        return positions;
    }
}