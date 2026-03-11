using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PathTileTests
{
    // --- TileType enum ---

    [Test]
    public void TileType_PathAndBuildable_AreDistinctValues()
    {
        Assert.AreNotEqual(TileType.Path, TileType.Buildable);
    }

    [Test]
    public void TileType_DefaultEnumValue_IsBuildable()
    {
        // Ensures the enum ordering won't silently flip the default TileType
        // in Tile.Initialize(col, row) (which now explicitly sets Buildable).
        TileType defaultValue = default;
        Assert.AreNotEqual(TileType.Path, defaultValue,
            "TileType.Path must not be the C# default (0) — Buildable must be the safe uninitialized state");
    }

    // --- Tile.Initialize tests ---

    [Test]
    public void Tile_Initialize_WithPathType_SetsTileTypeToPath()
    {
        var go = new GameObject();
        go.AddComponent<SpriteRenderer>();
        var tile = go.AddComponent<Tile>();

        tile.Initialize(TileType.Path, Color.red);

        Assert.AreEqual(TileType.Path, tile.TileType);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Tile_Initialize_WithBuildableType_SetsTileTypeToBuildable()
    {
        var go = new GameObject();
        go.AddComponent<SpriteRenderer>();
        var tile = go.AddComponent<Tile>();

        tile.Initialize(TileType.Buildable, Color.green);

        Assert.AreEqual(TileType.Buildable, tile.TileType);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Tile_InitializeColRow_DefaultsToBuildable()
    {
        var go = new GameObject();
        go.AddComponent<SpriteRenderer>();
        var tile = go.AddComponent<Tile>();

        tile.Initialize(0, 0);

        Assert.AreEqual(TileType.Buildable, tile.TileType,
            "Before ColorTiles runs, all tiles must default to Buildable — not Path");
        Object.DestroyImmediate(go);
    }

    // --- ComputePathCells static algorithm ---

    [Test]
    public void ComputePathCells_NullWaypoints_ReturnsEmptySet()
    {
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(null);
        Assert.AreEqual(0, cells.Count);
    }

    [Test]
    public void ComputePathCells_SingleWaypoint_ReturnsEmptySet()
    {
        var waypoints = new Vector2[] { new Vector2(0f, 0f) };
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(waypoints);
        Assert.AreEqual(0, cells.Count);
    }

    [Test]
    public void ComputePathCells_SimpleHorizontalSegment_ReturnsAllCells()
    {
        var waypoints = new Vector2[] { new Vector2(0f, 0f), new Vector2(3f, 0f) };
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(waypoints);

        Assert.IsTrue(cells.Contains(new Vector2Int(0, 0)));
        Assert.IsTrue(cells.Contains(new Vector2Int(1, 0)));
        Assert.IsTrue(cells.Contains(new Vector2Int(2, 0)));
        Assert.IsTrue(cells.Contains(new Vector2Int(3, 0)));
        Assert.AreEqual(4, cells.Count);
    }

    [Test]
    public void ComputePathCells_SimpleVerticalSegment_ReturnsAllCells()
    {
        var waypoints = new Vector2[] { new Vector2(2f, -1f), new Vector2(2f, 2f) };
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(waypoints);

        Assert.IsTrue(cells.Contains(new Vector2Int(2, -1)));
        Assert.IsTrue(cells.Contains(new Vector2Int(2,  0)));
        Assert.IsTrue(cells.Contains(new Vector2Int(2,  1)));
        Assert.IsTrue(cells.Contains(new Vector2Int(2,  2)));
        Assert.AreEqual(4, cells.Count);
    }

    [Test]
    public void ComputePathCells_WithDefaultWaypoints_ContainsEntryCell()
    {
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(PathManager.DefaultWaypoints);
        Assert.IsTrue(cells.Contains(new Vector2Int(-10, 2)), "Entry cell (-10,2) must be on path");
    }

    [Test]
    public void ComputePathCells_WithDefaultWaypoints_ContainsExitCell()
    {
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(PathManager.DefaultWaypoints);
        Assert.IsTrue(cells.Contains(new Vector2Int(10, 3)), "Exit cell (10,3) must be on path");
    }

    [Test]
    public void ComputePathCells_WithDefaultWaypoints_ContainsMidHorizontalCell()
    {
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(PathManager.DefaultWaypoints);
        // (0,-2) is on the segment from (-3,-2) to (4,-2)
        Assert.IsTrue(cells.Contains(new Vector2Int(0, -2)), "Cell (0,-2) must be on path");
    }

    [Test]
    public void ComputePathCells_WithDefaultWaypoints_ContainsMidVerticalCell()
    {
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(PathManager.DefaultWaypoints);
        // (-3,0) is on the vertical segment from (-3,2) to (-3,-2)
        Assert.IsTrue(cells.Contains(new Vector2Int(-3, 0)), "Cell (-3,0) must be on path");
    }

    [Test]
    public void ComputePathCells_WithDefaultWaypoints_DoesNotContainBuildableCell()
    {
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(PathManager.DefaultWaypoints);
        Assert.IsFalse(cells.Contains(new Vector2Int(0, 0)), "Cell (0,0) is buildable, not on path");
    }

    [Test]
    public void ComputePathCells_WithDefaultWaypoints_HasExpectedCellCount()
    {
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(PathManager.DefaultWaypoints);
        // Segments (new cells only, shared corners counted once):
        // (-10 to -3, y=2) → 8  |  (-3, 2 to -2) → 4 new  |  (-3 to 4, y=-2) → 7 new
        // (4, -2 to 3) → 5 new  |  (4 to 10, y=3) → 6 new  = 30 total
        Assert.AreEqual(30, cells.Count, "Default waypoints should produce 30 unique path cells");
    }

    [Test]
    public void ComputePathCells_SharedCornerCells_NotCountedTwice()
    {
        // Two segments sharing a corner: (0,0)→(2,0) and (2,0)→(2,2)
        var waypoints = new Vector2[] { new Vector2(0f, 0f), new Vector2(2f, 0f), new Vector2(2f, 2f) };
        HashSet<Vector2Int> cells = PathManager.ComputePathCells(waypoints);

        // HashSet guarantees uniqueness; total count proves corner wasn't duplicated
        Assert.IsTrue(cells.Contains(new Vector2Int(2, 0)), "Corner cell must be present");
        Assert.AreEqual(5, cells.Count, "(0,0),(1,0),(2,0),(2,1),(2,2) = 5 unique cells");
    }
}
