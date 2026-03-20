using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private int _defaultCapacity = 20;
    [SerializeField] private int _maxSize = 50;

    private ObjectPool<Enemy> _pool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Assert(_enemyPrefab != null, "[EnemyPool] _enemyPrefab not assigned");

        _pool = new ObjectPool<Enemy>(
            createFunc: () => Instantiate(_enemyPrefab),
            actionOnGet: e => e.OnGetFromPool(),
            actionOnRelease: e => e.OnReleaseToPool(),
            actionOnDestroy: e => Destroy(e.gameObject),
            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    public Enemy Get() => _pool.Get();
    public void Release(Enemy enemy) => _pool.Release(enemy);
}
