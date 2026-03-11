---
project_name: 'basic-game'
user_name: 'Nate'
date: '2026-03-05'
sections_completed: ['technology_stack', 'engine_rules', 'performance_rules', 'code_organization', 'testing_rules', 'platform_build_rules', 'critical_rules']
status: 'complete'
rule_count: 42
optimized_for_llm: true
existing_patterns_found: 12
---

# Project Context for AI Agents

_This file contains critical rules and patterns that AI agents must follow when implementing game code in this project. Focus on unobvious details that agents might otherwise miss._

---

## Technology Stack & Versions

| Technology | Version | Role |
|---|---|---|
| Unity | 6.3 LTS | Game engine |
| URP 2D Renderer | Bundled with Unity 6.3 | Sprite rendering pipeline |
| C# | 10 (via Unity's .NET runtime) | Scripting language |
| Unity New Input System | Bundled with Unity 6.3 | Mouse-only input |
| Unity ObjectPool\<T\> | Bundled (Unity.Pool) | Enemy and projectile pooling |
| Unity UGUI | Bundled with Unity 6.3 | All UI (Canvas-based) |
| mcp-unity (CoderGamester) | Latest | AI agent Unity integration |
| Context7 (upstash) | Latest | Unity API docs in context |

**Target:** PC Desktop standalone executable (Windows)
**Resolution:** 2560×1440 default
**Frame Rate:** 60fps minimum, 144fps target

## Critical Implementation Rules

### Critical Don't-Miss Rules

**Anti-Patterns — never do these:**
- `FindObjectOfType<T>()` anywhere — use `Manager.Instance` instead (slow, fragile)
- `GameObject.Find("name")` anywhere — wire references in Inspector
- `public` fields on MonoBehaviours — use `[SerializeField] private` + public property getter
- Modifying ScriptableObject fields at runtime — they mutate the asset on disk in Editor
- Instantiating enemies or projectiles with `Instantiate()` — use pools only
- Subscribing to events in `Start()` without unsubscribing in `OnDisable()` — causes null ref after object death
- `Debug.Log()` calls without `GameLog` wrapper — won't strip in release builds
- Storing world-space positions in UI — use Canvas/RectTransform coordinates for UI

**Tower Defense Gotchas:**
- Towers must NOT store direct Enemy references across frames without null-checking — enemies return to pool and get reused
- When a tower's target dies mid-flight, the projectile should either redirect or self-destruct — handle the null target case
- `WaveDefinitionSO` data is shared — never modify wave data at runtime to track state; use `WaveManager` local variables instead
- Enemy path progress is measured by waypoint index + lerp, not world distance — targeting strategies must use this consistently
- Sell refund is always `Mathf.FloorToInt(placementCost * GameConstants.SELL_REFUND_PERCENT)` — not rounded up

**Event System Gotchas:**
- Always check for null before invoking: `OnStateChanged?.Invoke(newState)` — never bare `OnStateChanged(newState)`
- Events fire AFTER state is already updated — subscribers read the new state, not the old one
- Unsubscribing a handler that was never subscribed does NOT throw — safe to call in `OnDisable` unconditionally

**Unity 6 Specific:**
- `ObjectPool<T>` is in the `UnityEngine.Pool` namespace — add `using UnityEngine.Pool;`
- New Input System requires `using UnityEngine.InputSystem;` — not `UnityEngine.Input`
- URP 2D: use `UniversalRenderPipelineAsset` — do not modify the default URP asset directly, create a copy

---

### Platform & Build Rules

**Target Platform:** PC Desktop (Windows), standalone executable

**Build Configurations:**
- **Development Build** — debug overlay (F1), cheat keys (F2/F3), `GameLog.Info/Warn` active
- **Release Build** — all debug features stripped via `#if UNITY_EDITOR || DEVELOPMENT_BUILD`
- Use IL2CPP backend for release (better performance); Mono acceptable for development iteration

**Input — mouse only:**
- All input goes through Unity New Input System action maps — no legacy `Input.GetKey()` calls
- No keyboard gameplay input except debug keys (F1/F2/F3) gated behind `DEVELOPMENT_BUILD`
- Right-click = cancel placement mode; Left-click = confirm/place
- No controller, touch, or gamepad input — do not add it

**Resolution & Display:**
- Default to 2560×1440, windowed fullscreen
- Do not hardcode pixel positions in UI — use anchors and layout groups
- All UI elements must scale correctly with Canvas Scaler set to Scale With Screen Size

**Debug Conditional Compilation:**
```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // debug-only code here
#endif
```
- All cheat keys, F1 overlay, and verbose logging must be inside this guard
- Never ship `Debug.Log` calls in release — use `GameLog.Info` which strips automatically

---

### Testing Rules

**Testing Infrastructure:**
- Unity Test Runner available via mcp-unity integration
- Edit Mode tests: pure C# logic with no Unity lifecycle (targeting strategies, economy math, wave data)
- Play Mode tests: requires Unity scene — use sparingly for integration validation
- No formal test suite established yet — add tests as systems are implemented

**What to test (Edit Mode):**
- `ITargetingStrategy` implementations — pure logic, easy to unit test
- `EconomyManager` currency arithmetic (earn, spend, sell refund calculations)
- `GameConstants` values — sanity checks
- `WaveDefinitionSO` data integrity (wave 25 exists, boss waves at intervals of 5)

**What NOT to unit test:**
- MonoBehaviour lifecycle (Awake/Start/Update) — test via Play Mode or manual verification
- Rendering, pooling, and pathfinding — validate through play sessions
- UI panel behavior — manual QA only

**Test Naming:**
- `MethodName_Condition_ExpectedResult` (e.g., `SelectTarget_FirstPriority_ReturnsLowestPathProgress`)
- Place test files in `Assets/Tests/EditMode/` or `Assets/Tests/PlayMode/`

---

### Code Organization Rules

**Script Location — every file has exactly one home:**
- `Scripts/Core/` — GameManager, GameState, GameConstants, GameLog only
- `Scripts/Grid/` — GridManager, Tile, PathManager only
- `Scripts/Enemies/` — Enemy, EnemyManager, EnemyPool only
- `Scripts/Towers/` — Tower, TowerManager, TowerPlacer + `Targeting/` subfolder
- `Scripts/Towers/Targeting/` — ITargetingStrategy + 5 strategy implementations only
- `Scripts/Combat/` — Projectile, ProjectilePool, AoeExplosion only
- `Scripts/Waves/` — WaveManager only
- `Scripts/Economy/` — EconomyManager only
- `Scripts/UI/` — All UGUI panel controller scripts only
- `Scripts/Utils/` — GameLog only
- Never place scripts at the `Assets/` root or `Assets/Scripts/` root

**Architectural Boundaries — never cross these:**
- `Scripts/Core/` has zero imports from feature folders — it is imported, never imports
- `Scripts/UI/` subscribes to manager events only — no gameplay logic, no direct state mutation
- `Scripts/Combat/` does not reference `Scripts/Towers/` — damage is applied to Enemy directly
- Manager scripts never reference UI scripts — UI listens, managers don't push

**One Class Per File — always:**
- Each `.cs` file contains exactly one class, struct, or interface
- Filename must exactly match class name (Unity requirement)
- Enums may be in their own file (`GameState.cs`) or co-located with their primary consumer

**Naming — no exceptions:**
- Classes/Methods/Properties: `PascalCase`
- Private fields: `_camelCase` with underscore prefix
- Constants: `UPPER_SNAKE_CASE`
- Interfaces: `I` prefix + `PascalCase` (e.g., `ITargetingStrategy`)
- ScriptableObjects: `SO` suffix (e.g., `TowerDefinitionSO`)
- C# events: `On` + past-tense verb (e.g., `OnStateChanged`, `OnEnemyReachedEnd`)
- Sprites: `snake_case` (e.g., `tower_fast_lvl1.png`)
- Prefabs: `PascalCase` matching script name (e.g., `Tower_Fast.prefab`)

---

### Performance Rules

**Frame Budget:**
- 144fps target = ~6.9ms per frame total
- `Update()` on Enemy and Tower scripts are the hottest paths — zero allocations allowed
- Profile with Unity Profiler before optimizing — don't guess

**Zero-Allocation Rules for Hot Paths:**
- Never use LINQ in `Update()`, `FixedUpdate()`, or any per-frame method
- Never use `new List<T>()` in `Update()` — pre-allocate and reuse collections
- Never use string concatenation in `Update()` — use `GameLog.Info` only outside hot paths
- Never call `GetComponent<T>()` in `Update()` — always cached in `Awake()`

**Object Pooling — mandatory for projectiles and enemies:**
- All enemy spawning goes through `EnemyPool.Instance.Get()` only
- All projectile spawning goes through `ProjectilePool.Instance.Get()` only
- On death/completion: call `EnemyPool.Instance.Release(enemy)` / `ProjectilePool.Instance.Release(projectile)`
- Reset all state in the pooled object's `OnGetFromPool()` callback — never rely on constructor defaults

**AOE Hit Detection:**
- Brute-force distance check against active enemy list is acceptable (expected <50 enemies on screen)
- Cache `EnemyManager.Instance.ActiveEnemies` list reference — don't call GetEnemies() every frame
- Use `sqrMagnitude` instead of `magnitude` for distance comparisons (avoids sqrt)

**Targeting Loop:**
- Towers run targeting on a configurable interval (e.g., every 0.1–0.2s), not every frame
- Use `InvokeRepeating` or a timer accumulator — never raw per-frame targeting scans

---

### Engine-Specific Rules (Unity)

**Lifecycle Order — always follow this sequence:**
- `Awake()` — cache component references, initialize internal state
- `OnEnable()` — subscribe to C# events
- `Start()` — read from other components/managers (safe after all Awakes run)
- `OnDisable()` — unsubscribe from all C# events (prevents memory leaks)
- `OnDestroy()` — cleanup only if OnDisable is not sufficient

**Serialization Rules:**
- Use `[SerializeField] private` — never `public` fields on MonoBehaviours
- `Debug.Assert` in `Awake()` for every `[SerializeField]` reference — fail fast if not wired
- Never modify ScriptableObject data at runtime — they are shared assets; copy values to local fields if mutation is needed
- ScriptableObjects are read-only contracts; treat them as immutable configuration

**Singleton Pattern:**
- All manager singletons use `public static T Instance { get; private set; }`
- Set `Instance = this` in `Awake()`, null-check and destroy if duplicate
- Never use `FindObjectOfType<T>()` — always access via `ManagerName.Instance`
- Never cache a manager reference across scene loads (N/A here — single scene, but good habit)

**Coroutines vs Async:**
- Prefer coroutines (`IEnumerator` + `StartCoroutine`) over `async/await` for game loops and timed sequences
- Never start coroutines in `Awake()` — use `Start()` or later
- Always stop coroutines before the GameObject is disabled (store reference from `StartCoroutine()`)

**GetComponent Rules:**
- Cache ALL `GetComponent<T>()` calls in `Awake()` — never call in `Update()` or hot paths
- Prefer `TryGetComponent<T>()` when component may not exist (avoids null allocation)

---

## Usage Guidelines

**For AI Agents:**
- Read this file before implementing any game code
- Follow ALL rules exactly as documented
- When in doubt, prefer the more restrictive option
- If you discover a new pattern not covered here, flag it for addition

**For Humans:**
- Keep this file lean and focused on agent needs
- Update when technology stack or patterns change
- Remove rules that become obvious over time as the codebase matures

Last Updated: 2026-03-05
