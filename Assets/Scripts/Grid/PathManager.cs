using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    // NOTE: readonly prevents reference reassignment but NOT element mutation.
    // Treat this array as immutable — do not modify elements at runtime.
    public static readonly Vector2[] DefaultWaypoints = new Vector2[]
    {
        new Vector2(-10f,  2f),  // Entry: left side
        new Vector2( -3f,  2f),  // Turn
        new Vector2( -3f, -2f),  // Turn
        new Vector2(  4f, -2f),  // Turn
        new Vector2(  4f,  3f),  // Turn
        new Vector2( 10f,  3f),  // Exit: right side
    };

    private readonly Vector2[] _waypoints = DefaultWaypoints;

    public Vector2[] Waypoints => _waypoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public HashSet<Vector2Int> GetPathCells()
    {
        return ComputePathCells(_waypoints);
    }

    public static HashSet<Vector2Int> ComputePathCells(Vector2[] waypoints)
    {
        if (waypoints == null || waypoints.Length < 2)
            return new HashSet<Vector2Int>();

        var cells = new HashSet<Vector2Int>();
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector2 from = waypoints[i];
            Vector2 to   = waypoints[i + 1];

            if (Mathf.Approximately(from.y, to.y))
            {
                int y    = Mathf.RoundToInt(from.y);
                int xMin = Mathf.RoundToInt(Mathf.Min(from.x, to.x));
                int xMax = Mathf.RoundToInt(Mathf.Max(from.x, to.x));
                for (int x = xMin; x <= xMax; x++)
                    cells.Add(new Vector2Int(x, y));
            }
            else
            {
                int x    = Mathf.RoundToInt(from.x);
                int yMin = Mathf.RoundToInt(Mathf.Min(from.y, to.y));
                int yMax = Mathf.RoundToInt(Mathf.Max(from.y, to.y));
                for (int y = yMin; y <= yMax; y++)
                    cells.Add(new Vector2Int(x, y));
            }
        }
        return cells;
    }
}
