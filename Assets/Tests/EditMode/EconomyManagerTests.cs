using NUnit.Framework;
using UnityEngine;

public class EconomyManagerTests
{
    private GameObject _go;
    private EconomyManager _economy;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("EconomyManager");
        _economy = _go.AddComponent<EconomyManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_go);
    }

    [Test]
    public void StartingCurrency_Constant_Is150()
    {
        Assert.AreEqual(150, GameConstants.STARTING_CURRENCY);
    }

    [Test]
    public void Currency_AfterAwake_EqualsStartingCurrency()
    {
        Assert.AreEqual(GameConstants.STARTING_CURRENCY, _economy.Currency);
    }

    [Test]
    public void AddCurrency_PositiveAmount_IncreasesCurrency()
    {
        _economy.AddCurrency(50);
        Assert.AreEqual(GameConstants.STARTING_CURRENCY + 50, _economy.Currency);
    }

    [Test]
    public void AddCurrency_FiresOnCurrencyChanged()
    {
        int received = -1;
        _economy.OnCurrencyChanged += v => received = v;

        _economy.AddCurrency(25);

        Assert.AreEqual(GameConstants.STARTING_CURRENCY + 25, received);
    }

    [Test]
    public void TrySpendCurrency_SufficientFunds_ReturnsTrueAndDeducts()
    {
        bool result = _economy.TrySpendCurrency(50);

        Assert.IsTrue(result);
        Assert.AreEqual(GameConstants.STARTING_CURRENCY - 50, _economy.Currency);
    }

    [Test]
    public void TrySpendCurrency_InsufficientFunds_ReturnsFalseAndNoChange()
    {
        bool result = _economy.TrySpendCurrency(GameConstants.STARTING_CURRENCY + 1);

        Assert.IsFalse(result);
        Assert.AreEqual(GameConstants.STARTING_CURRENCY, _economy.Currency);
    }

    [Test]
    public void TrySpendCurrency_ExactAmount_ReturnsTrueAndCurrencyIsZero()
    {
        bool result = _economy.TrySpendCurrency(GameConstants.STARTING_CURRENCY);

        Assert.IsTrue(result);
        Assert.AreEqual(0, _economy.Currency);
    }

    [Test]
    public void TrySpendCurrency_Success_FiresOnCurrencyChanged()
    {
        int received = -1;
        _economy.OnCurrencyChanged += v => received = v;

        _economy.TrySpendCurrency(50);

        Assert.AreEqual(GameConstants.STARTING_CURRENCY - 50, received);
    }

    [Test]
    public void TrySpendCurrency_Failure_DoesNotFireOnCurrencyChanged()
    {
        int callCount = 0;
        _economy.OnCurrencyChanged += _ => callCount++;

        _economy.TrySpendCurrency(GameConstants.STARTING_CURRENCY + 1);

        Assert.AreEqual(0, callCount);
    }

}
