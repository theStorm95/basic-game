using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] private EnemyDefinitionSO _enemyDefinition;
    [SerializeField] private float _spawnInterval = 2f; // Test loop — WaveManager replaces in Epic 6

    public event System.Action OnEnemyReachedEnd; // Story 2-2 subscribes to deduct a life

    private readonly List<Enemy> _activeEnemies = new List<Enemy>();
    public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Debug.Assert(_enemyDefinition != null, "[EnemyManager] _enemyDefinition not assigned");
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[EnemyManager] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop()); // Initial spawn loop; not triggered by HandleStateChanged on first load
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.GameOver || state == GameState.Win)
        {
            StopAllCoroutines();
            ReturnAllEnemies();
        }
        else if (state == GameState.PreWave)
        {
            StartCoroutine(SpawnLoop()); // Restart after player clicks Restart
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        var enemy = EnemyPool.Instance.Get();
        enemy.Initialize(_enemyDefinition);
        _activeEnemies.Add(enemy);
        GameLog.Info("EnemyManager", "Enemy spawned");
    }

    public void OnEnemyReachedPathEnd(Enemy enemy)
    {
        _activeEnemies.Remove(enemy);
        EnemyPool.Instance.Release(enemy);
        OnEnemyReachedEnd?.Invoke();
        GameLog.Info("EnemyManager", "Enemy reached path end");
    }

    private void ReturnAllEnemies()
    {
        // Iterate backwards and release directly — bypasses OnEnemyReachedPathEnd
        // to avoid double-remove from _activeEnemies
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            EnemyPool.Instance.Release(_activeEnemies[i]);
        }
        _activeEnemies.Clear();
    }
}
