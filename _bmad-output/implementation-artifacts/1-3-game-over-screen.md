# Story 1.3: Game Over Screen

Status: ready-for-dev

<!-- Note: Validation is optional. Run validate-create-story for quality check before dev-story. -->

## Story

As a player,
I want to see a game over screen when the game over condition is triggered,
so that I have a clear visual signal that the game has ended and I need to take action.

## Acceptance Criteria

1. A "GAME OVER" panel becomes visible when `GameManager.SetState(GameState.GameOver)` is called
2. The panel is invisible (inactive in the hierarchy) at game start and whenever the state is not `GameOver`
3. The panel displays "GAME OVER" text clearly readable at 2560×1440
4. `GameOverPanel.cs` subscribes to `GameManager.Instance.OnStateChanged` in `OnEnable()` and unsubscribes in `OnDisable()`
5. The panel contains a placeholder "Restart" button (visible, not wired to any logic — functionality is added in Story 1.5)
6. No gameplay systems are modified — this is a UI overlay only
7. A `#if UNITY_EDITOR || DEVELOPMENT_BUILD` gated keyboard shortcut (F4) in `GameManager.Update()` calls `SetState(GameState.GameOver)` to enable manual in-editor testing

## Tasks / Subtasks

- [ ] Task 1: Create Canvas and GameOverPanel hierarchy in `Main.unity` (AC: 1, 2, 3, 5)
  - [ ] Add a Canvas GameObject: Render Mode = Screen Space - Overlay, Sort Order = 10
  - [ ] Add Canvas Scaler component: UI Scale Mode = Scale With Screen Size, Reference Resolution = 2560×1440, Match = 0.5
  - [ ] Add GraphicRaycaster component to Canvas (required for button interaction)
  - [ ] Create "GameOverPanel" as a child of Canvas: full-screen RectTransform (anchors stretch/stretch, offsets all 0)
  - [ ] Add Image component to panel: Color = (0, 0, 0, 0.75) — semi-transparent dark overlay
  - [ ] Add "GameOverText" child: TextMeshPro - UGUI, text = "GAME OVER", font size 120, alignment center, white, bold
  - [ ] Position GameOverText at (0, 80, 0) relative to panel center
  - [ ] Add "RestartButton" child: Unity UI Button, label text = "Restart", font size 48, white text
  - [ ] Position RestartButton at (0, -80, 0) relative to panel center, size 320×80
  - [ ] Set GameOverPanel GameObject to **inactive** (unchecked in Inspector) — it starts hidden
  - [ ] **Do NOT wire the Restart button's onClick** — that is Story 1.5

- [ ] Task 2: Create `Assets/Scripts/UI/GameOverPanel.cs` (AC: 1, 2, 4, 6)
  - [ ] Class `GameOverPanel : MonoBehaviour`
  - [ ] In `OnEnable()`: subscribe `GameManager.Instance.OnStateChanged += HandleStateChanged`
  - [ ] In `OnDisable()`: unsubscribe `GameManager.Instance.OnStateChanged -= HandleStateChanged`
  - [ ] `HandleStateChanged(GameState newState)`:
    - [ ] `bool show = newState == GameState.GameOver`
    - [ ] `gameObject.SetActive(show)`
    - [ ] Log: `GameLog.Info("GameOverPanel", show ? "Showing" : "Hiding")`
  - [ ] Add `Debug.Assert(GameManager.Instance != null, "[GameOverPanel] GameManager.Instance is null")` at the top of `OnEnable()`
  - [ ] Add `private void Awake()` guard: verify the GameObject starts inactive (no logic needed, just Awake exists for future safety)
  - [ ] **No [SerializeField] references needed** — this script controls its own GameObject's active state

- [ ] Task 3: Add debug trigger to `GameManager.cs` (AC: 7)
  - [ ] Add `using UnityEngine.InputSystem;` at top of `GameManager.cs` (New Input System)
  - [ ] Add `private void Update()` method to `GameManager.cs`:
    ```csharp
    private void Update()
    {
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Keyboard.current.f4Key.wasPressedThisFrame)
            SetState(GameState.GameOver);
    #endif
    }
    ```
  - [ ] Note: F4 is "force game over" — temporary dev trigger. **Do not use F1/F2/F3** — those are reserved for the debug overlay (F1), add currency (F2, Epic 5), and skip wave (F3, Epic 6)

- [ ] Task 4: Wire GameOverPanel in scene and verify (AC: 1, 2)
  - [ ] Add `GameOverPanel` component to the "GameOverPanel" GameObject in Main.unity
  - [ ] Verify the panel GameObject is inactive in the Inspector before pressing Play
  - [ ] Press Play → confirm panel is not visible at start
  - [ ] Press F4 → confirm "GAME OVER" panel slides into view
  - [ ] Verify text is readable and layout is clean at 2560×1440 Game view resolution

## Dev Notes

### Context from Stories 1.1 and 1.2

The following files already exist and **must not be recreated**:

| File | Location | Status |
|---|---|---|
| `GameManager.cs` | `Assets/Scripts/Core/` | Has `SetState()`, `OnStateChanged` event, `CurrentState` — **only add `Update()`** |
| `GameState.cs` | `Assets/Scripts/Core/` | Enum: `PreWave`, `WaveActive`, `GameOver`, `Win` — already exists, do not modify |
| `GameConstants.cs` | `Assets/Scripts/Core/` | No changes needed for this story |
| `GameLog.cs` | `Assets/Scripts/Utils/` | Use for all logging |
| `Main.unity` | `Assets/Scenes/` | Add Canvas + GameOverPanel to existing scene |
| `Tile.cs` / `GridManager.cs` / `PathManager.cs` | `Assets/Scripts/Grid/` | No changes needed |

Story 1.1 review fixes added these to `GameManager.cs`:
- `Application.targetFrameRate = 144` and `QualitySettings.vSyncCount = 0` in `Awake()`
- These must be preserved when adding `Update()`

### GameManager.cs — Current Shape

Before adding Update(), GameManager looks like this (from Story 1.1):

```csharp
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState = GameState.PreWave;

    public event System.Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
    }

    public void SetState(GameState newState)
    {
        _currentState = newState;
        OnStateChanged?.Invoke(newState);
        GameLog.Info("GameManager", $"State changed to {newState}");
    }

    public GameState CurrentState => _currentState;
}
```

**Only add the `Update()` method — do not modify anything else.**

### GameOverPanel.cs Pattern

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverPanel : MonoBehaviour
{
    private void Awake()
    {
        // Panel starts inactive — Awake only runs once when first activated.
        // This method exists as a safe hook for future initialization.
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[GameOverPanel] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        bool show = newState == GameState.GameOver;
        gameObject.SetActive(show);
        GameLog.Info("GameOverPanel", show ? "Showing game over panel" : "Hiding game over panel");
    }
}
```

**Critical pitfall:** `OnEnable()` fires when the GameObject is first set active. `GameManager.Instance` must exist before the panel's `OnEnable()` runs. Since `GameManager.Awake()` sets `Instance = this` and the panel starts inactive (never calls OnEnable during scene startup), this is safe. The panel's OnEnable only fires when explicitly set active — which only happens after F4 press, well after GameManager.Awake().

**Do NOT** put subscription logic in `Start()` — the panel starts inactive, so `Start()` never runs until the object is first activated. `OnEnable`/`OnDisable` is the correct lifecycle pair per architecture rules.

### Canvas Setup — Critical Details

**Why UGUI Canvas, not World Space:**
Architecture ADR-006: game over is a UI panel — not a scene reload, not a world-space element. Canvas Render Mode = **Screen Space - Overlay** ensures it renders on top of everything regardless of camera position.

**TextMeshPro requirement:**
Use `TextMeshPro - UGUI` (TMP_Text), NOT the legacy `UI.Text`. Unity 6 ships with TextMeshPro included. Right-click in hierarchy → UI → Text - TextMeshPro.

**Canvas Scaler — why 2560×1440 reference:**
Scale With Screen Size + 2560×1440 reference means all layout coordinates are authored at the target resolution. UI scales correctly at any window size. Match = 0.5 blends width/height scaling for balanced behavior.

**Panel starts INACTIVE:**
The GameOverPanel GameObject must be **unchecked** (inactive) in the Inspector before entering Play Mode. If it's active at start, `OnEnable()` fires during scene initialization, which could race with GameManager singleton setup.

### Input System — New Input System vs Legacy

This project uses the **New Input System** (`UnityEngine.InputSystem`). Using `Input.GetKeyDown(KeyCode.F4)` will produce warnings or fail if legacy input is disabled. Use:

```csharp
// CORRECT — New Input System
using UnityEngine.InputSystem;

if (Keyboard.current.f4Key.wasPressedThisFrame)
    SetState(GameState.GameOver);

// WRONG — Legacy input (disabled in this project)
// if (Input.GetKeyDown(KeyCode.F4)) ...
```

`Keyboard.current` can be null if no keyboard is connected (rare on PC). Add a null check if desired:
```csharp
if (Keyboard.current?.f4Key.wasPressedThisFrame == true)
    SetState(GameState.GameOver);
```

### Architecture Compliance Checklist

- **No `FindObjectOfType<T>()`** — access `GameManager` via `GameManager.Instance`
- **`[SerializeField] private`** for all Inspector fields — GameOverPanel has no SerializeField fields in this story
- **Subscribe in `OnEnable`, unsubscribe in `OnDisable`** — never `Start`/`OnDestroy` for events
- **`Scripts/UI/` only** — `GameOverPanel.cs` lives there; zero additions to `Scripts/Core/` (GameManager gets Update() only)
- **Architectural boundary preserved** — `GameOverPanel` reads GameManager events but does not write game state
- **`Debug.Assert` in `OnEnable()`** for the GameManager.Instance null check

### Project Structure Notes

- `GameOverPanel.cs` → `Assets/Scripts/UI/` — matches architecture system location mapping for all UI panel controllers
- Canvas and GameOverPanel are added to `Main.unity` — single scene architecture (ADR-006)
- **No new folders** needed for this story
- `WinPanel.cs` is listed in architecture but belongs to Story 1.4 — do NOT create it now
- `HudController.cs`, `TowerSelectionPanel.cs`, etc. — future stories, do NOT create now

### Forward Compatibility with Story 1.5 (Restart)

Story 1.5 will wire the "Restart" button's `onClick` handler. When creating the button:
- Name it clearly: `RestartButton`
- Leave `Button.onClick` empty (zero listeners)
- Story 1.5 will add a `[SerializeField] private Button _restartButton` to `GameOverPanel.cs` and wire it

Do NOT add a `Button` reference or click handler in this story.

### Testing Approach

Per `project-context.md`: "UI panel behavior — manual QA only." No Edit Mode tests are required or expected for `GameOverPanel.cs`.

**Manual verification steps:**
1. Open `Main.unity` in Unity Editor
2. In the Inspector, confirm "GameOverPanel" GameObject is inactive (checkbox unchecked)
3. Enter Play Mode
4. Confirm the panel is NOT visible at start
5. Press F4 — confirm "GAME OVER" panel appears with readable text and placeholder button
6. Confirm no console errors
7. Confirm path/tile coloring from Story 1.2 is unaffected

### References

- Architecture: `_bmad-output/game-architecture.md` → ADR-002 (GameState enum), ADR-006 (single scene / panel show-hide), ADR-007 (UGUI), Cross-cutting Concerns (Event System, Logging), Project Structure (Scripts/UI/)
- Epics: `_bmad-output/epics.md` → Epic 1: Game Foundation, Story 3
- Project Context: `_bmad-output/project-context.md` → Engine-Specific Rules (lifecycle order, event subscriptions), Code Organization Rules
- Previous Story: `_bmad-output/implementation-artifacts/1-1-launch-game-and-see-grid-map.md` → GameManager skeleton, GameState enum template

## Dev Agent Record

### Agent Model Used

_TBD_

### Debug Log References

### Completion Notes List

### File List
