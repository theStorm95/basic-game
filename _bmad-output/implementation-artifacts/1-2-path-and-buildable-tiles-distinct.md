# Story 1.2: Path and Buildable Tiles Distinct

Status: done

<!-- Note: Validation is optional. Run validate-create-story for quality check before dev-story. -->

## Story

As a player,
I want to see path tiles and buildable tiles visually distinct on the grid,
so that I can identify where enemies travel and where I can place towers.

## Acceptance Criteria

1. Path tiles render in a visually distinct color from buildable tiles (e.g., brown/tan for path, green for buildable)
2. All grid cells that the enemy path passes through are correctly identified and rendered as path tiles
3. All remaining tiles not on the path are rendered as buildable tiles in a distinct color
4. The `Tile` class exposes a `TileType` property (enum: `Path` or `Buildable`) for use by future systems (tower placement, enemy routing)
5. The path tile determination is driven by `PathManager` waypoints — no hardcoded per-tile logic in `GridManager`
6. No gameplay systems are added — visual distinction only; nothing is interactive yet

## Tasks / Subtasks

- [x] Task 1: Add `TileType` enum and update `Tile.cs` (AC: 4)
  - [x] Add `TileType` enum with values `Path` and `Buildable` (can live in `Tile.cs` or its own file `TileType.cs` in `Scripts/Grid/`)
  - [x] Add `[SerializeField] private TileType _tileType` field to `Tile.cs`
  - [x] Add `public TileType TileType { get; private set; }` read-only property
  - [x] Add `public void Initialize(TileType tileType, Color color)` method (replaces or supplements any prior init approach)
  - [x] Tile sets `SpriteRenderer` color in `Initialize()` based on tile type

- [x] Task 2: Add path-cell query to `PathManager` (AC: 5)
  - [x] Add `public HashSet<Vector2Int> GetPathCells(int gridWidth, int gridHeight)` method to `PathManager`
  - [x] Method iterates each waypoint segment (straight horizontal/vertical lines only — path is axis-aligned)
  - [x] For each segment, step through each integer grid cell the segment passes through and add to the HashSet
  - [x] Convert world positions to grid cell coordinates: `Vector2Int cell = new Vector2Int(Mathf.RoundToInt(worldX), Mathf.RoundToInt(worldY))`
  - [x] Return the full set of path cells so `GridManager` can mark tiles

- [x] Task 3: Update `GridManager` to mark and color tiles (AC: 1, 2, 3, 5)
  - [x] After grid creation, call `PathManager.Instance.GetPathCells(GRID_WIDTH, GRID_HEIGHT)` to get path cell set
  - [x] Iterate all tiles; for each tile, check if its grid position is in the path cell set
  - [x] Call `tile.Initialize(TileType.Path, pathColor)` or `tile.Initialize(TileType.Buildable, buildableColor)`
  - [x] Define `[SerializeField] private Color _pathTileColor = new Color(0.76f, 0.60f, 0.42f)` (tan/brown) in `GridManager`
  - [x] Define `[SerializeField] private Color _buildableTileColor = new Color(0.27f, 0.55f, 0.27f)` (green) in `GridManager`
  - [x] Verify in Play Mode: path follows the expected route, buildable tiles surround it

- [x] Task 4: Verify architecture compliance and compile (AC: 6)
  - [x] No `FindObjectOfType<T>()` calls — access `PathManager` via `PathManager.Instance`
  - [x] All new fields use `[SerializeField] private` — no public fields on MonoBehaviours
  - [x] All scripts compile with zero errors in Unity console
  - [x] No gameplay logic added — tiles are purely visual

## Dev Notes

### Context from Story 1.1

Story 1.1 created the full grid scaffolding. The following files already exist and **must not be recreated**:

| File | Location | Notes |
|---|---|---|
| `GridManager.cs` | `Assets/Scripts/Grid/` | Creates 20×12 grid, stores `Tile[,] _grid` |
| `Tile.cs` | `Assets/Scripts/Grid/` | Per-cell GameObject with `SpriteRenderer` |
| `PathManager.cs` | `Assets/Scripts/Grid/` | Stores `Vector2[] _waypoints`, exposes as public property |
| `GameManager.cs` | `Assets/Scripts/Core/` | Singleton, state machine |
| `GameConstants.cs` | `Assets/Scripts/Core/` | `GRID_WIDTH = 20`, `GRID_HEIGHT = 12` |
| `GameLog.cs` | `Assets/Scripts/Utils/` | Tagged logging wrapper |

**Story 1.1 established:**
- All tiles are the same placeholder white color — this story adds the color distinction
- `PathManager._waypoints` defines the path as `Vector2[]` world-space positions
- Grid is centered: x from -10 to 9, y from -6 to 5; each tile = 1 Unity unit

### Path Tile Marking Algorithm

The path consists of **axis-aligned segments** (horizontal or vertical only — no diagonals). The marking algorithm walks each segment:

```csharp
public HashSet<Vector2Int> GetPathCells()
{
    var cells = new HashSet<Vector2Int>();
    for (int i = 0; i < _waypoints.Length - 1; i++)
    {
        Vector2 from = _waypoints[i];
        Vector2 to   = _waypoints[i + 1];

        // Horizontal segment
        if (Mathf.Approximately(from.y, to.y))
        {
            int y = Mathf.RoundToInt(from.y);
            int xMin = Mathf.RoundToInt(Mathf.Min(from.x, to.x));
            int xMax = Mathf.RoundToInt(Mathf.Max(from.x, to.x));
            for (int x = xMin; x <= xMax; x++)
                cells.Add(new Vector2Int(x, y));
        }
        // Vertical segment
        else
        {
            int x = Mathf.RoundToInt(from.x);
            int yMin = Mathf.RoundToInt(Mathf.Min(from.y, to.y));
            int yMax = Mathf.RoundToInt(Mathf.Max(from.y, to.y));
            for (int y = yMin; y <= yMax; y++)
                cells.Add(new Vector2Int(x, y));
        }
    }
    return cells;
}
```

**Note:** `HashSet<Vector2Int>` is allocated once at initialization — this is fine (not a hot path). Store the result in `GridManager` if any future query needs repeated access, to avoid repeated allocation.

### Tile.cs Update Pattern

Extend the existing `Tile.cs` (do NOT recreate it):

```csharp
using UnityEngine;

public enum TileType { Path, Buildable }

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;  // already exists from Story 1.1

    public TileType TileType { get; private set; }

    public void Initialize(TileType tileType, Color color)
    {
        TileType = tileType;
        _renderer.color = color;
    }
}
```

**If `TileType` is placed in a separate file**, use `Assets/Scripts/Grid/TileType.cs`. Either co-location or separate file is acceptable — pick one and be consistent.

### GridManager.cs Update Pattern

After the grid creation loop in `GridManager`, add:

```csharp
[SerializeField] private Color _pathTileColor     = new Color(0.76f, 0.60f, 0.42f); // tan
[SerializeField] private Color _buildableTileColor = new Color(0.27f, 0.55f, 0.27f); // green

private void ColorTiles()
{
    HashSet<Vector2Int> pathCells = PathManager.Instance.GetPathCells();

    for (int col = 0; col < GameConstants.GRID_WIDTH; col++)
    {
        for (int row = 0; row < GameConstants.GRID_HEIGHT; row++)
        {
            // World-space position of this tile's center
            // (mirrors the positioning from Story 1.1's grid creation)
            int worldX = col - GameConstants.GRID_WIDTH / 2;
            int worldY = row - GameConstants.GRID_HEIGHT / 2;

            TileType type = pathCells.Contains(new Vector2Int(worldX, worldY))
                ? TileType.Path
                : TileType.Buildable;

            _grid[col, row].Initialize(type, type == TileType.Path ? _pathTileColor : _buildableTileColor);
        }
    }
}
```

Call `ColorTiles()` at the end of `GridManager.Start()` (or wherever the grid is built), **after** `PathManager.Instance` is guaranteed to exist. Use `Start()` not `Awake()` to ensure all singletons are initialized.

### Architecture Compliance

- **No `FindObjectOfType<T>()`** — access `PathManager.Instance`; the singleton must be set up identically to `GameManager`
- **`[SerializeField] private`** for all new Color fields in `GridManager`
- **`Debug.Assert`** in `GridManager.Awake()` if `PathManager.Instance` is null after scene load
- **`Scripts/Core/`** has zero new additions — `TileType` enum belongs in `Scripts/Grid/`
- **Architectural boundary respected** — `GridManager` calls `PathManager.GetPathCells()`, not the other way

### PathManager Singleton Check

Story 1.1 created `PathManager` but did not necessarily make it a singleton (it only stored waypoints). Confirm whether `PathManager` already has:

```csharp
public static PathManager Instance { get; private set; }
void Awake() { Instance = this; }
```

If not, **add the singleton pattern** to `PathManager` now — `GridManager` needs to access it via `Instance`.

### Project Structure Notes

- `TileType` enum: add to `Tile.cs` directly (co-location) OR create `Assets/Scripts/Grid/TileType.cs` — document choice in File List
- No new folders needed for this story
- Exact file locations from Story 1.1 must be preserved — do not move any existing scripts

### References

- Architecture: `_bmad-output/game-architecture.md` → Grid/Map system, Project Structure, Cross-cutting Concerns
- Epics: `_bmad-output/epics.md` → Epic 1: Game Foundation, Story 2
- Project Context: `_bmad-output/project-context.md` → Engine-Specific Rules, Code Organization Rules, Anti-Patterns
- Previous Story: `_bmad-output/implementation-artifacts/1-1-launch-game-and-see-grid-map.md` → Grid System Design, Core Script Templates

## Dev Agent Record

### Agent Model Used

claude-sonnet-4-6

### Debug Log References

_none_

### Completion Notes List

- Task 1: Added `TileType` enum (Path/Buildable) co-located in `Tile.cs`. Added `TileType` read-only property and new `Initialize(TileType, Color)` overload. Kept existing `Initialize(int col, int row)` for backward compatibility with `GridManager.BuildGrid()`.
- Task 2: Added `GetPathCells()` instance method and `ComputePathCells(Vector2[] waypoints)` static method to `PathManager`. Static method enables EditMode unit testing without MonoBehaviour instantiation. Algorithm walks each axis-aligned waypoint segment using `Mathf.Approximately` for horizontal/vertical detection.
- Task 3: Added `_pathTileColor` (tan) and `_buildableTileColor` (green) as `[SerializeField] private` Color fields to `GridManager`. Added `Start()` method that calls `ColorTiles()` after all Awake singletons are ready. `ColorTiles()` maps (col, row) to integer world coordinates via `col - GRID_WIDTH/2`, looks up each cell in the `HashSet<Vector2Int>`, and calls `tile.Initialize(TileType, Color)` accordingly. Removed the hardcoded placeholder color from `BuildGrid()`.
- Task 4: Verified all architecture compliance rules. Zero compile errors confirmed via IDE diagnostics and Unity batch test run.
- All 21 EditMode tests pass (10 pre-existing, 11 new). No regressions.

### File List

- `Assets/Scripts/Grid/Tile.cs` — updated: added `TileType` enum (co-located), `TileType` property, `Initialize(TileType, Color)` overload
- `Assets/Scripts/Grid/PathManager.cs` — updated: added `GetPathCells()` instance method, `ComputePathCells(Vector2[])` static method
- `Assets/Scripts/Grid/GridManager.cs` — updated: added `_pathTileColor`, `_buildableTileColor` fields, `Start()` method, `ColorTiles()` method; removed hardcoded placeholder color from `BuildGrid()`
- `Assets/Tests/EditMode/PathTileTests.cs` — new: 11 EditMode tests covering `TileType` enum and `PathManager.ComputePathCells` algorithm

## Change Log

- 2026-03-09: Story 1.2 implemented — added TileType enum, path cell computation algorithm, and GridManager coloring. Path tiles render tan/brown, buildable tiles render green. 21 tests pass.
- 2026-03-10: Code review fixes applied — (M1) null/empty guard added to `ComputePathCells`; (M2) mutability warning comment added to `DefaultWaypoints`; (M3) renamed `worldX/worldY` to `gridX/gridY` in `ColorTiles()` with coordinate-system comment; (M4) added 3 `Tile.Initialize` EditMode tests; (M5) `Initialize(col, row)` now explicitly sets `TileType = TileType.Buildable` as safe default; (M6) `Start()` null guard now returns early instead of relying solely on non-halting `Debug.Assert`. Tests: 2 tautological enum tests replaced with behavioral tests; corner test simplified; 2 null-input guard tests added. Total: 26 tests.
