using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Text _livesText;
    [SerializeField] private Text _currencyText;

    private void Awake()
    {
        Debug.Assert(_livesText != null, "[HudController] _livesText is not wired");
        Debug.Assert(_currencyText != null, "[HudController] _currencyText is not wired");
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[HudController] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnLivesChanged += UpdateLivesDisplay;
        UpdateLivesDisplay(GameManager.Instance.Lives);

        Debug.Assert(EconomyManager.Instance != null, "[HudController] EconomyManager.Instance is null in OnEnable");
        EconomyManager.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
        UpdateCurrencyDisplay(EconomyManager.Instance.Currency);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnLivesChanged -= UpdateLivesDisplay;

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
    }

    private void UpdateLivesDisplay(int lives)
    {
        _livesText.text = $"Lives: {lives}";
        GameLog.Info("HudController", $"Lives display updated: {lives}");
    }

    private void UpdateCurrencyDisplay(int amount)
    {
        _currencyText.text = $"Gold: {amount}";
        GameLog.Info("HudController", $"Currency display updated: {amount}");
    }
}
