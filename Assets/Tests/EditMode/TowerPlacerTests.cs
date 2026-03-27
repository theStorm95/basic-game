using NUnit.Framework;
using UnityEngine;

public class TowerPlacerTests
{
    private GameObject _go;
    private TowerPlacer _placer;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("TowerPlacer");
        _placer = _go.AddComponent<TowerPlacer>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_go);
    }

    [Test]
    public void AfterAwake_IsInPlacementMode_IsFalse()
    {
        Assert.IsFalse(_placer.IsInPlacementMode);
    }

    [Test]
    public void EnterPlacementMode_SetsIsInPlacementModeTrue()
    {
        _placer.EnterPlacementMode(TowerType.Fast);
        Assert.IsTrue(_placer.IsInPlacementMode);
    }

    [Test]
    public void EnterPlacementMode_SetsSelectedType()
    {
        _placer.EnterPlacementMode(TowerType.Heavy);
        Assert.AreEqual(TowerType.Heavy, _placer.SelectedType);
    }

    [Test]
    public void EnterPlacementMode_FiresOnPlacementModeEntered()
    {
        TowerType received = TowerType.Fast;
        bool fired = false;
        _placer.OnPlacementModeEntered += t => { received = t; fired = true; };

        _placer.EnterPlacementMode(TowerType.Aoe);

        Assert.IsTrue(fired);
        Assert.AreEqual(TowerType.Aoe, received);
    }

    [Test]
    public void ExitPlacementMode_SetsIsInPlacementModeFalse()
    {
        _placer.EnterPlacementMode(TowerType.Slow);
        _placer.ExitPlacementMode();
        Assert.IsFalse(_placer.IsInPlacementMode);
    }

    [Test]
    public void ExitPlacementMode_FiresOnPlacementModeExited()
    {
        bool fired = false;
        _placer.OnPlacementModeExited += () => fired = true;
        _placer.EnterPlacementMode(TowerType.Fast);

        _placer.ExitPlacementMode();

        Assert.IsTrue(fired);
    }

    [Test]
    public void EnterPlacementMode_CalledTwice_UpdatesSelectedType()
    {
        _placer.EnterPlacementMode(TowerType.Fast);
        _placer.EnterPlacementMode(TowerType.Heavy);

        Assert.AreEqual(TowerType.Heavy, _placer.SelectedType);
        Assert.IsTrue(_placer.IsInPlacementMode);
    }

    [Test]
    public void TowerType_HasExactlyFourValues()
    {
        Assert.AreEqual(4, System.Enum.GetValues(typeof(TowerType)).Length,
            "TowerType should have exactly 4 values");
    }
}
