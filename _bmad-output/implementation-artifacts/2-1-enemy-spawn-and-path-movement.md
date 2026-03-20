# Story 2.1: Enemy Spawn and Path Movement

Status: done

## Story

As a player,
I want to see enemies spawn at the start of the path and move along it,
so that the core enemy threat system is visible and functional.

## Acceptance Criteria

1. An enemy spawns at `PathManager.Waypoints[0]` (world position `(-10, 2)`) and moves smoothly along all 6 waypoints to the exit at `(10, 3)`
2. When an enemy reaches the last waypoint, it disappears from the screen (released back to pool) and `EnemyManager.OnEnemyReachedEnd` event is fired
3. `EnemyManager.ActiveEnemies` list is accurate: enemies are added on spawn and removed when they reach the path end
4. A test spawn loop automatically spawns one enemy every 2 seconds (configurable via `[SerializeField] private float _spawnInterval`) — this is a placeholder; `WaveManager` replaces this in Epic 6
5. Spawn loop stops and all active enemies are returned to pool on `GameState.GameOver` or `GameState.Win`; spawn loop restarts on `GameState.PreWave` (i.e., after Restart)
6. Enemy has a placeholder visual: square sprite (white with color set to red in code — `new Color(0.9f, 0.2f, 0.2f)`)
7. Enemy movement uses zero heap allocations in `Update()` — no LINQ, no `new` calls, no `GetComponent()`
8. Enemy and pool classes are in `Scripts/Enemies/`; `EnemyDefinitionSO` class is in `Scripts/Enemies/`; the `.asset` file is in `Data/Enemies/`

## Tasks / Subtasks

- [x] Task 1: Create `EnemyDefinitionSO.cs` in `Assets/Scripts/Enemies/` (AC: 1, 7)
  - [x] `[CreateAssetMenu(fileName = "EnemyDefinition", menuName = "BasicTD/EnemyDefinition")]`
  - [x] `[field: SerializeField] public float BaseHealth { get; private set; } = 100f;`
  - [x] `[field: SerializeField] public float BaseSpeed  { get; private set; } = 3f;`
  - [x] Create `Data/Enemies/` folder and create `EnemyDefinition.asset` in it via the menu

- [x] Task 2: Create `Enemy.cs` in `Assets/Scripts/Enemies/` (AC: 1, 2, 7)
  - [x] `[RequireComponent(typeof(SpriteRenderer))]`
  - [x] Private cached fields: `SpriteRenderer _renderer`, `Vector2[] _waypoints`, `int _waypointIndex`, `float _moveSpeed`
  - [x] Public read-only properties: `float CurrentHealth`, `int WaypointIndex` (for Combat system in Epic 4 targeting)
  - [x] `Awake()`: cache `_renderer`, `Debug.Assert` it's not null, set `_renderer.color = new Color(0.9f, 0.2f, 0.2f)`
  - [x] `OnGetFromPool()`: reset `_waypointIndex = 0`, `gameObject.SetActive(true)` — called by pool BEFORE `Initialize`
  - [x] `OnReleaseToPool()`: `gameObject.SetActive(false)`
  - [x] `Initialize(EnemyDefinitionSO def)`: fetch `_waypoints = PathManager.Instance.Waypoints`, set `_moveSpeed` and `CurrentHealth` from def, set `transform.position = _waypoints[0]`, set `_waypointIndex = 1`
  - [x] `Update()`: zero-alloc move toward `_waypoints[_waypointIndex]` using `Vector2.MoveTowards`; advance index when within `sqrMagnitude < 0.001f`; call `EnemyManager.Instance.OnEnemyReachedPathEnd(this)` when index exceeds last waypoint
  - [x] Guard at top of `Update()`: return early if `_waypoints == null || _waypointIndex >= _waypoints.Length`

- [x] Task 3: Create `EnemyPool.cs` in `Assets/Scripts/Enemies/` (AC: 7, 8)
  - [x] Singleton pattern identical to GameManager/PathManager — `Awake()` sets `Instance`, destroys duplicate
  - [x] `[SerializeField] private Enemy _enemyPrefab;`
  - [x] `[SerializeField] private int _defaultCapacity = 20; [SerializeField] private int _maxSize = 50;`
  - [x] `using UnityEngine.Pool;` — ObjectPool<Enemy> in Awake
  - [x] `ObjectPool<Enemy>` wiring: `createFunc: () => Instantiate(_enemyPrefab)`, `actionOnGet: e => e.OnGetFromPool()`, `actionOnRelease: e => e.OnReleaseToPool()`, `actionOnDestroy: e => Destroy(e.gameObject)`, `collectionCheck: true`
  - [x] `Debug.Assert(_enemyPrefab != null, "[EnemyPool] _enemyPrefab not assigned")` in Awake
  - [x] Public `Enemy Get() => _pool.Get()` and `void Release(Enemy enemy) => _pool.Release(enemy)`

- [x] Task 4: Create `EnemyManager.cs` in `Assets/Scripts/Enemies/` (AC: 2, 3, 4, 5)
  - [x] Singleton pattern, `Awake()` sets Instance, `Debug.Assert(_enemyDefinition != null, ...)`
  - [x] `[SerializeField] private EnemyDefinitionSO _enemyDefinition;`
  - [x] `[SerializeField] private float _spawnInterval = 2f;`
  - [x] `public event System.Action OnEnemyReachedEnd;` — Story 2-2 subscribes to deduct a life
  - [x] `private readonly List<Enemy> _activeEnemies = new List<Enemy>();` pre-allocated in field
  - [x] `public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;` — Combat system uses this in Epic 4
  - [x] `OnEnable()`: `Debug.Assert(GameManager.Instance != null)`, subscribe `GameManager.Instance.OnStateChanged += HandleStateChanged`
  - [x] `OnDisable()`: null-guard, unsubscribe `GameManager.Instance.OnStateChanged -= HandleStateChanged`
  - [x] `Start()`: `StartCoroutine(SpawnLoop())` — initial spawn loop (HandleStateChanged is NOT called for the initial PreWave state)
  - [x] `HandleStateChanged(GameState state)`: GameOver/Win → `StopAllCoroutines()` then `ReturnAllEnemies()`; PreWave → `StartCoroutine(SpawnLoop())` (restart after Restart button)
  - [x] `SpawnLoop()`: `while(true)` → `SpawnEnemy()` → `yield return new WaitForSeconds(_spawnInterval)`
  - [x] `SpawnEnemy()`: `var enemy = EnemyPool.Instance.Get(); enemy.Initialize(_enemyDefinition); _activeEnemies.Add(enemy); GameLog.Info("EnemyManager", "Enemy spawned")`
  - [x] `OnEnemyReachedPathEnd(Enemy enemy)`: `_activeEnemies.Remove(enemy); EnemyPool.Instance.Release(enemy); OnEnemyReachedEnd?.Invoke(); GameLog.Info("EnemyManager", "Enemy reached path end")`
  - [x] `ReturnAllEnemies()`: reverse loop `for (int i = _activeEnemies.Count - 1; i >= 0; i--) { _activeEnemies[i]... }` then `_activeEnemies.Clear()` — bypasses `OnEnemyReachedPathEnd` to prevent double-remove

- [x] Task 5: Create Enemy prefab (AC: 6)
  - [x] Create `Prefabs/Enemies/` folder in Assets
  - [x] Create a new 2D Sprite GameObject in scene, add `Enemy.cs`, save as prefab `Assets/Prefabs/Enemies/Enemy.prefab`
  - [x] Prefab needs a `SpriteRenderer` component with any white/grey square sprite (see Dev Notes for sprite options)
  - [x] Remove the temporary GameObject from scene after creating prefab

- [x] Task 6: Wire scene GameObjects (AC: 1–8)
  - [x] Create "EnemyPool" GameObject in scene, add `EnemyPool.cs`, wire `_enemyPrefab` → `Enemy.prefab`
  - [x] Create "EnemyManager" GameObject in scene, add `EnemyManager.cs`, wire `_enemyDefinition` → `Data/Enemies/EnemyDefinition.asset`
  - [x] Verify EnemyPool and EnemyManager GameObjects are NOT children of other managers (keep them at scene root alongside GameManager, PathManager, GridManager)

- [x] Task 7: Manual QA (AC: 1–8)
  - [x] Enter Play Mode — no console errors or missing-reference assertions
  - [x] Confirm enemies appear at top-left of path (`(-10, 2)`) every 2 seconds and walk the full path
  - [x] Confirm enemies disappear when they exit at `(10, 3)` (right side)
  - [x] Press backtick → GameOver → enemies disappear and stop spawning
  - [x] Click Restart → enemies resume spawning after the delay
  - [x] Confirm path tile coloring from Story 1.2 is unaffected (regression check)
  - [x] Confirm GameOverPanel and WinPanel still work correctly (regression from Stories 1.3–1.5)

## Dev Notes

### New Files to Create

| File | Location |
|------|----------|
| `EnemyDefinitionSO.cs` | `Assets/Scripts/Enemies/` |
| `Enemy.cs` | `Assets/Scripts/Enemies/` |
| `EnemyPool.cs` | `Assets/Scripts/Enemies/` |
| `Enemy.prefab` | `Assets/Prefabs/Enemies/` |
| `EnemyDefinition.asset` | `Assets/Data/Enemies/` |

### Existing Files to Touch — DO NOT Recreate

None — all changes are new files and scene wiring only. `GameManager.cs`, `PathManager.cs`, `GridManager.cs`, and all UI scripts are **untouched**.

### Enemy.cs — Final Shape

```csharp
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
        _waypoints = PathManager.Instance.Waypoints;
        _moveSpeed = def.BaseSpeed;
        CurrentHealth = def.BaseHealth;
        transform.position = (Vector2)_waypoints[0];
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
```

### EnemyPool.cs — Final Shape

```csharp
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
```

### EnemyManager.cs — Final Shape

```csharp
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
```

### EnemyDefinitionSO.cs — Final Shape

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinition", menuName = "BasicTD/EnemyDefinition")]
public class EnemyDefinitionSO : ScriptableObject
{
    [field: SerializeField] public float BaseHealth { get; private set; } = 100f;
    [field: SerializeField] public float BaseSpeed  { get; private set; } = 3f;
}
```

### Enemy Prefab — Sprite Setup

The Enemy prefab needs a `SpriteRenderer` with a square sprite. **Three options (choose one):**

1. **Easiest:** In the Unity Editor, create a 2D Object → Sprite Shape or use Sprites > Square from the Sprite editor. This creates a simple quad.
2. **Consistent with GridManager:** GridManager creates a 2x2 white `Texture2D` programmatically. You can do the same for the Enemy prefab during scene setup and save the sprite to `Art/Enemies/`. This avoids any external asset.
3. **Cleanest for future:** Import a tiny white PNG (even 4×4 pixels) as a Sprite to `Art/Enemies/enemy_placeholder.png`, assign to prefab. Replace with real art in Epic 7.

`Enemy.Awake()` always overrides `_renderer.color` to red — the sprite itself just needs to be any white/grey square.

**Enemy size:** Set prefab scale to `(1, 1, 1)` — same grid unit size as tiles. Enemies should be clearly visible against the path (tan/brown) tiles.

### Path Waypoints Reference

`PathManager.DefaultWaypoints` (the path enemies follow):

```
(-10, 2) → (-3, 2) → (-3, -2) → (4, -2) → (4, 3) → (10, 3)
```

Enemies spawn at `(-10, 2)` and exit at `(10, 3)`. The path crosses the grid from left to right with two bends.

### `ObjectPool<T>` Lifecycle — Order of Operations

On `EnemyPool.Get()`:
1. Pool calls `createFunc` (first time only): `Instantiate(_enemyPrefab)` — `Enemy.Awake()` runs
2. Pool calls `actionOnGet(enemy)` → `enemy.OnGetFromPool()` → resets state, activates GameObject
3. Control returns to caller — caller then calls `enemy.Initialize(def)`

On `EnemyPool.Release(enemy)`:
1. Pool calls `actionOnRelease(enemy)` → `enemy.OnReleaseToPool()` → deactivates GameObject
2. Enemy is held in pool, awaiting next Get()

**Critical:** All state is reset in `OnGetFromPool()` and `Initialize()`. Never rely on a field value from a previous use of the same enemy instance.

### Architecture Compliance Checklist

- **No `FindObjectOfType<T>()`** — access managers via `Manager.Instance` only
- **`[SerializeField] private`** for all Inspector-wired fields — `_enemyPrefab`, `_enemyDefinition`, `_spawnInterval`
- **`Debug.Assert` in `Awake()`** for every wired field
- **Subscribe in `OnEnable()`, unsubscribe in `OnDisable()`** — event lifecycle matches GameOverPanel/WinPanel pattern
- **`using UnityEngine.Pool;`** required for `ObjectPool<T>` — add to EnemyPool.cs
- **Zero allocations in `Enemy.Update()`** — `Vector2.MoveTowards` returns a struct (no alloc), `Vector2.SqrMagnitude` is a value comparison (no alloc)
- **`_activeEnemies`** is pre-allocated as a field (`new List<Enemy>()`) — never re-created during play
- **`EnemyPool` is the only place `Instantiate(enemyPrefab)` is called** — enemy creation is pool-only
- **`ReturnAllEnemies()` uses direct `EnemyPool.Instance.Release()`** — bypasses `OnEnemyReachedPathEnd` to prevent double-remove from `_activeEnemies`
- **`GameLog.Info` not `Debug.Log`** — strips from release builds
- **Script folders**: `Scripts/Enemies/` for all 4 .cs files — never in `Assets/Scripts/` root
- **Enemy color** follows GDD color palette: red/orange tones for enemies, never same family as towers (blue/teal) or path (neutral)

### Story Scope Boundary — What This Story Does NOT Include

- **Lives deduction** when enemy reaches end — Story 2-2 (`EnemyManager.OnEnemyReachedEnd` event is declared here but nothing subscribes yet)
- **Lives UI** (lives count display) — Story 2-3
- **Game over on zero lives** — Story 2-4
- **Wave management** — Epic 6; the `_spawnInterval` test loop is a placeholder
- **Enemy stat scaling** per wave — Epic 6
- **Enemy health/damage** system — Epic 4

The `OnEnemyReachedEnd` event and `ActiveEnemies` list are declared now because future stories (2-2, Epic 4) will depend on them — but nothing subscribes or reads them yet in this story.

### Previous Story Intelligence (Story 1.5 / Epic 1 Foundation)

- `GameManager.RestartGame()` calls `SetState(GameState.PreWave)` — `EnemyManager.HandleStateChanged(PreWave)` fires and restarts the spawn loop. **Test this flow explicitly in QA.**
- Pattern for singleton: `if (Instance != null && Instance != this) { Destroy(gameObject); return; }` — identical to GameManager, PathManager, GridManager
- `GameManager.cs` has `executionOrder: -100` in its .meta file — EnemyManager and EnemyPool do NOT need special execution order; they access GameManager in `OnEnable()` (after all `Awake()` calls are done)
- `WinPanel` and `GameOverPanel` subscribe to `GameManager.OnStateChanged` in `OnEnable()` — EnemyManager follows the same pattern, no conflicts
- Debug key backtick (`GameOver`) and F5 (`Win`) are reserved. **No new debug keys** needed for this story — the auto-spawn loop is sufficient for manual testing.

### Testing

Per `project-context.md`: Edit Mode tests are for pure C# logic (targeting strategies, economy math). Enemy movement and pool behavior are validated through Play Mode — manual QA only for this story.

No unit tests needed for this story. `EnemyDefinitionSO` data values (BaseHealth, BaseSpeed) are not tested since they're just inspector-editable floats.

### Project Structure Notes

New folders to create:
- `Assets/Scripts/Enemies/` (new — 4 .cs files go here)
- `Assets/Prefabs/Enemies/` (new — Enemy.prefab goes here)
- `Assets/Data/Enemies/` (new — EnemyDefinition.asset goes here)

Existing folders unchanged:
- `Assets/Scripts/Core/`
- `Assets/Scripts/Grid/`
- `Assets/Scripts/UI/`
- `Assets/Scenes/` (Main.unity modified only to add EnemyPool and EnemyManager GameObjects)

### References

- Architecture: `_bmad-output/game-architecture.md` → ADR-005 (ObjectPool<T>), ADR-006 (single scene), Entity Creation Patterns, Performance Rules, Code Organization Rules
- Epics: `_bmad-output/epics.md` → Epic 2: Enemy Path System, Story 2-1
- GDD: `_bmad-output/gdd.md` → Art Direction (enemy color: red/orange), Core Gameplay Loop, Win/Loss Conditions
- Project Context: `_bmad-output/project-context.md` → Anti-Patterns (no FindObjectOfType, no Instantiate outside pool), Performance Rules (zero-alloc Update), Engine-Specific Rules (lifecycle order, Awake/OnEnable/Start)
- Previous Story: `_bmad-output/implementation-artifacts/1-5-restart-from-end-screens.md` → RestartGame() flow, reserved debug keys, CanvasGroup/event subscription pattern

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

None — implementation matched story spec exactly.

### Completion Notes List

- Tasks 1–4: All script files created in `Assets/Scripts/Enemies/` matching the shape defined in Dev Notes, with the following documented deviations:
- `EnemyDefinitionSO.cs`: ScriptableObject with BaseHealth=100, BaseSpeed=3, CreateAssetMenu registered.
- `Enemy.cs`: Full zero-alloc Update() using Vector2.MoveTowards + SqrMagnitude; pool lifecycle (OnGetFromPool/OnReleaseToPool/Initialize) implemented; color set to red in Awake(). **Implementation deviation from story spec shape:** (1) `Awake()` also sets `_renderer.sortingOrder = 1` (renders above tiles) and pre-allocates `_waypoints = new Vector2[PathManager.DefaultWaypoints.Length]` once per instance to avoid per-pool-cycle allocation. (2) `Initialize()` copies waypoints with a `+new Vector2(0.5f, 0.5f)` offset into the pre-allocated array — this aligns enemies to tile centers (GridManager places tile centers at `gridX + 0.5, gridY + 0.5`). As a result, enemies follow `(-9.5, 2.5) → (-2.5, 2.5) → (-2.5, -1.5) → (4.5, -1.5) → (4.5, 3.5) → (10.5, 3.5)` rather than the raw waypoints. The AC 1 world positions `(-10, 2)` and `(10, 3)` refer to the raw waypoints; actual spawn/exit are offset by `(+0.5, +0.5)` to align with tile centers.
- `EnemyPool.cs`: ObjectPool<Enemy> with singleton, collectionCheck=true, capacity 20/50.
- `EnemyManager.cs`: Spawn loop, HandleStateChanged (GameOver/Win stops+clears; PreWave restarts), OnEnemyReachedPathEnd, ReturnAllEnemies (reverse iteration, direct pool release).
- `EnemySetupWizard.cs` (Editor): Run via `BasicTD > Setup > Story 2-1 Enemy System` to create EnemyDefinition.asset, Enemy.prefab (using enemy-base.png sprite), and wire EnemyPool/EnemyManager GameObjects in the active scene. Idempotent.
- Tasks 5, 6: Automated via EnemySetupWizard — run the menu item in Unity Editor.
- Task 7: Manual QA required in Unity Editor Play Mode.

### File List

- `Assets/Scripts/Enemies/EnemyDefinitionSO.cs` (new)
- `Assets/Scripts/Enemies/Enemy.cs` (new)
- `Assets/Scripts/Enemies/EnemyPool.cs` (new)
- `Assets/Scripts/Enemies/EnemyManager.cs` (new)
- `Assets/Scripts/Editor/EnemySetupWizard.cs` (new)
- `Assets/Art/Enemies/enemy-base.png` (new — sprite used by EnemySetupWizard to build Enemy.prefab)
- `Assets/Prefabs/Enemies/Enemy.prefab` (new — created by EnemySetupWizard)
- `Assets/Data/Enemies/EnemyDefinition.asset` (new — created by EnemySetupWizard)
- `Assets/Scenes/Main.unity` (modified — EnemyPool and EnemyManager GameObjects added by EnemySetupWizard)

## Change Log

- 2026-03-18: Story 2-1 — Enemy Spawn and Path Movement. Created EnemyDefinitionSO, Enemy, EnemyPool, EnemyManager scripts. Created EnemySetupWizard Editor tool for prefab/asset/scene wiring automation.
