using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Vector2[] _waypoints;
    private int _waypointIndex;
    private float _moveSpeed;

    public float CurrentHealth { get; private set; }
    public int WaypointIndex => _waypointIndex;  // Used by targeting strategies in Epic 4

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        Debug.Assert(_renderer != null, "[Enemy] SpriteRenderer component is missing");
        _renderer.color = new Color(0.9f, 0.2f, 0.2f); // Enemy red — GDD color palette
        _renderer.sortingOrder = 1;                     // Render above tiles (sortingOrder 0)

        // Pre-allocate world-space waypoint array once per instance (reused across pool cycles)
        _waypoints = new Vector2[PathManager.DefaultWaypoints.Length];
    }

    public void OnGetFromPool()
    {
        _waypointIndex = 0;
        gameObject.SetActive(true);
    }

    public void OnReleaseToPool()
    {
        gameObject.SetActive(false);
    }

    public void Initialize(EnemyDefinitionSO def)
    {
        Debug.Assert(PathManager.Instance != null, "[Enemy] PathManager.Instance is null in Initialize");

        // Convert integer grid coordinates to tile-center world positions (+0.5 offset).
        // GridManager places tile centers at (gridX + 0.5, gridY + 0.5); waypoints use gridX/gridY.
        Vector2[] raw = PathManager.Instance.Waypoints;
        for (int i = 0; i < raw.Length; i++)
            _waypoints[i] = raw[i] + new Vector2(0.5f, 0.5f);

        _moveSpeed = def.BaseSpeed;
        CurrentHealth = def.BaseHealth;
        transform.position = _waypoints[0];
        _waypointIndex = 1;
    }

    private void Update()
    {
        if (_waypoints == null || _waypointIndex >= _waypoints.Length) return;

        Vector2 target = _waypoints[_waypointIndex];
        Vector2 next = Vector2.MoveTowards((Vector2)transform.position, target, _moveSpeed * Time.deltaTime);
        transform.position = next;

        if (Vector2.SqrMagnitude(next - target) < 0.001f)
        {
            _waypointIndex++;
            if (_waypointIndex >= _waypoints.Length)
            {
                EnemyManager.Instance.OnEnemyReachedPathEnd(this);
            }
        }
    }
}
