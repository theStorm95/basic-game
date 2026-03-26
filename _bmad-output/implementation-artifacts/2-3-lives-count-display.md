# Story 2.3: Lives Count Display

Status: done

## Story

As a player,
I want to see my current lives count in the UI,
so that I know how many lives I have remaining at a glance.

## Acceptance Criteria

1. A lives count text element is visible in the HUD at all times during gameplay
2. The text displays the current lives count (e.g., `"Lives: 3"`)
3. The text updates immediately when a life is lost — subscribes to `GameManager.Instance.OnLivesChanged`
4. The text initializes correctly on scene load (shows `GameConstants.MAX_LIVES` = 3)
5. The text updates correctly on restart (shows `GameConstants.MAX_LIVES` after `RestartGame()`)
6. `HudController` subscribes in `OnEnable` and unsubscribes in `OnDisable` (same pattern as `GameOverPanel`)
7. No changes to `GameManager.cs`, `EnemyManager.cs`, or any non-UI file

## Tasks / Subtasks

- [x] Task 1: Create `HudController.cs` in `Assets/Scripts/UI/` (AC: 1–6)
  - [x] `[SerializeField] private Text _livesText;` (or `TextMeshProUGUI` — see Dev Notes)
  - [x] `Awake()` — `Debug.Assert(_livesText != null, "[HudController] _livesText not wired")`
  - [x] `OnEnable()` — subscribe to `GameManager.Instance.OnLivesChanged`, then call `UpdateLivesDisplay(GameManager.Instance.Lives)` immediately to sync initial value
  - [x] `OnDisable()` — null-guard and unsubscribe from `OnLivesChanged`
  - [x] `UpdateLivesDisplay(int lives)` — sets `_livesText.text = $"Lives: {lives}"`; logs via `GameLog.Info`

- [x] Task 2: Wire HudController in Unity Editor (AC: 1, 4)
  - [x] Add a Text (or TextMeshProUGUI) UI element to the Canvas in `Main.unity` (e.g., top-left corner of HUD)
  - [x] Add `HudController` component to an appropriate GameObject (e.g., a "HUD" child of Canvas)
  - [x] Wire `_livesText` field to the Text element in the Inspector
  - [x] Enter Play Mode and confirm text shows "Lives: 3" on scene load

- [x] Task 3: Manual QA (AC: 1–7)
  - [x] Enter Play Mode — HUD shows "Lives: 3"
  - [x] Let enemy reach end — HUD updates to "Lives: 2" immediately
  - [x] Let 2 more enemies reach end — HUD shows "Lives: 0", does not go below
  - [x] Press Restart — HUD resets to "Lives: 3"
  - [x] Confirm backtick (GameOver) and F5 (Win) cheat keys still work
  - [x] Confirm no change to enemy spawning, path movement, or pool behavior

## Dev Notes

### New File

| File | Location | Purpose |
|------|----------|---------|
| `HudController.cs` | `Assets/Scripts/UI/` | Subscribes to `OnLivesChanged`, updates lives text in HUD |

No changes to `GameManager.cs`, `EnemyManager.cs`, or any other existing file.

### HudController.cs — Implementation Shape

```csharp
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Text _livesText;

    private void Awake()
    {
        Debug.Assert(_livesText != null, "[HudController] _livesText is not wired");
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[HudController] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnLivesChanged += UpdateLivesDisplay;
        UpdateLivesDisplay(GameManager.Instance.Lives);  // sync immediately — no event fires on scene load
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnLivesChanged -= UpdateLivesDisplay;
    }

    private void UpdateLivesDisplay(int lives)
    {
        _livesText.text = $"Lives: {lives}";
        GameLog.Info("HudController", $"Lives display updated: {lives}");
    }
}
```

### Text Component — Legacy vs TMPro

Check the scene before implementing:
- If any existing Canvas text uses `TextMeshProUGUI` → use `TextMeshProUGUI` with `using TMPro;` instead of `using UnityEngine.UI;`
- If no text elements exist yet → prefer `TextMeshProUGUI` (Unity 6 recommended)
- If scene already uses `UnityEngine.UI.Text` → match that

The logic is identical regardless of component type — only the type name and namespace change.

### Why Initialize on OnEnable

`OnLivesChanged` only fires when lives change. On scene load, lives start at `MAX_LIVES` but no event fires — the event was declared in Story 2.2 for use here. Calling `UpdateLivesDisplay(GameManager.Instance.Lives)` immediately in `OnEnable` after subscribing ensures the text shows the correct initial value (3) without waiting for the first life loss. This is the same pattern `GameOverPanel` uses: it calls `HandleStateChanged(GameManager.Instance.CurrentState)` directly in `OnEnable`.

### OnLivesChanged — Already Fully Wired

`GameManager.OnLivesChanged` and `GameManager.Lives` were both added in Story 2.2 and are confirmed present in `Assets/Scripts/Core/GameManager.cs`. The event fires:
1. Each call to `LoseLife()` — with new `_lives` value
2. Each call to `RestartGame()` — with `GameConstants.MAX_LIVES` (3), before state changes to `PreWave`

No modifications to `GameManager.cs` are needed.

### Architecture Compliance

- **Boundary**: `HudController` is in `Scripts/UI/` — reads from `GameManager` via event, writes nothing to game state
- **No `FindObjectOfType<T>()`** — access via `GameManager.Instance`
- **`[SerializeField] private`** — `_livesText` follows the project pattern
- **`Debug.Assert`** — validates both field wiring (`Awake`) and singleton availability (`OnEnable`)
- **Subscribe in `OnEnable`, unsubscribe in `OnDisable`** — prevents memory leaks on disable/destroy
- **`GameLog.Info` not `Debug.Log`** — strips from release builds automatically

### Scene Wiring

Add to `Assets/Scenes/Main.unity`:
- A Text (or TextMeshProUGUI) element positioned in the HUD area (top-left recommended)
- A GameObject with `HudController` component (can be a "HUD" child of the Canvas root)
- Wire `_livesText` in the Inspector

Existing `GameOverPanel` and `WinPanel` GameObjects are unchanged. No other scene changes.

### Story Scope Boundary

This story does NOT include:
- **Game over on 0 lives** — Story 2.4 (adds `if (_lives == 0) SetState(GameState.GameOver)` to `LoseLife()`)
- **Currency, wave counter, or other HUD elements** — future epics
- **Any change to `GameManager`, `EnemyManager`, `Enemy`** — UI only

### Project Structure After This Story

```
Assets/Scripts/UI/
├── HudController.cs     <- NEW (this story)
├── GameOverPanel.cs     <- unchanged
└── WinPanel.cs          <- unchanged
```

### Previous Story Intelligence (Story 2.2)

Confirmed from Story 2.2 implementation and source verification:
- `GameManager.cs` has `private int _lives`, `public int Lives => _lives`, `public event System.Action<int> OnLivesChanged`, and `LoseLife()` — all confirmed in source
- `RestartGame()` fires `OnLivesChanged` BEFORE calling `SetState(PreWave)` — HudController receives the reset value before enemy spawn restarts
- `EnemyManager.OnEnemyReachedPathEnd` calls `GameManager.Instance.LoseLife()` — the full life-loss chain works with no changes from this story
- Cheat key `` ` `` triggers GameOver; F5 triggers Win — both remain functional

### References

- Architecture: `_bmad-output/game-architecture.md` → Architectural Boundaries (`UI/` subscribes to events only, `Core/` has no outbound imports), ADR-007 (UGUI), Event System pattern
- Project Context: `_bmad-output/project-context.md` → Anti-Patterns (no FindObjectOfType), Event System Gotchas (null-check before invoke, unsubscribe in OnDisable), Lifecycle Order (Awake/OnEnable/Start)
- Previous Story: `_bmad-output/implementation-artifacts/2-2-lose-life-on-enemy-reach-end.md` → `OnLivesChanged` shape, RestartGame reset order, GameManager final shape
- Source: `Assets/Scripts/Core/GameManager.cs` — `OnLivesChanged` event and `Lives` property confirmed present (lines 12, 34)
- Source: `Assets/Scripts/UI/GameOverPanel.cs` — subscribe/unsubscribe pattern and immediate-sync in `OnEnable` to mirror exactly

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

### Completion Notes List

- Task 1 complete: Created `HudController.cs` using `UnityEngine.UI.Text` (matches existing GameOverPanel/WinPanel pattern). Subscribes to `OnLivesChanged` in `OnEnable` with immediate sync, unsubscribes in `OnDisable` with null-guard. `Awake` asserts field wiring.
- Tasks 2 and 3 completed manually in Unity Editor: Text element added to Canvas, `HudController` component wired to GameObject, `_livesText` field wired in Inspector, and manual QA confirmed correct behavior across all scenarios.

### File List

- `Assets/Scripts/UI/HudController.cs` (new)
- `Assets/Scripts/UI/HudController.cs.meta` (new)
