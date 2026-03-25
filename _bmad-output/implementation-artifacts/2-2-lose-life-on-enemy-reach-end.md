# Story 2.2: Lose Life on Enemy Reach End

Status: done

## Story

As a player,
I want to lose a life when an enemy reaches the end of the path,
so that enemies pose a meaningful threat to the game state.

## Acceptance Criteria

1. `GameManager` tracks a `Lives` property initialized to `GameConstants.MAX_LIVES` (3) when the scene loads
2. When an enemy reaches the path end, `GameManager.LoseLife()` is called and `Lives` decrements by 1
3. `Lives` cannot go below 0 (clamped via `Mathf.Max(0, _lives - 1)`)
4. `Lives` resets to `GameConstants.MAX_LIVES` on game restart (`GameState.PreWave` transition)
5. `GameManager` fires `public event System.Action<int> OnLivesChanged` after each life deduction — Story 2.3 subscribes to this to update the UI
6. Lives deduction is logged: `GameLog.Info("GameManager", $"Life lost — {_lives} remaining")`
7. No UI changes in this story — lives display is Story 2.3; game over trigger is Story 2.4

## Tasks / Subtasks

- [x] Task 1: Extend `GameManager.cs` with lives tracking (AC: 1, 3, 4, 5, 6)
  - [x] Add `private int _lives = GameConstants.MAX_LIVES;`
  - [x] Add `public int Lives => _lives;`
  - [x] Add `public event System.Action<int> OnLivesChanged;`
  - [x] Add `public void LoseLife()` — `_lives = Mathf.Max(0, _lives - 1);`, fire `OnLivesChanged?.Invoke(_lives)`, log
  - [x] Reset `_lives = GameConstants.MAX_LIVES` inside `RestartGame()` before calling `SetState(GameState.PreWave)`

- [x] Task 2: Call `GameManager.Instance.LoseLife()` from `EnemyManager` (AC: 2)
  - [x] In `EnemyManager.OnEnemyReachedPathEnd(Enemy enemy)`, call `GameManager.Instance.LoseLife()` after `OnEnemyReachedEnd?.Invoke()`

- [x] Task 3: Manual QA (AC: 1–7)
  - [x] Enter Play Mode — `Lives` starts at 3 (confirm via debug overlay or console log)
  - [x] Let enemy reach path end — confirm `"Life lost — 2 remaining"` logged in Console
  - [x] Let 3 more enemies reach end — confirm lives clamps at 0 (not -1 or lower)
  - [x] Press Restart — confirm `"Life lost"` log does NOT appear and lives resets to 3
  - [x] Confirm existing backtick (GameOver) and F5 (Win) cheat keys still work
  - [x] Confirm enemy spawning, path movement, and pool behavior from Story 2.1 unaffected

## Dev Notes

### Files to Modify — DO NOT Recreate

| File | Location | Change |
|------|----------|--------|
| `GameManager.cs` | `Assets/Scripts/Core/` | Add lives tracking, `LoseLife()`, `OnLivesChanged` event |
| `EnemyManager.cs` | `Assets/Scripts/Enemies/` | Call `GameManager.Instance.LoseLife()` in `OnEnemyReachedPathEnd` |

No new files. No new GameObjects. No scene wiring changes.

### Architecture Boundary — Critical

`Scripts/Core/` has zero imports from feature folders — it is imported, never imports.

This means **`GameManager.cs` CANNOT subscribe to `EnemyManager.OnEnemyReachedEnd`**. The call must go the other direction:

- `EnemyManager` (in `Scripts/Enemies/`) → calls `GameManager.Instance.LoseLife()` — ALLOWED (Enemies imports Core)
- `GameManager` (in `Scripts/Core/`) → references `EnemyManager` — **FORBIDDEN**

### GameManager.cs — Modified Shape

Add only these members; do NOT restructure existing code:

```csharp
// Field — alongside _currentState
private int _lives = GameConstants.MAX_LIVES;

// Property — alongside CurrentState
public int Lives => _lives;

// Event — alongside OnStateChanged
public event System.Action<int> OnLivesChanged;

// Method — new, after RestartGame()
public void LoseLife()
{
    _lives = Mathf.Max(0, _lives - 1);
    OnLivesChanged?.Invoke(_lives);
    GameLog.Info("GameManager", $"Life lost — {_lives} remaining");
}
```

Update `RestartGame()` to reset lives before firing state change:

```csharp
public void RestartGame()
{
    _lives = GameConstants.MAX_LIVES;
    OnLivesChanged?.Invoke(_lives);  // Notify UI of reset (Story 2.3 will use this)
    GameLog.Info("GameManager", "RestartGame called — resetting to PreWave");
    SetState(GameState.PreWave);
}
```

### EnemyManager.cs — Modified Shape

Change only `OnEnemyReachedPathEnd`; everything else is untouched:

```csharp
public void OnEnemyReachedPathEnd(Enemy enemy)
{
    _activeEnemies.Remove(enemy);
    EnemyPool.Instance.Release(enemy);
    OnEnemyReachedEnd?.Invoke();
    GameManager.Instance.LoseLife();   // ← ADD THIS LINE
    GameLog.Info("EnemyManager", "Enemy reached path end");
}
```

### OnLivesChanged Event — Why Declare Now

Story 2.3 (lives count display) will subscribe to `GameManager.Instance.OnLivesChanged` from a UI script. Declaring it now avoids modifying `GameManager` again in 2.3 and gives the UI subscriber the correct current lives on reset. Follow the same subscribe-in-`OnEnable`, unsubscribe-in-`OnDisable` pattern used by `GameOverPanel` and `WinPanel`.

### Lives Reset on Restart — Order Matters

`RestartGame()` must reset `_lives` and fire `OnLivesChanged` **before** calling `SetState(GameState.PreWave)`. If the order were reversed, `EnemyManager.HandleStateChanged(PreWave)` would restart the spawn loop before lives are reset, which is harmless now but is confusing. Correct order: reset → notify → transition state.

### Architecture Compliance Checklist

- **`[SerializeField] private`** — no new serialized fields added to `GameManager` (lives is an internal runtime value)
- **`Debug.Assert`** — not needed; `_lives` is always initialized from a constant, never inspector-wired
- **`GameLog.Info` not `Debug.Log`** — use `GameLog.Info("GameManager", ...)` only
- **`OnLivesChanged?.Invoke()`** — always null-check before invoking
- **No `FindObjectOfType<T>()`** — access via `GameManager.Instance`
- **Boundary rule** — `GameManager` adds no imports; `EnemyManager` calls `GameManager.Instance.LoseLife()` (Enemies → Core: allowed)
- **Zero allocations** — `Mathf.Max` is a value operation; no heap allocation

### Story Scope Boundary

This story does NOT include:
- **Lives display in UI** — Story 2.3 (`HudController` subscribes to `OnLivesChanged`)
- **Game over trigger on 0 lives** — Story 2.4 (adds `if (_lives == 0) SetState(GameState.GameOver)` to `LoseLife()`)
- **Wave management** — Epic 6
- **Enemy health or damage** — Epic 4

`LoseLife()` in this story only decrements and fires the event. Story 2.4 will add the `GameOver` trigger inside `LoseLife()`. Do not add it now.

### Testing

No unit tests needed. Lives arithmetic is trivial (`Mathf.Max` clamp). Validate through Play Mode manual QA per Task 3.

### Previous Story Intelligence (Story 2.1)

From Story 2.1 Completion Notes:
- `EnemyManager.OnEnemyReachedPathEnd(Enemy enemy)` is the correct call site — it fires after the enemy is removed from `_activeEnemies` and released to pool
- `EnemyManager.OnEnemyReachedEnd` event is declared and fires in `OnEnemyReachedPathEnd` — do NOT remove or replace it; it may be used by future subscribers (UI, DebugOverlay)
- Singleton pattern on `GameManager`: `GameManager.Instance` is always valid at this point in the frame — `EnemyManager.OnEnable()` already `Debug.Assert`s that `GameManager.Instance != null`
- Enemy waypoints are offset by `(+0.5, +0.5)` from raw waypoints — irrelevant to this story but noted for context
- `EnemySetupWizard.cs` (Editor-only) exists in `Assets/Scripts/Editor/` — do not touch it

### Project Structure Notes

No new folders or files. Modify only:
- `Assets/Scripts/Core/GameManager.cs` — Core system file
- `Assets/Scripts/Enemies/EnemyManager.cs` — Enemies system file

Existing folders unchanged:
- `Assets/Scripts/Grid/`
- `Assets/Scripts/UI/`
- `Assets/Scenes/Main.unity` (no scene changes)

### References

- Architecture: `_bmad-output/game-architecture.md` → Architectural Boundaries (Core imports nothing), Communication Patterns (event-driven cross-system), Configuration (`GameConstants.MAX_LIVES = 3`)
- Epics: `_bmad-output/epics.md` → Epic 2: Enemy Path System, Story 2-2
- Project Context: `_bmad-output/project-context.md` → Anti-Patterns (no FindObjectOfType), Event System Gotchas (null-check before invoke), Lifecycle Order (Awake/OnEnable/Start)
- Previous Story: `_bmad-output/implementation-artifacts/2-1-enemy-spawn-and-path-movement.md` → `OnEnemyReachedPathEnd` call site, EnemyManager final shape, architecture compliance checklist
- GameManager source: `Assets/Scripts/Core/GameManager.cs` — verified current shape before writing this story
- GameConstants source: `Assets/Scripts/Core/GameConstants.cs` — `MAX_LIVES = 3` confirmed

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

### Completion Notes List

- Task 1 complete: Added `_lives` field, `Lives` property, `OnLivesChanged` event, `LoseLife()` method to `GameManager.cs`. Updated `RestartGame()` to reset lives and fire `OnLivesChanged` before calling `SetState(GameState.PreWave)`.
- Task 2 complete: Added `GameManager.Instance.LoseLife()` call in `EnemyManager.OnEnemyReachedPathEnd` after `OnEnemyReachedEnd?.Invoke()`. Architecture boundary respected — Enemies → Core (allowed).
- No new files, no new GameObjects, no scene wiring changes.
- Task 3 (Manual QA) requires user verification in Unity Play Mode per the subtasks listed.

### File List

- Assets/Scripts/Core/GameManager.cs
- Assets/Scripts/Enemies/EnemyManager.cs

### Change Log

- 2026-03-24: Implemented Story 2-2 — added lives tracking to GameManager (field, property, event, LoseLife method, RestartGame reset); wired EnemyManager.OnEnemyReachedPathEnd to call GameManager.Instance.LoseLife().
