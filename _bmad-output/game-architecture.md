---
title: 'Game Architecture'
project: 'basic-game'
date: '2026-03-05'
author: 'Nate'
version: '1.0'
stepsCompleted: [1, 2, 3, 4, 5, 6, 7, 8, 9]
status: 'complete'
engine: 'Unity 6.3 LTS'
platform: 'PC Desktop'

# Source Documents
gdd: '_bmad-output/gdd.md'
epics: '_bmad-output/epics.md'
brief: '_bmad-output/game-brief.md'
---

# Game Architecture

## Executive Summary

**Basic Tower Defense** is a single-player PC desktop tower defense game built on **Unity 6.3 LTS** with URP 2D Renderer and C# scripting. The architecture prioritizes simplicity and learning value: Manager Singleton pattern organizes 9 core systems, a single Unity scene hosts all gameplay, and ScriptableObjects drive all wave and tower balance data — making tuning fast without recompiling.

**Key architectural decisions:** (1) Manager Singletons (`GameManager`, `WaveManager`, `TowerManager`, `EnemyManager`, `EconomyManager`) communicate via typed C# events; (2) an `ITargetingStrategy` interface with 5 implementations enables per-tower configurable targeting priority; (3) Unity's built-in `ObjectPool<T>` wraps projectile and enemy spawning to meet the 144fps performance target.

**Project structure** follows a hybrid organization — type folders at the `Assets/` root, feature subfolders within `Scripts/` mapped directly to all 9 systems. All 8 architectural decisions are documented with ADRs, all cross-cutting concerns have concrete C# examples, and all 7 epics are mapped to specific file locations. The document is ready for AI agent implementation.

---

## Project Context

### Game Overview

**Basic Tower Defense** — A clean, no-frills tower defense game. Players place specialized
towers on a fixed-path grid map to intercept 25 escalating waves of enemies. Towers persist
across all waves. Boss waves every 5th level. Full game over on 3 lives lost — restart from
wave 1.

### Technical Scope

**Platform:** PC Desktop — standalone executable, offline, mouse-only input
**Genre:** Tower Defense
**Project Level:** Low–Medium complexity. Well-understood genre, no networking, no physics
engine required, no audio pipeline.

### Core Systems

| System | Complexity | Notes |
|---|---|---|
| Grid / Map | Low | Static grid, fixed path, 2 tile types |
| Enemy Path Movement | Medium | Smooth movement along fixed waypoints |
| Tower Placement | Low–Medium | Grid interaction + range visualization |
| Combat (Targeting + Projectiles) | Medium–High | Configurable priority, 4 tower types, AOE area hit |
| Wave Management | Medium | 25 defined waves, scaling, boss logic, manual launch |
| Economy | Low | Currency arithmetic — earn, spend, 75% sell refund |
| Tower Upgrades | Low | Stat multipliers across 3 tiers |
| Game State | Low | Lives, game over, win, restart state machine |
| UI | Low–Medium | Event-driven panels, tower selection, upgrade/sell |

### Technical Requirements

- **Performance:** 60fps minimum, 144fps target @ 2560x1440
- **Networking:** None — fully offline
- **Input:** Mouse only
- **Audio:** None (deferred from v1)
- **Distribution:** Standalone executable — no launcher, no storefront
- **Assets:** AI-generated PNG sprites — small set (~20 assets total)

### Complexity Drivers

- **Configurable targeting priority per tower** — needs per-tower state + targeting strategy pattern
- **AOE area hit detection** — spatial query against enemies in explosion radius
- **Tower persistence across waves** — towers are session-long entities, must survive wave transitions cleanly

### Technical Risks

1. **Engine selection** — Biggest architectural decision; wrong choice could require significant rework
2. **144fps target** — With many projectiles and enemies on screen; projectile pooling likely needed
3. **AOE hit detection** — Needs efficient spatial query; brute-force acceptable for small enemy counts, may need optimization at higher wave counts

---

## Engine & Framework

### Selected Engine

**Unity 6.3 LTS** — Industry-standard game engine with first-class 2D support,
C# scripting, and broad documentation. Selected for learning value and industry
relevance over simpler alternatives.

**Rationale:** C# familiarity goal, massive documentation ecosystem, standalone
PC export built-in, excellent 2D sprite and scene tooling.

### Project Initialization

```bash
# In Unity Hub:
# New Project → 2D (Core) template → Unity 6.3 LTS
# Project name: basic-game
```

### Engine-Provided Architecture

| Component | Solution | Notes |
|---|---|---|
| Rendering | URP 2D Renderer | Sprites, 2D camera, no 3D overhead |
| Physics | Unity 2D Physics | Available but not used — no physics needed |
| Input | New Input System | Mouse event-driven input |
| Audio | Unity Audio System | Deferred — no audio in v1 |
| Scene Management | Unity SceneManager | Single scene expected |
| Build System | Unity Build + Mono/IL2CPP | Standalone PC executable export |
| Asset Pipeline | Unity Asset Database | Sprite import, texture atlasing |

### Remaining Architectural Decisions

The following must be explicitly decided in subsequent steps:

- Game architecture pattern (MonoBehaviour managers vs ScriptableObjects vs custom)
- Game state machine (lives, wave, game over, win)
- Tower targeting strategy pattern (configurable per-tower priority)
- Wave data representation (ScriptableObjects vs code vs JSON)
- Object pooling (projectiles, enemies)
- Scene structure (single scene vs loading)

---

## Architectural Decisions

### Decision Summary

| Category | Decision | Rationale |
|---|---|---|
| Game Architecture Pattern | Manager Singletons | Simple, well-documented Unity pattern; maps cleanly to 9 core systems; ideal for learning |
| Game State Machine | Enum + GameManager | `GameState` enum owned by `GameManager`; state change events decouple dependent systems |
| Tower Targeting Strategy | Strategy pattern (`ITargetingStrategy`) | 5 small implementations (First/Last/Nearest/Strongest/Weakest); swappable per-tower at runtime |
| Wave Data Representation | ScriptableObjects (`WaveDefinitionSO`) | Inspector-editable; no parsing code; balance tweaks without recompiling |
| Object Pooling | Unity `ObjectPool<T>` | Built-in, modern, addresses 144fps GC risk; wraps projectile and enemy pools |
| Scene Structure | Single scene | All objects persist naturally; game over/win are UI panels; simplifies tower persistence |
| UI Framework | UGUI (Canvas) | Mature ecosystem, maximum tutorial coverage, sufficient for this scope |
| Asset Loading | Direct Inspector references | ~20 sprites total; `[SerializeField]` wiring is simplest correct solution |

### Architecture Decision Records

**ADR-001: Manager Singleton Pattern**
GameManager, WaveManager, TowerManager, EnemyManager, and EconomyManager implemented as MonoBehaviour singletons. Each manager owns its domain. Inter-manager communication via C# events to avoid tight coupling.

**ADR-002: Enum-Based Game State Machine**
`GameState` enum: `PreWave | WaveActive | GameOver | Win`. `GameManager` owns current state and fires `OnGameStateChanged` event. All systems subscribe and react independently — no direct polling.

**ADR-003: ITargetingStrategy Pattern**
Interface: `ITargetingStrategy { Enemy SelectTarget(List<Enemy> enemiesInRange); }`. Five implementations: `FirstEnemyStrategy`, `LastEnemyStrategy`, `NearestEnemyStrategy`, `StrongestEnemyStrategy`, `WeakestEnemyStrategy`. Each `Tower` holds an active strategy reference, hot-swapped via UI.

**ADR-004: WaveDefinitionSO**
One ScriptableObject per wave or a single `WaveDatabaseSO` holding all 25. Fields: enemy count, base health multiplier, base speed multiplier, `isBossWave` flag. Referenced directly by `WaveManager` via Inspector.

**ADR-005: Unity ObjectPool<T> for Projectiles and Enemies**
`ProjectilePool` and `EnemyPool` MonoBehaviours wrap `ObjectPool<T>`. Pools pre-warm on scene load. Towers request from pool on fire; projectiles and enemies return to pool on death or path completion.

**ADR-006: Single Scene Architecture**
All gameplay in one scene. Game Over and Win states show/hide Canvas panels. No `SceneManager.LoadScene` calls needed. Tower persistence across waves is implicit — nothing unloads.

**ADR-007: UGUI for all UI**
Canvas-based UI for HUD (lives, currency, wave counter), tower selection panel, upgrade/sell panel, game over panel, win panel. Event-driven — panels subscribe to `GameManager` state events.

**ADR-008: Direct Asset References**
All sprites and ScriptableObjects assigned via `[SerializeField]` in the Inspector. No runtime loading code. With ~20 total assets, a loading strategy adds complexity with no benefit.

---

---

## Cross-cutting Concerns

These patterns apply to ALL systems and must be followed by every implementation.

### Error Handling

**Strategy:** Try-catch at data/IO boundaries only. `Debug.Assert` for preconditions in development. `Debug.LogError` for runtime failures — log and recover gracefully, never crash the game.

**Rules:**
- Try-catch only at system entry points (ScriptableObject loading, file I/O)
- Internal game logic does not use try-catch
- `Debug.Assert(condition, message)` for preconditions (stripped in release)
- `Debug.LogError` for unexpected runtime states — always include system context
- Errors degrade gracefully; the game continues running where possible

**Example:**
```csharp
// Boundary: loading data
try {
    waveDatabase = Resources.Load<WaveDatabaseSO>("WaveDatabase");
} catch (Exception e) {
    Debug.LogError($"[WaveManager] Failed to load WaveDatabase: {e.Message}");
}

// Precondition: internal logic
Debug.Assert(waveIndex >= 0 && waveIndex < waves.Count, "[WaveManager] Wave index out of range");

// Runtime failure: graceful
if (target == null) {
    Debug.LogError("[Tower] Target lost unexpectedly — skipping fire cycle");
    return;
}
```

### Logging

**Format:** Tagged prefix per system — `[SystemName] Message`
**Destination:** Unity Console (editor + development builds); Info/Warn stripped from release

**Log Levels:**
- `GameLog.Error(tag, msg)` → `Debug.LogError` — always included
- `GameLog.Warn(tag, msg)` → `Debug.LogWarning` — included in dev builds
- `GameLog.Info(tag, msg)` → `Debug.Log` — stripped from release

**Example:**
```csharp
public static class GameLog {
    public static void Error(string tag, string msg) =>
        Debug.LogError($"[{tag}] {msg}");

    public static void Warn(string tag, string msg) =>
        Debug.LogWarning($"[{tag}] {msg}");

    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Info(string tag, string msg) =>
        Debug.Log($"[{tag}] {msg}");
}

// Usage
GameLog.Info("WaveManager", "Wave 5 started");
GameLog.Error("EnemyPool", "Pool exhausted — consider increasing pool size");
```

### Configuration

**Approach:** Two-tier — static constants for fixed values, ScriptableObjects for tunable values.

**`GameConstants.cs` (never changes):**
```csharp
public static class GameConstants {
    public const int MAX_LIVES = 3;
    public const int MAX_WAVES = 25;
    public const float SELL_REFUND_PERCENT = 0.75f;
    public const int BOSS_WAVE_INTERVAL = 5;
}
```

**ScriptableObjects (tunable in Inspector):**
- `TowerDefinitionSO` — cost, damage, range, fire rate per tower type and level
- `WaveDefinitionSO` / `WaveDatabaseSO` — enemy count, health/speed multipliers, boss flag
- `EnemyDefinitionSO` — base health, base speed

### Event System

**Pattern:** C# events declared on manager singletons. All events are typed delegates.

**Naming convention:** `On` + past-tense verb — e.g., `OnStateChanged`, `OnEnemyReachedEnd`, `OnCurrencyChanged`

**Example:**
```csharp
// Declaration on manager
public event Action<GameState> OnStateChanged;
public event Action<int> OnCurrencyChanged;
public event Action OnEnemyReachedEnd;

// Publishing
OnStateChanged?.Invoke(newState);

// Subscribing
void OnEnable() => GameManager.Instance.OnStateChanged += HandleStateChange;
void OnDisable() => GameManager.Instance.OnStateChanged -= HandleStateChange;
```

**Rules:**
- Always unsubscribe in `OnDisable` to prevent memory leaks
- Events are fired after state has already changed, not before
- No string-keyed events — all events are typed

### Debug Tools

**Available Tools:**
- **Unity Gizmos** — `OnDrawGizmos` on Tower (range circle), PathManager (waypoint visualization)
- **F1 overlay** — In-game UI panel: FPS, wave number, active enemy count, pool usage, current currency
- **Cheat keys** (development builds only): Add 1000 currency, skip to next wave, kill all enemies

**Activation:** Gizmos always on in Editor. F1 overlay available in `UNITY_EDITOR || DEVELOPMENT_BUILD` only. Cheat keys same conditional — completely absent in release builds.

```csharp
void Update() {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    if (Input.GetKeyDown(KeyCode.F1)) debugOverlay.SetActive(!debugOverlay.activeSelf);
    if (Input.GetKeyDown(KeyCode.F2)) EconomyManager.Instance.AddCurrency(1000);
    if (Input.GetKeyDown(KeyCode.F3)) WaveManager.Instance.SkipToNextWave();
#endif
}
```

---

---

## Project Structure

### Organization Pattern

**Pattern:** Hybrid — type folders at `Assets/` root, feature subfolders within `Scripts/`

**Rationale:** Matches Unity conventions and documentation examples; feature subfolders within Scripts/ map directly to the 9 core systems, making it clear where every file belongs.

### Directory Structure

```
Assets/
├── Scenes/
│   └── Main.unity
├── Scripts/
│   ├── Core/                        # Game state machine, constants, logging
│   │   ├── GameManager.cs
│   │   ├── GameState.cs             # enum: PreWave | WaveActive | GameOver | Win
│   │   ├── GameConstants.cs
│   │   └── GameLog.cs
│   ├── Grid/                        # Grid map, tile types, path waypoints
│   │   ├── GridManager.cs
│   │   ├── Tile.cs
│   │   └── PathManager.cs
│   ├── Enemies/                     # Enemy movement, health, pool
│   │   ├── Enemy.cs
│   │   ├── EnemyManager.cs
│   │   └── EnemyPool.cs
│   ├── Towers/                      # Placement, upgrades, targeting
│   │   ├── Tower.cs
│   │   ├── TowerManager.cs
│   │   ├── TowerPlacer.cs
│   │   └── Targeting/
│   │       ├── ITargetingStrategy.cs
│   │       ├── FirstEnemyStrategy.cs
│   │       ├── LastEnemyStrategy.cs
│   │       ├── NearestEnemyStrategy.cs
│   │       ├── StrongestEnemyStrategy.cs
│   │       └── WeakestEnemyStrategy.cs
│   ├── Combat/                      # Projectiles, AOE, damage
│   │   ├── Projectile.cs
│   │   ├── ProjectilePool.cs
│   │   └── AoeExplosion.cs
│   ├── Waves/                       # Wave sequencing, enemy spawning
│   │   └── WaveManager.cs
│   ├── Economy/                     # Currency, costs, sell refund
│   │   └── EconomyManager.cs
│   ├── UI/                          # All UGUI panel controllers
│   │   ├── HudController.cs
│   │   ├── TowerSelectionPanel.cs
│   │   ├── TowerInfoPanel.cs
│   │   ├── GameOverPanel.cs
│   │   ├── WinPanel.cs
│   │   └── DebugOverlay.cs
│   └── Utils/
│       └── GameLog.cs
├── Prefabs/
│   ├── Towers/
│   ├── Enemies/
│   ├── Projectiles/
│   └── UI/
├── Art/
│   ├── Towers/                      # 4 types × 3 levels = 12 sprites
│   ├── Enemies/                     # Enemy sprite, Boss sprite
│   ├── Projectiles/                 # 4 projectile sprites
│   └── Tiles/                       # Path tile, buildable tile sprites
├── Data/
│   ├── Towers/                      # TowerDefinitionSO assets
│   ├── Waves/                       # WaveDatabaseSO + WaveDefinitionSO assets
│   └── Enemies/                     # EnemyDefinitionSO assets
└── UI/
    └── Fonts/
```

### System Location Mapping

| System | Location | Responsibility |
|---|---|---|
| Game State Machine | `Scripts/Core/` | `GameManager`, `GameState` enum, `GameConstants` |
| Grid / Map | `Scripts/Grid/` | Tile types, grid queries, path waypoints |
| Enemy Path Movement | `Scripts/Enemies/` + `Scripts/Grid/PathManager.cs` | Spawning, movement, health, pool |
| Tower Placement | `Scripts/Towers/TowerPlacer.cs` | Grid validation, range preview, placement |
| Combat — Targeting | `Scripts/Towers/Targeting/` | `ITargetingStrategy` + 5 implementations |
| Combat — Projectiles | `Scripts/Combat/` | Projectile movement, AOE explosion, damage |
| Wave Management | `Scripts/Waves/` | Wave sequencing, spawn timing, boss logic |
| Economy & Upgrades | `Scripts/Economy/` | Currency earn/spend, upgrade costs, sell refund |
| UI | `Scripts/UI/` | All UGUI panel controllers |
| Object Pools | `Scripts/Enemies/EnemyPool.cs` + `Scripts/Combat/ProjectilePool.cs` | Unity `ObjectPool<T>` wrappers |

### Naming Conventions

#### Code Elements

| Element | Convention | Example |
|---|---|---|
| Classes | PascalCase | `EnemyManager`, `WaveDefinitionSO` |
| Methods | PascalCase | `SpawnEnemy()`, `GetNextWave()` |
| Private fields | `_camelCase` | `_currentWave`, `_livesRemaining` |
| Public properties | PascalCase | `CurrentWave`, `IsWaveActive` |
| Constants | UPPER_SNAKE | `MAX_LIVES`, `SELL_REFUND_PERCENT` |
| Interfaces | `I` prefix + PascalCase | `ITargetingStrategy` |
| ScriptableObjects | `SO` suffix | `TowerDefinitionSO`, `WaveDatabaseSO` |
| Enums | PascalCase | `GameState`, `TowerType`, `TargetPriority` |
| C# events | `On` + PascalCase | `OnStateChanged`, `OnEnemyReachedEnd` |

#### Assets

| Asset Type | Convention | Example |
|---|---|---|
| Prefabs | PascalCase matching script | `Enemy.prefab`, `Tower_Fast.prefab` |
| Sprites | `snake_case` | `tower_fast_lvl1.png`, `tile_path.png` |
| ScriptableObject assets | PascalCase | `WaveDatabase.asset`, `Tower_Fast_L1.asset` |
| Scenes | PascalCase | `Main.unity` |

### Architectural Boundaries

- `Core/` systems have no dependencies on feature folders — they are imported, not importing
- Feature folders (`Enemies/`, `Towers/`, etc.) may reference `Core/` and `Utils/` freely
- `UI/` subscribes to manager events only — no direct gameplay logic
- `Combat/` does not reference `Towers/` directly — damage flows through interfaces
- `Scripts/` never references `Art/` or `Data/` directly — assets are wired via Inspector

---

---

## Implementation Patterns

These patterns ensure consistent implementation across all AI agents.

### Novel Patterns

None required. AOE hit detection, target-death mid-flight handling, and wave spawn timing are handled ad hoc within their respective systems using standard Unity APIs.

### Communication Patterns

**Pattern:** C# events on manager singletons for cross-system communication; direct `GetComponent<T>()` for same-GameObject communication.

**Rules:**
- Cross-system: always go through a manager event — never hold a direct reference to another system's manager internals
- Same-GameObject: `GetComponent<T>()` in `Awake()`, cache the result, never call it in `Update()`
- UI to gameplay: UI reads from managers only — never writes game state directly

**Example:**
```csharp
// Cross-system: event-driven
EnemyManager.Instance.OnEnemyReachedEnd += HandleLifeLost;

// Same-GameObject: cached component reference
private SpriteRenderer _renderer;
void Awake() => _renderer = GetComponent<SpriteRenderer>();
```

### Entity Creation Patterns

**Enemies & Projectiles:** `ObjectPool<T>` — get from pool on spawn, release to pool on death/completion.

**Towers:** `Instantiate` on placement, `Destroy` on sell. Towers are infrequent, session-long — no pooling needed.

**Rules:**
- Never `Instantiate` enemies or projectiles outside their pool wrappers
- Pool wrappers (`EnemyPool`, `ProjectilePool`) are the only entry point for creating those entities
- Tower instantiation lives in `TowerPlacer.cs` only

**Example:**
```csharp
// Correct: enemy spawned through pool
Enemy enemy = EnemyPool.Instance.Get();
enemy.Initialize(spawnPosition, waveDef);

// Correct: tower placed directly
Tower tower = Instantiate(towerPrefab, gridPosition, Quaternion.identity);
TowerManager.Instance.RegisterTower(tower);

// WRONG: never do this
Enemy enemy = Instantiate(enemyPrefab); // bypasses pool
```

### State Transition Patterns

**Game-level state:** Enum state machine in `GameManager`. All transitions go through `GameManager.SetState(GameState newState)` — no other class changes game state directly.

**Entity-level state:** Simple boolean flags. No state machine classes for individual entities.

**Rules:**
- Only `GameManager` calls `SetState()` — never external systems
- Entity flags are set by the entity itself in response to events, not by managers
- States fire events on transition; subscribers react — managers never poll state

**Example:**
```csharp
// Game state: centralized transition
public void SetState(GameState newState) {
    _currentState = newState;
    OnStateChanged?.Invoke(newState);
}

// Entity state: simple flags
public bool IsSlowed { get; private set; }
public void ApplySlow(float factor) {
    IsSlowed = true;
    _moveSpeed *= factor;
}
```

### Data Access Patterns

**Pattern:** ScriptableObjects wired via Inspector `[SerializeField]`. All SO references cached in `Awake()`. No runtime asset lookups after initialization.

**Rules:**
- Every manager caches its SO references on `Awake()` — never call `Resources.Load()` or find assets at runtime
- SOs are read-only at runtime — never modify SO data during play
- Balance changes happen in the Inspector on the SO asset, not in code

**Example:**
```csharp
[SerializeField] private WaveDatabaseSO _waveDatabase;
[SerializeField] private TowerDefinitionSO[] _towerDefinitions;

void Awake() {
    Debug.Assert(_waveDatabase != null, "[WaveManager] WaveDatabase SO not assigned");
    Debug.Assert(_towerDefinitions.Length == 4, "[TowerManager] Expected 4 tower definitions");
}
```

### Consistency Rules

| Pattern | Rule | Enforcement |
|---|---|---|
| Manager access | Always via `Instance` singleton | No `FindObjectOfType<T>()` calls |
| Event subscription | Subscribe in `OnEnable`, unsubscribe in `OnDisable` | Prevents memory leaks |
| SO references | Assigned in Inspector, cached in `Awake()` | `Debug.Assert` if null |
| Pool usage | Only through pool wrapper classes | No direct `Instantiate` for pooled types |
| State changes | Only owning system modifies its own state | Game state only via `GameManager.SetState()` |
| Logging | Always via `GameLog` with system tag | No bare `Debug.Log()` calls |

---

---

## Architecture Validation

### Validation Summary

| Check | Result | Notes |
|---|---|---|
| Decision Compatibility | ✓ Pass | All 8 decisions coherent — no conflicts |
| GDD Coverage | ✓ Pass | All 9 systems and 6 technical requirements addressed |
| Pattern Completeness | ✓ Pass | All 6 pattern categories defined with examples |
| Epic Mapping | ✓ Pass | All 7 epics mapped to specific locations |
| Document Completeness | ✓ Pass | No placeholders, all mandatory sections present |

### Coverage Report

**Systems Covered:** 9/9
**Patterns Defined:** 4 standard, 0 novel
**Decisions Made:** 8

### Issues Resolved

None — document passed all checks on first validation.

### Validation Date

2026-03-05

---

## Development Environment

### Prerequisites

- Unity Hub (latest)
- Unity 6.3 LTS (install via Unity Hub)
- Node.js 18+ (required for MCP servers)
- npm 9+

### Setup Commands

```bash
# 1. In Unity Hub:
#    New Project → 2D (Core) template → Unity 6.3 LTS
#    Project name: basic-game

# 2. Install mcp-unity:
#    Unity → Window → Package Manager → + → Add package from git URL:
#    https://github.com/CoderGamester/mcp-unity.git

# 3. Install Context7 MCP:
claude mcp add context7 -- npx -y @upstash/context7-mcp

# 4. Start mcp-unity server:
#    Unity → Tools → MCP Unity → Server Window → Start Server
```

### First Steps

1. Create Unity project via Unity Hub using 2D (Core) template
2. Recreate the `Assets/` directory structure defined in the Project Structure section
3. Configure MCP servers per the AI Tooling instructions below
4. Create `GameConstants.cs` and `GameLog.cs` in `Scripts/Core/` as the first files

### AI Tooling (MCP Servers)

**mcp-unity (CoderGamester)**
- Repo: https://github.com/CoderGamester/mcp-unity
- Gives Claude Code direct access to Unity scenes, GameObjects, components,
  console logs, and Test Runner
- Install:
  1. Install Node.js 18+
  2. Unity → Window → Package Manager → + → Add package from git URL:
     `https://github.com/CoderGamester/mcp-unity.git`
  3. Unity → Tools → MCP Unity → Server Window → Start Server
- Requires: Unity 6+, Node.js 18+, npm 9+

**Context7 (upstash)**
- Repo: https://github.com/upstash/context7
- Pulls current Unity API documentation into Claude Code context — prevents
  outdated API usage
- Install: `claude mcp add context7 -- npx -y @upstash/context7-mcp`
- Requires: Node.js
