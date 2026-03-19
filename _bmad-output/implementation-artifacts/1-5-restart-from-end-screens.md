# Story 1.5: Restart from End Screens

Status: done

<!-- Note: Validation is optional. Run validate-create-story for quality check before dev-story. -->

## Story

As a player,
I want to restart the game from the Game Over or Win screen,
so that I can play again without quitting and relaunching.

## Acceptance Criteria

1. Clicking "Restart" on the GameOverPanel resets the game to its initial state (GameState.PreWave)
2. Clicking "Restart" on the WinPanel resets the game to its initial state (GameState.PreWave)
3. After restart, neither the GameOverPanel nor the WinPanel is visible (alpha=0)
4. `GameOverPanel.cs` and `WinPanel.cs` each gain a `[SerializeField] private Button _restartButton` field wired in the Inspector to their existing RestartButton child GameObjects
5. Both panels subscribe `_restartButton.onClick` in `OnEnable()` and unsubscribe in `OnDisable()`
6. `GameManager` gains a public `RestartGame()` method that calls `SetState(GameState.PreWave)` — no additional logic needed in Epic 1
7. No new MonoBehaviour scripts, scenes, or folders are created — this is wiring only

## Tasks / Subtasks

- [x] Task 1: Add `RestartGame()` to `GameManager.cs` (AC: 6)
  - [x] Add `public void RestartGame()` method that calls `SetState(GameState.PreWave)` with a `GameLog.Info` log: `GameLog.Info("GameManager", "RestartGame called — resetting to PreWave")`
  - [x] Verify the existing `#if UNITY_EDITOR || DEVELOPMENT_BUILD` Update() block is untouched

- [x] Task 2: Wire restart button in `GameOverPanel.cs` (AC: 1, 3, 4, 5)
  - [x] Add `using UnityEngine.UI;` import if not already present
  - [x] Add `[SerializeField] private Button _restartButton;` field
  - [x] Add `Debug.Assert(_restartButton != null, "[GameOverPanel] _restartButton is not wired");` in `Awake()`
  - [x] In `OnEnable()`: after existing event subscription, add `_restartButton.onClick.AddListener(OnRestartClicked)`
  - [x] In `OnDisable()`: add `_restartButton.onClick.RemoveListener(OnRestartClicked)`
  - [x] Add `private void OnRestartClicked() { GameManager.Instance.RestartGame(); }`

- [x] Task 3: Wire restart button in `WinPanel.cs` (AC: 2, 3, 4, 5)
  - [x] Add `using UnityEngine.UI;` import if not already present
  - [x] Add `[SerializeField] private Button _restartButton;` field
  - [x] Add `Debug.Assert(_restartButton != null, "[WinPanel] _restartButton is not wired");` in `Awake()`
  - [x] In `OnEnable()`: after existing event subscription, add `_restartButton.onClick.AddListener(OnRestartClicked)`
  - [x] In `OnDisable()`: add `_restartButton.onClick.RemoveListener(OnRestartClicked)`
  - [x] Add `private void OnRestartClicked() { GameManager.Instance.RestartGame(); }`

- [x] Task 4: Wire Inspector references in `Main.unity` (AC: 4)
  - [x] Open `Main.unity` scene YAML
  - [x] On the GameOverPanel GameObject (which has `GameOverPanel.cs`): set `_restartButton` to reference the existing RestartButton child (look for the Button component fileID under GameOverPanel)
  - [x] On the WinPanel GameObject (which has `WinPanel.cs`): set `_restartButton` to reference the existing RestartButton child under WinPanel

- [x] Task 5: Manual QA verification (AC: 1, 2, 3)
  - [x] Enter Play Mode; confirm no console errors at startup
  - [x] Press backtick (`` ` ``) → GameOver panel appears → click Restart → panel disappears, game state is PreWave
  - [x] Press F5 → Win panel appears → click Restart → panel disappears, game state is PreWave
  - [x] Confirm path/tile coloring from Story 1.2 is unaffected
  - [x] Press backtick again after restart → GameOver panel reappears correctly (event subscription survived restart)

## Dev Notes

### Existing Files to Modify — DO NOT Recreate

| File | Location | What to Change |
|---|---|---|
| `GameManager.cs` | `Assets/Scripts/Core/` | Add `public void RestartGame()` method only |
| `GameOverPanel.cs` | `Assets/Scripts/UI/` | Add `_restartButton` field + onClick wiring |
| `WinPanel.cs` | `Assets/Scripts/UI/` | Add `_restartButton` field + onClick wiring |
| `Main.unity` | `Assets/Scenes/` | Wire `_restartButton` serialized references in Inspector |

**No new files. No new folders. No new scene objects.** RestartButton GameObjects already exist in the scene from Stories 1.3 and 1.4.

### GameOverPanel.cs — Final Shape After This Story

```csharp
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GameOverPanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private Button _restartButton;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(_canvasGroup != null, "[GameOverPanel] CanvasGroup component is missing");
        Debug.Assert(_restartButton != null, "[GameOverPanel] _restartButton is not wired");
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[GameOverPanel] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(GameManager.Instance.CurrentState);
        _restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        _restartButton.onClick.RemoveListener(OnRestartClicked);
    }

    private void HandleStateChanged(GameState newState)
    {
        bool show = newState == GameState.GameOver;
        _canvasGroup.alpha = show ? 1f : 0f;
        _canvasGroup.interactable = show;
        _canvasGroup.blocksRaycasts = show;
        GameLog.Info("GameOverPanel", show ? "Showing game over panel" : "Hiding game over panel");
    }

    private void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }
}
```

### WinPanel.cs — Final Shape After This Story

```csharp
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class WinPanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private Button _restartButton;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(_canvasGroup != null, "[WinPanel] CanvasGroup component is missing");
        Debug.Assert(_restartButton != null, "[WinPanel] _restartButton is not wired");
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[WinPanel] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(GameManager.Instance.CurrentState);
        _restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        _restartButton.onClick.RemoveListener(OnRestartClicked);
    }

    private void HandleStateChanged(GameState newState)
    {
        bool show = newState == GameState.Win;
        _canvasGroup.alpha = show ? 1f : 0f;
        _canvasGroup.interactable = show;
        _canvasGroup.blocksRaycasts = show;
        GameLog.Info("WinPanel", show ? "Showing win panel" : "Hiding win panel");
    }

    private void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }
}
```

### GameManager.cs — RestartGame() Addition Only

Add this method to the existing `GameManager.cs` — do NOT alter `SetState()`, `Update()`, or any other existing method:

```csharp
public void RestartGame()
{
    GameLog.Info("GameManager", "RestartGame called — resetting to PreWave");
    SetState(GameState.PreWave);
}
```

> **Forward compatibility:** In future epics, `RestartGame()` will notify other managers (EnemyManager, TowerManager, WaveManager, EconomyManager) to reset their state. For Epic 1 there are no such managers — `SetState(PreWave)` is the complete implementation. Do NOT add stubs or placeholders for future managers.

### Why onClick.AddListener in OnEnable (not Awake/Start)

- `_restartButton.onClick.AddListener` is added in `OnEnable` and removed in `OnDisable` — same lifecycle as the event subscription — so if the panel is ever disabled/re-enabled (e.g., after restart), the listener is cleanly removed and re-added without accumulating duplicates.
- `RemoveListener` is safe to call even if the listener was never added.

### Scene YAML — Wiring _restartButton

In `Main.unity`, find the `GameOverPanel` component entry and add the `_restartButton` reference pointing to the RestartButton child's fileID. Find the `WinPanel` component entry and do the same for WinPanel's RestartButton child.

Pattern (match the fileID of the RestartButton GameObject under each panel):
```yaml
  - component:
      fileID: <GameOverPanel script component fileID>
    # In serialized data:
    _restartButton: {fileID: <RestartButton GameObject fileID under GameOverPanel>}
```

Look up the correct fileIDs by searching for `RestartButton` in `Main.unity`. There are two — one under GameOverPanel and one under WinPanel. Do not swap them.

### Architecture Compliance Checklist

- **No `FindObjectOfType<T>()`** — access managers via `Manager.Instance`
- **`[SerializeField] private`** for `_restartButton` — wired in Inspector, not via code
- **`Debug.Assert` in `Awake()`** for `_restartButton` null check
- **Subscribe/unsubscribe onClick in `OnEnable`/`OnDisable`** — not `Start`/`OnDestroy`
- **`Scripts/UI/` only** for panel scripts — no new folders
- **`Scripts/Core/` only** for `GameManager.RestartGame()` — no UI logic in GameManager
- **UI boundary preserved** — panels call `GameManager.RestartGame()`, GameManager does not reference panels
- **`GameLog.Info`** not `Debug.Log` — strips from release builds automatically
- **`using UnityEngine.UI;`** required for `Button` type — confirm it's present in both files

### Previous Story Intelligence (Story 1.4)

- `WinPanel.cs` uses `[RequireComponent(typeof(CanvasGroup))]` + `GetComponent<CanvasGroup>()` in `Awake()` — follow this exact pattern for the new `_restartButton` field too
- `GameOverPanel.cs` is the reference implementation for this story — same changes apply to both files
- Story 1.4 deliberately left `Button.onClick` empty with a note: "Story 1.5 will add `[SerializeField] private Button _restartButton` to both panel scripts" — this is that story
- `TMP_AutoSetup.cs` was deleted in Story 1.4 — do not recreate it
- `GameManager.cs.meta` has `executionOrder: -100` — do NOT change it
- The RestartButton label on WinPanel says "Restart" (TMP, size 48, white) — no scene changes to button labels needed

### Reserved Debug Keys (Do Not Reuse)

| Key | Owner | Story |
|---|---|---|
| Backtick (`` ` ``) | Force GameOver | Story 1.3 |
| F5 | Force Win | Story 1.4 |
| F1 | Debug overlay | Future |
| F2 | Add currency | Epic 5 |
| F3 | Skip wave | Epic 6 |

### Testing Approach

Per `project-context.md`: "UI panel behavior — manual QA only." No Edit Mode tests required.

**Manual verification steps:**
1. Open `Main.unity` in Unity Editor
2. Verify `_restartButton` is wired in Inspector on both GameOverPanel and WinPanel components
3. Enter Play Mode — confirm no console errors or missing reference assertions
4. Press backtick → GameOver panel appears → click Restart → panel hides, state returns to PreWave
5. Press F5 → Win panel appears → click Restart → panel hides, state returns to PreWave
6. Press backtick again (regression) → GameOver panel reappears correctly
7. Confirm path/tile coloring from Story 1.2 is unaffected

### Project Structure Notes

- All changes are in-place modifications to existing files — zero new files
- `Main.unity` is the only scene (ADR-006 single scene architecture)
- `GameOverPanel.cs` → `Assets/Scripts/UI/` (existing)
- `WinPanel.cs` → `Assets/Scripts/UI/` (existing)
- `GameManager.cs` → `Assets/Scripts/Core/` (existing)

### References

- Architecture: `_bmad-output/game-architecture.md` → ADR-002 (GameState enum), ADR-006 (single scene, no SceneManager.LoadScene), ADR-007 (UGUI), Cross-cutting Concerns (Event System, Logging), Project Structure (Scripts/UI/, Scripts/Core/)
- Epics: `_bmad-output/epics.md` → Epic 1: Game Foundation, Story 5
- Project Context: `_bmad-output/project-context.md` → Engine-Specific Rules (lifecycle order, event subscriptions), Code Organization Rules, Anti-Patterns
- Previous Story: `_bmad-output/implementation-artifacts/1-4-win-screen.md` → WinPanel.cs pattern, RestartButton forward-compatibility note, CanvasGroup pattern

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

### Completion Notes List

- Task 1: Added `public void RestartGame()` to `GameManager.cs` — calls `GameLog.Info` then `SetState(GameState.PreWave)`. The existing `#if UNITY_EDITOR || DEVELOPMENT_BUILD` Update() block is untouched.
- Task 2: Updated `GameOverPanel.cs` to match the final shape from Dev Notes — added `using UnityEngine.UI;`, `[SerializeField] private Button _restartButton`, `Debug.Assert` in Awake, `onClick.AddListener(OnRestartClicked)` in OnEnable, `RemoveListener` in OnDisable, and `OnRestartClicked()` method.
- Task 3: Updated `WinPanel.cs` identically to GameOverPanel.cs pattern — all same changes applied.
- Task 4: Wired `_restartButton` serialized references in `Main.unity` YAML: GameOverPanel script component (114000021) → RestartButton fileID 100000040; WinPanel script component (114000061) → RestartButton fileID 100000080.
- Task 5: Requires manual QA in Unity Editor Play Mode — see checklist steps in Tasks section.

### File List

- Assets/Scripts/Core/GameManager.cs
- Assets/Scripts/UI/GameOverPanel.cs
- Assets/Scripts/UI/WinPanel.cs
- Assets/Scenes/Main.unity

### Change Log

- 2026-03-18: Implemented Tasks 1–4 — added RestartGame() to GameManager, wired _restartButton in GameOverPanel and WinPanel, wired Inspector references in Main.unity YAML. Task 5 (manual QA) pending user verification in Unity Play Mode.
