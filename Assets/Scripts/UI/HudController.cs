using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Text _livesText;

    private void Awake()
    {
        Debug.Assert(_livesText != null, "[HudController] _livesText is not wired");
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[HudController] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnLivesChanged += UpdateLivesDisplay;
        UpdateLivesDisplay(GameManager.Instance.Lives);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnLivesChanged -= UpdateLivesDisplay;
    }

    private void UpdateLivesDisplay(int lives)
    {
        _livesText.text = $"Lives: {lives}";
        GameLog.Info("HudController", $"Lives display updated: {lives}");
    }
}
