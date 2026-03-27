using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    private int _currency;

    public event Action<int> OnCurrencyChanged;

    public int Currency => _currency;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _currency = GameConstants.STARTING_CURRENCY;
        OnCurrencyChanged?.Invoke(_currency);
    }

    public void AddCurrency(int amount)
    {
        _currency += amount;
        OnCurrencyChanged?.Invoke(_currency);
        GameLog.Info("EconomyManager", $"Currency added: +{amount} → {_currency}");
    }

    public bool TrySpendCurrency(int amount)
    {
        if (_currency < amount)
        {
            GameLog.Info("EconomyManager", $"Insufficient currency: need {amount}, have {_currency}");
            return false;
        }
        _currency -= amount;
        OnCurrencyChanged?.Invoke(_currency);
        GameLog.Info("EconomyManager", $"Currency spent: -{amount} → {_currency}");
        return true;
    }
}
