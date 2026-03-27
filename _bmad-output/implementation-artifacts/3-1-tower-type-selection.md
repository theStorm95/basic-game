# Story 3.1: Tower Type Selection

Status: done

## Story

As a player,
I want to select a tower type from a UI panel,
so that I can enter placement mode with the chosen tower type ready to place.

## Acceptance Criteria

1. `TowerSelectionPanel` displays 4 buttons — Fast, Heavy, AOE, Slow — each labeled with the tower name and placement cost
2. Clicking a tower button calls `TowerPlacer.Instance.EnterPlacementMode(type)`, setting `IsInPlacementMode = true` and `SelectedType` to the chosen type
3. The selected tower button is visually highlighted (distinct color); all others are de-highlighted
4. `EconomyManager` initializes with `GameConstants.STARTING_CURRENCY` (150) on `Awake`; `OnCurrencyChanged` fires immediately so HUD initializes correctly
5. `HudController` shows current currency ("Gold: X") subscribed to `EconomyManager.OnCurrencyChanged`
6. No regression to: lives display, game over/win screens, restart, cheat keys (backtick / F5)

## Tasks / Subtasks

- [x] Task 1: Add foundation data types (AC: 1, 2, 4)
  - [x] Add `STARTING_CURRENCY = 150` constant to `GameConstants.cs`
  - [x] Create `TowerType.cs` enum: `Fast, Heavy, Aoe, Slow`
  - [x] Create `TowerDefinitionSO.cs` ScriptableObject: `TowerType towerType`, `string displayName`, `int placementCost`, `Color buttonColor`

- [x] Task 2: Create `EconomyManager.cs` singleton (AC: 4)
  - [x] Singleton pattern (`public static EconomyManager Instance`) set in `Awake`
  - [x] Private `_currency` field initialized to `GameConstants.STARTING_CURRENCY`
  - [x] `public event Action<int> OnCurrencyChanged`
  - [x] `public int Currency => _currency` property
  - [x] `public void AddCurrency(int amount)` — adds amount, fires `OnCurrencyChanged`
  - [x] `public bool TrySpendCurrency(int amount)` — returns false if insufficient, deducts and fires event if sufficient
  - [x] Fire `OnCurrencyChanged` with initial value at end of `Awake` so subscribers initialize correctly

- [x] Task 3: Create `TowerPlacer.cs` singleton (AC: 2, 3)
  - [x] Singleton pattern set in `Awake`
  - [x] `public bool IsInPlacementMode { get; private set; }`
  - [x] `public TowerType SelectedType { get; private set; }`
  - [x] `public event Action<TowerType> OnPlacementModeEntered`
  - [x] `public event Action OnPlacementModeExited`
  - [x] `public void EnterPlacementMode(TowerType type)` — sets mode true, stores type, fires event
  - [x] `public void ExitPlacementMode()` — sets mode false, fires event

- [x] Task 4: Create `TowerSelectionPanel.cs` UI controller (AC: 1, 2, 3)
  - [x] `[SerializeField] private TowerDefinitionSO[] _towerDefinitions` (length 4, one per type)
  - [x] `[SerializeField] private Button[] _towerButtons` (length 4, matching order)
  - [x] `Debug.Assert` in `Awake` that both arrays are length 4 and non-null
  - [x] On `Start`: wire each button's `onClick` to call `TowerPlacer.Instance.EnterPlacementMode(def.towerType)` and set button label (name + cost)
  - [x] Subscribe to `TowerPlacer.Instance.OnPlacementModeEntered` and `OnPlacementModeExited` in `OnEnable`/`OnDisable`
  - [x] On placement mode entered: highlight the selected button (use `def.buttonColor`); de-highlight others
  - [x] On placement mode exited: de-highlight all buttons (reset to white)

- [x] Task 5: Update `HudController.cs` to show currency (AC: 5, 6)
  - [x] Add `[SerializeField] private Text _currencyText` field
  - [x] Add `Debug.Assert` in `Awake` for `_currencyText`
  - [x] Subscribe to `EconomyManager.Instance.OnCurrencyChanged` in `OnEnable` + immediately update display
  - [x] Unsubscribe in `OnDisable`
  - [x] `UpdateCurrencyDisplay(int amount)` sets `_currencyText.text = $"Gold: {amount}"`

- [x] Task 6: Manual QA (AC: 1–6)
  - [x] Enter Play Mode — tower panel shows 4 buttons with names and costs; HUD shows "Gold: 150"
  - [x] Click Fast button — button highlighted, `TowerPlacer.IsInPlacementMode == true`, `SelectedType == Fast`
  - [x] Click Heavy button — Heavy button highlighted, Fast de-highlighted
  - [x] No placement occurs when clicking buttons (placement mode is set, but no tiles are clicked yet)
  - [x] HUD still shows "Lives: 3" — lives display unaffected
  - [x] Backtick cheat → Game Over screen; F5 → Win screen — no regression
  - [x] Restart from Game Over — tower panel still visible and functional

## Dev Notes

### Architecture Fit

**EconomyManager** goes in `Scripts/Economy/` — Manager Singleton pattern, same as `GameManager`. Fires `OnCurrencyChanged` event so both `HudController` and future placement/upgrade logic react without polling.

**TowerPlacer** goes in `Scripts/Towers/` — manages placement mode state machine. Story 3-2 (range preview) and 3-3 (actual placement) will extend this class. Keeping it a singleton allows `TowerSelectionPanel` to call it cleanly.

**TowerSelectionPanel** goes in `Scripts/UI/` — event-driven subscriber. Reads from `TowerPlacer` events, never mutates game state directly.

**TowerDefinitionSO** goes in `Scripts/Towers/` (definition) with assets in `Data/Towers/` — wired via Inspector on `TowerSelectionPanel`. Each of the 4 types gets its own SO asset.

### Key Implementation Details

**Event initialization pattern** (from HudController precedent): fire `OnCurrencyChanged` at end of `Awake` so any subscriber that calls `OnEnable` before `Start` gets the initial value. Same pattern as `GameManager.RestartGame()` calling `OnLivesChanged?.Invoke(_lives)`.

**Button highlight pattern**: use Unity UGUI `ColorBlock` on each `Button` component. On selection, set the button's `colors.normalColor` to `def.buttonColor`. On deselection, reset to `Color.white`. Alternatively, change a child `Image` component's color directly — both are acceptable.

**TowerType enum values** (match GDD exactly):
- `Fast` — Fast/Weak tower (high fire rate, low damage)
- `Heavy` — Slow/Heavy tower (low fire rate, high damage)
- `Aoe` — AOE tower (area damage)
- `Slow` — Slow tower (utility, no damage)

**Hardcoded costs for testing** (balance TBD in Epic 6):
- Fast: 50 gold
- Heavy: 100 gold
- AOE: 125 gold
- Slow: 75 gold

**STARTING_CURRENCY = 150** — enough to place one Fast tower or one Slow tower; intentionally tight.

### Files to Create/Modify

**Create:**
- `Assets/Scripts/Core/GameConstants.cs` — add STARTING_CURRENCY
- `Assets/Scripts/Towers/TowerType.cs`
- `Assets/Scripts/Towers/TowerDefinitionSO.cs`
- `Assets/Scripts/Economy/EconomyManager.cs`
- `Assets/Scripts/Towers/TowerPlacer.cs`
- `Assets/Scripts/UI/TowerSelectionPanel.cs`
- `Assets/Tests/EditMode/EconomyManagerTests.cs`

**Modify:**
- `Assets/Scripts/UI/HudController.cs` — add currency display

### References

- Architecture: `_bmad-output/game-architecture.md` — Manager Singleton pattern, event subscription rules, UI boundary rules
- Project Context: `_bmad-output/project-context.md` — lifecycle order, anti-patterns, naming conventions
- Prior story: `2-3-lives-count-display.md` — HudController event subscription pattern
- Prior story: `2-4-game-over-on-zero-lives.md` — EconomyManager `AddCurrency` pattern reference (same event pattern)
- Epic 3 scope: `_bmad-output/epics.md` — currency display is hardcoded for testing; real currency system is Epic 5/6

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

None — all code compiled clean per IDE diagnostics (zero errors/warnings on all C# files).

### Completion Notes List

- Task 1 complete: Added `STARTING_CURRENCY = 150` to `GameConstants.cs`. Created `TowerType.cs` (Fast, Heavy, Aoe, Slow) and `TowerDefinitionSO.cs` (towerType, displayName, placementCost, buttonColor) with `[CreateAssetMenu]` attribute — ready to create 4 SO assets in the Inspector.
- Task 2 complete: `EconomyManager.cs` created as Manager Singleton. Initializes to `STARTING_CURRENCY` in `Awake`, fires `OnCurrencyChanged` immediately. `AddCurrency` and `TrySpendCurrency` both fire the event; insufficient-funds case returns false with no event fired. Unit tested in `EconomyManagerTests.cs` (10 tests).
- Task 3 complete: `TowerPlacer.cs` created as Manager Singleton. `EnterPlacementMode` / `ExitPlacementMode` toggle `IsInPlacementMode`, store `SelectedType`, and fire typed events. Calling `EnterPlacementMode` a second time (switching tower type) updates state correctly. Unit tested in `TowerPlacerTests.cs` (8 tests).
- Task 4 complete: `TowerSelectionPanel.cs` wires buttons in `Start()` using `TowerDefinitionSO` array. Each button gets label text (`name + cost`) and an `onClick` listener delegating to `TowerPlacer`. Subscribes to `OnPlacementModeEntered`/`OnPlacementModeExited` in `OnEnable`/`OnDisable`. Highlight uses `ColorBlock.normalColor` and `selectedColor`.
- Task 5 complete: `HudController.cs` updated with `_currencyText` field. Subscribes to `EconomyManager.OnCurrencyChanged` in `OnEnable` and seeds the display immediately. Unsubscribes in `OnDisable`. `_lives` display unchanged — no regression.
- Task 6 complete: Manual QA passed. Note: Unity 6 default buttons use TextMeshPro — switched `_buttonLabels` to explicit `[SerializeField] private Text[] _buttonLabels` using Legacy buttons (UI → Legacy → Button) to stay consistent with the rest of the project's legacy Text usage. All 4 buttons show correct labels and costs, highlight on selection, HUD shows Gold: 150, lives display unaffected, cheat keys and restart unaffected.

### File List

- `Assets/Scripts/Core/GameConstants.cs` (modified — added STARTING_CURRENCY)
- `Assets/Scripts/Towers/TowerType.cs` (created)
- `Assets/Scripts/Towers/TowerDefinitionSO.cs` (created)
- `Assets/Scripts/Economy/EconomyManager.cs` (created)
- `Assets/Scripts/Towers/TowerPlacer.cs` (created)
- `Assets/Scripts/UI/TowerSelectionPanel.cs` (created)
- `Assets/Scripts/UI/HudController.cs` (modified — added currency display)
- `Assets/Tests/EditMode/EconomyManagerTests.cs` (created)
- `Assets/Tests/EditMode/TowerPlacerTests.cs` (created)
- `Assets/Scripts/Towers/Tower_Fast.asset` (created — TowerDefinitionSO asset)
- `Assets/Scripts/Towers/Tower_Heavy.asset` (created — TowerDefinitionSO asset)
- `Assets/Scripts/Towers/Tower_AOE.asset` (created — TowerDefinitionSO asset)
- `Assets/Scripts/Towers/Tower_Slow.asset` (created — TowerDefinitionSO asset)

### Change Log

- Created Epic 3 foundation: TowerType enum, TowerDefinitionSO ScriptableObject, EconomyManager singleton (150 starting currency), TowerPlacer placement-mode state machine, TowerSelectionPanel UI controller, HudController currency display — Story 3.1 Tower Type Selection (Date: 2026-03-25)
