using NUnit.Framework;
using UnityEngine;

public class GameConstantsTests
{
    [Test]
    public void GridWidth_Is20()
    {
        Assert.AreEqual(20, GameConstants.GRID_WIDTH);
    }

    [Test]
    public void GridHeight_Is12()
    {
        Assert.AreEqual(12, GameConstants.GRID_HEIGHT);
    }

    [Test]
    public void MaxLives_Is3()
    {
        Assert.AreEqual(3, GameConstants.MAX_LIVES);
    }

    [Test]
    public void MaxWaves_Is25()
    {
        Assert.AreEqual(25, GameConstants.MAX_WAVES);
    }

    [Test]
    public void SellRefundPercent_Is75Percent()
    {
        Assert.AreEqual(0.75f, GameConstants.SELL_REFUND_PERCENT, 0.0001f);
    }

    [Test]
    public void BossWaveInterval_Is5()
    {
        Assert.AreEqual(5, GameConstants.BOSS_WAVE_INTERVAL);
    }

    // PathManager waypoints — static array, testable without MonoBehaviour instantiation

    [Test]
    public void PathManager_DefaultWaypoints_HasSixPoints()
    {
        Assert.AreEqual(6, PathManager.DefaultWaypoints.Length);
    }

    [Test]
    public void PathManager_DefaultWaypoints_EntryIsOnLeftSide()
    {
        Assert.Less(PathManager.DefaultWaypoints[0].x, 0f,
            "Entry waypoint should be on the left side of the grid");
    }

    [Test]
    public void PathManager_DefaultWaypoints_ExitIsOnRightSide()
    {
        int last = PathManager.DefaultWaypoints.Length - 1;
        Assert.Greater(PathManager.DefaultWaypoints[last].x, 0f,
            "Exit waypoint should be on the right side of the grid");
    }

    [Test]
    public void PathManager_DefaultWaypoints_AllPointsWithinGridBounds()
    {
        float halfW = GameConstants.GRID_WIDTH / 2f;
        float halfH = GameConstants.GRID_HEIGHT / 2f;

        foreach (var wp in PathManager.DefaultWaypoints)
        {
            Assert.LessOrEqual(Mathf.Abs(wp.x), halfW + 1f,
                $"Waypoint {wp} X is too far outside grid bounds");
            Assert.LessOrEqual(Mathf.Abs(wp.y), halfH + 1f,
                $"Waypoint {wp} Y is too far outside grid bounds");
        }
    }
}
