# Story 2.4: Game Over on Zero Lives

Status: done

## Story

As a player,
I want the game to end when all 3 lives are lost,
so that losing all lives has a consequence and the game over screen appears.

## Acceptance Criteria

1. When `LoseLife()` causes `_lives` to reach 0, `SetState(GameState.GameOver)` is called immediately after `OnLivesChanged` fires
2. The Game Over screen appears when lives drop to 0 (via `GameOverPanel` subscribing to `OnStateChanged`)
3. The `LoseLife()` guard (`if (_lives == 0) return`) prevents calls once already at 0 — no double-trigger
4. No regression to: lives display, restart, cheat keys (backtick / F5)
5. Only `GameManager.cs` is modified — no other file changes

## Tasks / Subtasks

- [x] Task 1: Modify `GameManager.LoseLife()` to trigger game over at 0 lives (AC: 1, 3, 5)
  - [x] After `OnLivesChanged?.Invoke(_lives)`, add: `if (_lives == 0) SetState(GameState.GameOver);`
  - [x] Confirm the existing `if (_lives == 0) return;` guard at top of method is preserved (no double-trigger possible)
  - [x] Confirm `GameLog.Info` line still present after the new state call

- [x] Task 2: Manual QA (AC: 2, 4)
  - [x] Enter Play Mode — HUD shows "Lives: 3", no game over screen
  - [x] Let 3 enemies reach end — Game Over screen appears after third life lost
  - [x] Confirm HUD shows "Lives: 0" and game over screen appears simultaneously (same frame)
  - [x] Press Restart on Game Over screen — HUD resets to "Lives: 3", game over screen dismissed
  - [x] Let enemy reach end during a new session — HUD updates to "Lives: 2", no premature game over
  - [x] Press backtick (cheat) — Game Over screen appears immediately, no regression
  - [x] Press F5 (cheat) — Win screen appears immediately, no regression
  - [x] Confirm enemy spawning, path movement, and HUD display are unaffected

## Dev Notes

### Change Required

Single line addition to `GameManager.cs` `LoseLife()` method:

```csharp
public void LoseLife()
{
    if (_lives == 0) return;                        // existing guard — no change
    _lives = Mathf.Max(0, _lives - 1);             // existing — no change
    OnLivesChanged?.Invoke(_lives);                 // existing — fires HUD update first
    if (_lives == 0) SetState(GameState.GameOver);  // NEW — trigger game over after HUD notified
    GameLog.Info("GameManager", $"Life lost — {_lives} remaining");  // existing — no change
}
```

**Event order matters:** `OnLivesChanged` fires before `SetState(GameOver)`, so `HudController` updates the lives display to "0" before `GameOverPanel` receives `OnStateChanged` and shows itself. This ensures the player sees "Lives: 0" under the game over panel.

### Why This Works Without Other Changes

- `GameOverPanel` already subscribes to `GameManager.Instance.OnStateChanged` and shows itself on `GameState.GameOver` — confirmed present from Story 1.3
- The existing `if (_lives == 0) return` guard at the top of `LoseLife()` means once game over is triggered, subsequent calls (e.g. more enemies reaching end before `GameOverPanel` disables enemy spawning) are silently no-ops — no double game over
- `RestartGame()` already resets `_lives = GameConstants.MAX_LIVES` and calls `SetState(PreWave)` — no changes needed for restart

### Architecture Compliance

- Only `Scripts/Core/GameManager.cs` changes — no boundary violations
- `SetState()` is already used within `GameManager` (cheat keys call it directly) — this is the same pattern
- No new events, no new fields, no new MonoBehaviours

### What This Story Does NOT Include

- Disabling enemy spawning on game over — `EnemyManager` already responds to `OnStateChanged` to stop spawning (Story 2.1)
- Any new UI — `GameOverPanel` from Story 1.3 handles display
- Any changes to restart flow — Story 1.5 handles restart

### References

- Source: `Assets/Scripts/Core/GameManager.cs` — current `LoseLife()` at line 44
- Source: `Assets/Scripts/UI/GameOverPanel.cs` — subscribes to `OnStateChanged`, shows on `GameState.GameOver`
- Story 2.3 Dev Notes: "Game over on 0 lives — Story 2.4 (adds `if (_lives == 0) SetState(GameState.GameOver)` to `LoseLife()`)"
- Architecture: `_bmad-output/game-architecture.md` — event ordering, Core boundary rules

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

### Completion Notes List

- Task 1 complete: Added `if (_lives == 0) SetState(GameState.GameOver);` to `LoseLife()` in `GameManager.cs` after `OnLivesChanged?.Invoke(_lives)`. Existing guard at top of method preserved. Event order ensures HUD updates to "0" before game over screen appears.
- Task 2 complete: Manual QA passed — game over screen fires on third life lost, HUD shows "Lives: 0", restart resets correctly, cheat keys unaffected.

### File List

- `Assets/Scripts/Core/GameManager.cs` (modified)

### Change Log

- Added game-over trigger to `GameManager.LoseLife()` — fires `SetState(GameState.GameOver)` when `_lives` reaches 0, after notifying HUD via `OnLivesChanged` (Date: 2026-03-25)
