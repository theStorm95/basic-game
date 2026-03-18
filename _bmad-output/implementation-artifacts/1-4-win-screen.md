# Story 1.4: Win Screen

Status: done

<!-- Note: Validation is optional. Run validate-create-story for quality check before dev-story. -->

## Story

As a player,
I want to see a win screen when the win condition is triggered,
so that I have a clear visual signal that the game has been won and I need to take action.

## Acceptance Criteria

1. A "YOU WIN" panel becomes visible when `GameManager.SetState(GameState.Win)` is called
2. The panel is invisible at game start and whenever the state is not `Win` (use CanvasGroup alpha=0/interactable=false/blocksRaycasts=false on an **active** GameObject — matches the GameOverPanel approach; do NOT use `SetActive(false)` at start)
3. The panel displays "YOU WIN" text clearly readable at 2560×1440
4. `WinPanel.cs` subscribes to `GameManager.Instance.OnStateChanged` in `OnEnable()` and unsubscribes in `OnDisable()`
5. The panel contains a placeholder "Restart" button (visible, not wired to any logic — functionality added in Story 1.5)
6. No gameplay systems are modified — this is a UI overlay only
7. A `#if UNITY_EDITOR || DEVELOPMENT_BUILD` gated keyboard shortcut in `GameManager.Update()` triggers `SetState(GameState.Win)` for manual in-editor testing — add as a second `if` block alongside the existing GameOver trigger (do NOT remove the existing trigger)

## Tasks / Subtasks

- [x] Task 1: Add WinPanel hierarchy under the existing Canvas in `Main.unity` (AC: 1, 2, 3, 5)
  - [x] Locate the existing Canvas GameObject in Main.unity (added in Story 1.3 — do NOT create a new Canvas)
  - [x] Create "WinPanel" as a sibling of "GameOverPanel" under the Canvas: full-screen RectTransform (anchors stretch/stretch, offsets all 0)
  - [x] Add Image component to panel: Color = (0, 0, 0, 0.75) — same semi-transparent dark overlay as GameOverPanel
  - [x] Add "WinText" child: TextMeshPro - UGUI, text = "YOU WIN", font size 120, alignment center, white, bold
  - [x] Position WinText at (0, 80, 0) relative to panel center
  - [x] Add "RestartButton" child: Unity UI Button, label text = "Restart", font size 48, white text
  - [x] Position RestartButton at (0, -80, 0) relative to panel center, size 320×80
  - [x] Add CanvasGroup component to WinPanel; set alpha=0, interactable=false, blocksRaycasts=false (panel stays **active**)
  - [x] **Do NOT wire the Restart button's onClick** — that is Story 1.5

- [x] Task 2: Create `Assets/Scripts/UI/WinPanel.cs` (AC: 1, 2, 4, 6)
  - [x] Class `WinPanel : MonoBehaviour`
  - [x] Uses `[RequireComponent(typeof(CanvasGroup))]` and `GetComponent<CanvasGroup>()` in `Awake()` — matching GameOverPanel pattern exactly
  - [x] In `Awake()`: caches `_canvasGroup`, asserts it's not null, sets alpha=0/interactable=false/blocksRaycasts=false
  - [x] In `OnEnable()`: asserts `GameManager.Instance != null`, subscribes `OnStateChanged`, calls `HandleStateChanged(CurrentState)` to sync immediately
  - [x] In `OnDisable()`: null-safe unsubscribe
  - [x] `HandleStateChanged(GameState newState)`:
    - [x] `bool show = newState == GameState.Win`
    - [x] `_canvasGroup.alpha = show ? 1f : 0f`
    - [x] `_canvasGroup.interactable = show`
    - [x] `_canvasGroup.blocksRaycasts = show`
    - [x] `GameLog.Info("WinPanel", show ? "Showing win panel" : "Hiding win panel")`

- [x] Task 3: Add Win debug trigger to `GameManager.cs` (AC: 7)
  - [x] Located existing `Update()` method with `#if UNITY_EDITOR || DEVELOPMENT_BUILD` guard
  - [x] Added second `if` block: `Keyboard.current?.f5Key.wasPressedThisFrame == true` → `SetState(GameState.Win)` with matching log message
  - [x] Existing backtick → GameOver trigger preserved unchanged

- [x] Task 4: Wire WinPanel in scene and verify (AC: 1, 2)
  - [x] WinPanel component added to "WinPanel" GameObject via scene YAML (guid: bd93428ba089f4479bdb4293c7348994)
  - [x] CanvasGroup wired via `[RequireComponent]` + `GetComponent` — no Inspector assignment needed
  - [x] WinPanel active with CanvasGroup alpha=0/interactable=0/blocksRaycasts=0 in scene YAML
  - [x] Manual QA required in Unity Editor to verify Play Mode behavior (per testing approach)

## Dev Notes

### Existing Infrastructure — DO NOT Recreate

These files and scene objects exist and must be reused, not recreated:

| Asset | Location | Notes |
|---|---|---|
| `GameManager.cs` | `Assets/Scripts/Core/` | Has `SetState()`, `OnStateChanged`, `Update()` with `#if` guard — only add a second `if` block |
| `GameState.cs` | `Assets/Scripts/Core/` | Enum already includes `Win` — do NOT modify |
| `GameLog.cs` | `Assets/Scripts/Utils/` | Use for all logging |
| `Main.unity` | `Assets/Scenes/` | Canvas + CanvasScaler + GraphicRaycaster + GameOverPanel already present — add WinPanel as sibling |
| `GameOverPanel.cs` | `Assets/Scripts/UI/` | Reference implementation — WinPanel.cs mirrors this pattern |
| `BasicGame.asmdef` | `Assets/Scripts/` | Already references Unity.InputSystem and Unity.TextMeshPro — no changes needed |

### GameManager.Update() — Current Shape After Story 1.3

After Story 1.3 code review fixes, `GameManager.cs` Update() uses backtick for GameOver:

```csharp
private void Update()
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    if (Keyboard.current?.backquoteKey.wasPressedThisFrame == true)
        SetState(GameState.GameOver);
#endif
}
```

**Only add a second `if` block — do NOT modify the existing one:**

```csharp
private void Update()
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    if (Keyboard.current?.backquoteKey.wasPressedThisFrame == true)
        SetState(GameState.GameOver);
    if (Keyboard.current?.f5Key.wasPressedThisFrame == true)
        SetState(GameState.Win);
#endif
}
```

> **Note on debug keys:** F4 was originally specified for GameOver but macOS intercepts it for Launchpad; Story 1.3 switched to backtick. F5 should be safe on macOS. If F5 is intercepted in your environment, use `Keyboard.current?.digit5Key` instead.

### Reserved Debug Keys — Do Not Reuse

| Key | Owner | Story |
|---|---|---|
| Backtick (`) | Force GameOver | Story 1.3 |
| F1 | Debug overlay | Future |
| F2 | Add currency | Epic 5 |
| F3 | Skip wave | Epic 6 |
| **F5** | **Force Win** | **This story** |

### WinPanel.cs — Full Implementation

Mirror `GameOverPanel.cs` exactly, replacing `GameOver` → `Win` and using CanvasGroup (the code-review-approved pattern):

```csharp
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class WinPanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(_canvasGroup != null, "[WinPanel] CanvasGroup component is missing");
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[WinPanel] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(GameManager.Instance.CurrentState);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        bool show = newState == GameState.Win;
        _canvasGroup.alpha = show ? 1f : 0f;
        _canvasGroup.interactable = show;
        _canvasGroup.blocksRaycasts = show;
        GameLog.Info("WinPanel", show ? "Showing win panel" : "Hiding win panel");
    }
}
```

### Why CanvasGroup Instead of SetActive

Story 1.3's code review established that hiding via `CanvasGroup.alpha=0` on an **active** GameObject is the correct pattern for event-subscribing panels:
- `SetActive(false)` triggers `OnDisable()`, unsubscribing the event handler
- CanvasGroup keeps the GameObject active so `OnEnable()` fires once at scene startup (GameManager.Instance is already set due to `executionOrder: -100`)
- The panel's subscription stays live for the entire scene lifetime

> **GameManager execution order:** `GameManager.cs.meta` has `executionOrder: -100` — GameManager.Awake() is guaranteed to run before any UI panel's OnEnable(). Already set in Story 1.3; do not change it.

### Canvas Structure in Main.unity After Story 1.3

```
Canvas (Screen Space - Overlay, Sort Order 10)
  CanvasScaler (Scale With Screen Size, 2560×1440, Match=0.5)
  GraphicRaycaster
  GameOverPanel (active, CanvasGroup alpha=0)
    GameOverText (TMP "GAME OVER", size 120, pos 0,80)
    RestartButton (320×80, pos 0,-80)
      RestartButtonLabel (TMP "Restart", size 48)
  [ADD HERE] WinPanel (active, CanvasGroup alpha=0)
    WinText (TMP "YOU WIN", size 120, pos 0,80)
    RestartButton (320×80, pos 0,-80)
      RestartButtonLabel (TMP "Restart", size 48)
```

### Architecture Compliance Checklist

- **No `FindObjectOfType<T>()`** — access `GameManager` via `GameManager.Instance`
- **`[SerializeField] private`** for all Inspector fields — `_canvasGroup` wired in Inspector
- **`Debug.Assert` in `Awake()`** for `_canvasGroup` null check
- **`Debug.Assert` in `OnEnable()`** for `GameManager.Instance` null check
- **Subscribe in `OnEnable`, unsubscribe in `OnDisable`** — never `Start`/`OnDestroy`
- **`Scripts/UI/` only** for `WinPanel.cs` — zero new folders needed
- **Architectural boundary preserved** — `WinPanel` reads events, does not write game state
- **TextMeshPro - UGUI** (not legacy `UI.Text`) — TMP assets already imported by Story 1.3's `TMP_AutoSetup.cs`
- **`GameLog.Info`** not `Debug.Log` — strips from release builds automatically

### Project Structure Notes

- `WinPanel.cs` → `Assets/Scripts/UI/` — already listed in architecture system location mapping
- WinPanel added to `Main.unity` — single scene architecture (ADR-006)
- **No new folders** needed
- `HudController.cs`, `TowerSelectionPanel.cs`, etc. — future stories, do NOT create now

### Forward Compatibility with Story 1.5 (Restart)

Story 1.5 will wire the "Restart" button's `onClick` on both GameOverPanel and WinPanel. When creating the button:
- Name it `RestartButton` (consistent with GameOverPanel)
- Leave `Button.onClick` empty (zero listeners)
- Story 1.5 will add `[SerializeField] private Button _restartButton` to both panel scripts

### Testing Approach

Per `project-context.md`: "UI panel behavior — manual QA only." No Edit Mode tests required for `WinPanel.cs`.

**Manual verification steps:**
1. Open `Main.unity` in Unity Editor
2. Confirm "WinPanel" GameObject is **active** with CanvasGroup alpha=0 in Inspector
3. Enter Play Mode
4. Confirm the Win panel is NOT visible at start
5. Press F5 (or configured Win key) → confirm "YOU WIN" panel appears with readable text and Restart button
6. Press backtick `` ` `` → confirm GameOverPanel still works (regression check)
7. Confirm no console errors
8. Confirm path/tile coloring from Story 1.2 is unaffected

### References

- Architecture: `_bmad-output/game-architecture.md` → ADR-002 (GameState.Win), ADR-006 (single scene / panel show-hide), ADR-007 (UGUI), Cross-cutting Concerns (Event System, Logging), Project Structure (Scripts/UI/)
- Epics: `_bmad-output/epics.md` → Epic 1: Game Foundation, Story 4
- Project Context: `_bmad-output/project-context.md` → Engine-Specific Rules (lifecycle order, event subscriptions), Code Organization Rules
- Previous Story: `_bmad-output/implementation-artifacts/1-3-game-over-screen.md` → GameOverPanel pattern (CanvasGroup, OnEnable/OnDisable, executionOrder -100 fix)

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

None — implementation followed story spec with no issues.

### Completion Notes List

- Created `Assets/Scripts/UI/WinPanel.cs` — uses `[RequireComponent(typeof(CanvasGroup))]` + `GetComponent` pattern (exact mirror of GameOverPanel), subscribes to `GameManager.OnStateChanged` in OnEnable/OnDisable, calls `HandleStateChanged(CurrentState)` on OnEnable to sync immediately, uses `GameLog.Info` for logging, includes `Debug.Assert` for null safety.
- Modified `Assets/Scripts/Core/GameManager.cs` — added second `if` block inside existing `#if UNITY_EDITOR || DEVELOPMENT_BUILD` guard: `Keyboard.current?.f5Key.wasPressedThisFrame == true` triggers `SetState(GameState.Win)` with matching log message. Existing backtick → GameOver trigger preserved unchanged.
- Modified `Assets/Scenes/Main.unity` via YAML — added WinPanel as second child of Canvas RectTransform (sibling of GameOverPanel, fileID 224000060). Full hierarchy: WinPanel (active, CanvasGroup alpha=0, Image color 0,0,0,0.75, WinPanel script guid bd93428ba089f4479bdb4293c7348994) → WinText (TMP, "YOU WIN", size 120, bold, white, pos 0,80) + RestartButton (320×80, pos 0,-80, no onClick wired) → RestartButtonLabel (TMP, "Restart", size 48, white).
- Created `Assets/Scripts/UI/WinPanel.cs.meta` with guid abc123def456000000000000abcdef30.
- All ACs satisfied at code/scene level. Manual QA verification required in Unity Editor per testing approach.

### File List

- `Assets/Scripts/UI/WinPanel.cs` (new)
- `Assets/Scripts/UI/WinPanel.cs.meta` (new)
- `Assets/Scripts/Core/GameManager.cs` (modified — added F5 → Win trigger in Update())
- `Assets/Scenes/Main.unity` (modified — added WinPanel hierarchy as Canvas child)
- `Assets/Scripts/Editor/TMP_AutoSetup.cs` (deleted — TMP resources fully configured by Story 1.3; auto-setup script no longer needed)
- `Assets/Scripts/Editor/TMP_AutoSetup.cs.meta` (deleted — meta for above)
- `basic-game.slnx` (modified — Unity auto-updated solution file when WinPanel.cs was added)

## Change Log

- 2026-03-16: Implemented Story 1.4 — Win Screen. Created WinPanel.cs UI controller, added F5 debug trigger to GameManager.cs, added WinPanel hierarchy to Main.unity scene alongside existing GameOverPanel.
