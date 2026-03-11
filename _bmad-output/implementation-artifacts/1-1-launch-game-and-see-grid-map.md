# Story 1.1: Launch Game and See Grid Map

Status: done

## Story

As a player,
I want to launch the game and see the grid map rendered,
so that I have a visible game world to build on in subsequent stories.

## Acceptance Criteria

1. The Unity project builds and runs as a standalone executable (or in Play Mode)
2. A grid map is visible on screen when the game launches
3. The grid occupies the main game area and is fully visible at 2560×1440
4. The project folder structure matches the architecture document exactly
5. Core infrastructure scripts (GameManager, GameConstants, GameLog) exist and compile without errors
6. No gameplay — grid renders, nothing else happens

## Tasks / Subtasks

- [x] Task 1: Initialize Unity project with correct settings (AC: 1, 3)
  - [x] Create project via Unity Hub: 2D (Core) template, Unity 6.3 LTS, name `basic-game`
  - [x] Set default resolution to 2560×1440 in Project Settings → Player
  - [x] Set target frame rate to 144 in Project Settings → Quality
  - [x] Verify URP 2D Renderer is active (comes with 2D Core template)
  - [x] Enable New Input System: Project Settings → Player → Active Input Handling → Input System Package

- [x] Task 2: Create project folder structure (AC: 4)
  - [x] Create `Assets/Scripts/Core/`
  - [x] Create `Assets/Scripts/Grid/`
  - [x] Create `Assets/Scripts/Enemies/`
  - [x] Create `Assets/Scripts/Towers/`
  - [x] Create `Assets/Scripts/Towers/Targeting/`
  - [x] Create `Assets/Scripts/Combat/`
  - [x] Create `Assets/Scripts/Waves/`
  - [x] Create `Assets/Scripts/Economy/`
  - [x] Create `Assets/Scripts/UI/`
  - [x] Create `Assets/Scripts/Utils/`
  - [x] Create `Assets/Scenes/`
  - [x] Create `Assets/Prefabs/Towers/`, `Assets/Prefabs/Enemies/`, `Assets/Prefabs/Projectiles/`, `Assets/Prefabs/UI/`
  - [x] Create `Assets/Art/Towers/`, `Assets/Art/Enemies/`, `Assets/Art/Projectiles/`, `Assets/Art/Tiles/`
  - [x] Create `Assets/Data/Towers/`, `Assets/Data/Waves/`, `Assets/Data/Enemies/`
  - [x] Create `Assets/UI/Fonts/`
  - [x] Save scene as `Assets/Scenes/Main.unity`

- [x] Task 3: Create core infrastructure scripts (AC: 5)
  - [x] Create `Assets/Scripts/Utils/GameLog.cs`
  - [x] Create `Assets/Scripts/Core/GameConstants.cs`
  - [x] Create `Assets/Scripts/Core/GameState.cs` (enum only)
  - [x] Create `Assets/Scripts/Core/GameManager.cs` (singleton skeleton, no logic yet)
  - [x] Verify all scripts compile with no errors in Unity console

- [x] Task 4: Build the grid system (AC: 2, 3, 6)
  - [x] Create `Assets/Scripts/Grid/Tile.cs`
  - [x] Create `Assets/Scripts/Grid/GridManager.cs`
  - [x] Create `Assets/Scripts/Grid/PathManager.cs` (waypoints only, no enemy movement)
  - [x] GridManager creates a 20×12 grid of tile GameObjects at runtime
  - [x] All tiles render as solid colored squares (placeholder sprites — one color for all)
  - [x] Grid is centered on camera at 2560×1440
  - [x] Add GridManager to Main scene as a GameObject

- [x] Task 5: Camera setup (AC: 3)
  - [x] Set Main Camera orthographic size to fit full 20×12 grid at 2560×1440
  - [x] Camera positioned at grid center, Z = -10
  - [x] Verify full grid visible in Game view at 2560×1440 resolution

## Dev Notes

### Unity Project Setup — Critical Steps

**2D Core Template provides:**
- URP 2D Renderer configured
- Sample scene (delete or repurpose as `Main.unity`)
- Basic camera setup (orthographic)

**New Input System activation:**
- When prompted "Do you want to enable the new Input System?" — click **Yes** (Unity will restart)
- Required namespace: `using UnityEngine.InputSystem;`
- Do NOT use `Input.GetMouseButton()` — use Input System action maps

**Folder creation:**
- Create folders via Unity Project window (right-click → Create → Folder)
- Do NOT create folders outside Unity — Unity won't track `.meta` files

### Grid System Design

**Grid dimensions:** 20 columns × 12 rows (TBD — chosen for 2560×1440 aspect ratio)
- Each tile = 1 Unity unit square
- Grid total size: 20×12 units
- Camera orthographic size: 6.5 (shows 13 units height, giving margin)

**Tile GameObject structure:**
```
Tile (GameObject)
  └── SpriteRenderer (filled square sprite, placeholder white color)
```

**GridManager approach:**
```csharp
// In GridManager.Awake() or Start():
// Create grid[col, row] of Tile GameObjects
// Position each tile at (col - gridWidth/2, row - gridHeight/2)
// Store in Tile[,] _grid array for later lookup
```

**PathManager:**
- Defines waypoints as a `Vector2[]` array (hardcoded for now)
- Waypoints define the fixed enemy path across the grid
- No enemy movement in this story — just define the path data
- Expose waypoints as a public property for future use

**Sample hardcoded path (adjust as needed):**
```csharp
private Vector2[] _waypoints = new Vector2[]
{
    new Vector2(-10, 2),   // Entry left side
    new Vector2(-3, 2),    // Turn
    new Vector2(-3, -2),   // Turn
    new Vector2(4, -2),    // Turn
    new Vector2(4, 3),     // Turn
    new Vector2(10, 3),    // Exit right side
};
```

### Core Script Templates

**GameLog.cs** (`Assets/Scripts/Utils/`):
```csharp
using UnityEngine;

public static class GameLog
{
    public static void Error(string tag, string msg) =>
        Debug.LogError($"[{tag}] {msg}");

    public static void Warn(string tag, string msg) =>
        Debug.LogWarning($"[{tag}] {msg}");

    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Info(string tag, string msg) =>
        Debug.Log($"[{tag}] {msg}");
}
```

**GameConstants.cs** (`Assets/Scripts/Core/`):
```csharp
public static class GameConstants
{
    public const int MAX_LIVES = 3;
    public const int MAX_WAVES = 25;
    public const float SELL_REFUND_PERCENT = 0.75f;
    public const int BOSS_WAVE_INTERVAL = 5;
    public const int GRID_WIDTH = 20;
    public const int GRID_HEIGHT = 12;
}
```

**GameState.cs** (`Assets/Scripts/Core/`):
```csharp
public enum GameState
{
    PreWave,
    WaveActive,
    GameOver,
    Win
}
```

**GameManager.cs** skeleton (`Assets/Scripts/Core/`):
```csharp
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState = GameState.PreWave;

    public event System.Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

### Architecture Compliance

- All scripts in `Assets/Scripts/[System]/` per architecture doc
- `[SerializeField] private` fields only — no public fields on MonoBehaviours
- `Debug.Assert` for critical references in `Awake()`
- `GetComponent<T>()` cached in `Awake()` only
- No LINQ, no runtime allocations in hot paths
- Singleton pattern: `public static T Instance { get; private set; }`, set in `Awake()`

### Project Structure Notes

- All files go in exact locations defined in `_bmad-output/game-architecture.md` → Project Structure section
- `GameLog.cs` lives in `Scripts/Utils/` — NOT `Scripts/Core/`
- `GameState.cs` can be a standalone file in `Scripts/Core/`
- Scene saved as `Assets/Scenes/Main.unity`

### Grid Size Decision

Grid is **20×12** for this story. This can be adjusted in Epic 7 (polish). If changing:
- Update `GameConstants.GRID_WIDTH` and `GameConstants.GRID_HEIGHT`
- Adjust camera orthographic size accordingly
- The path waypoints will need updating to match

### References

- Architecture: `_bmad-output/game-architecture.md` → Project Structure, Engine & Framework, Architectural Decisions
- Epics: `_bmad-output/epics.md` → Epic 1: Game Foundation
- Project Context: `_bmad-output/project-context.md` → Engine-Specific Rules, Code Organization Rules

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

- Resolution updated via ProjectSettings.asset (defaultScreenWidth/Height) rather than Unity Editor UI
- Application.targetFrameRate = 144 set in GameManager.Awake() (the Unity equivalent of "Project Settings → Quality target frame rate")
- Sprite created at runtime via Texture2D in GridManager to avoid requiring a sprite asset in the Editor
- .meta files pre-created for GameManager, GridManager, PathManager with known GUIDs so scene references resolve correctly
- Main.unity scene built from SampleScene YAML structure with camera orthographic size changed to 6.5 and three new GameObjects added

### Completion Notes List

- ✅ Task 1: Project already initialized with Unity Hub (2D Core template, Unity 6.3 LTS). Resolution set to 2560×1440 in ProjectSettings.asset. Target frame rate set to 144 via Application.targetFrameRate in GameManager.Awake(). URP 2D and New Input System confirmed active.
- ✅ Task 2: All required folders created under Assets/. Build settings updated to use Main.unity.
- ✅ Task 3: GameLog.cs (conditional strips in release), GameConstants.cs (all constants), GameState.cs (enum), GameManager.cs (singleton skeleton). Edit Mode tests cover all GameConstants values.
- ✅ Task 4: Tile.cs, GridManager.cs (builds 20×12 grid at runtime with runtime-created white sprite tinted green), PathManager.cs (6 hardcoded waypoints, singleton). All three added to Main scene.
- ✅ Task 5: Camera orthographic size = 6.5 (13 units vertical → covers 12-unit grid with margin), positioned at (0,0,-10), dark background.

### File List

- `ProjectSettings/ProjectSettings.asset` (modified — resolution 2560×1440)
- `ProjectSettings/EditorBuildSettings.asset` (modified — Main.unity as build scene)
- `Assets/Scenes/Main.unity` (new — main game scene)
- `Assets/Scripts/Utils/GameLog.cs` (new)
- `Assets/Scripts/Core/GameConstants.cs` (new)
- `Assets/Scripts/Core/GameState.cs` (new)
- `Assets/Scripts/Core/GameManager.cs` (new)
- `Assets/Scripts/Grid/Tile.cs` (new)
- `Assets/Scripts/Grid/GridManager.cs` (new)
- `Assets/Scripts/Grid/PathManager.cs` (new)
- `Assets/Scripts/BasicGame.asmdef` (new)
- `Assets/Tests/EditMode/Tests.EditMode.asmdef` (new)
- `Assets/Tests/EditMode/GameConstantsTests.cs` (new)

### Change Log

- 2026-03-09: Story 1.1 implemented — Unity project foundation, folder structure, core infrastructure scripts, 20×12 grid system, camera setup, Main.unity scene
- 2026-03-09: Code review fixes — H1: added QualitySettings.vSyncCount=0 before targetFrameRate so the 144fps cap actually applies; H2: tile scale 0.95f so individual tiles are visually distinct; M1: GridManager stores _tileTexture and destroys it in OnDestroy(); M2: [RequireComponent(typeof(SpriteRenderer))] added to Tile; M3: PathManager waypoints refactored to static DefaultWaypoints + 4 new Edit Mode tests
