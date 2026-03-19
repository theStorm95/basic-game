using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class WinPanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private Button _restartButton;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(_canvasGroup != null, "[WinPanel] CanvasGroup component is missing");
        Debug.Assert(_restartButton != null, "[WinPanel] _restartButton is not wired");
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        Debug.Assert(GameManager.Instance != null, "[WinPanel] GameManager.Instance is null in OnEnable");
        GameManager.Instance.OnStateChanged += HandleStateChanged;
        HandleStateChanged(GameManager.Instance.CurrentState);
        _restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
        _restartButton.onClick.RemoveListener(OnRestartClicked);
    }

    private void HandleStateChanged(GameState newState)
    {
        bool show = newState == GameState.Win;
        _canvasGroup.alpha = show ? 1f : 0f;
        _canvasGroup.interactable = show;
        _canvasGroup.blocksRaycasts = show;
        GameLog.Info("WinPanel", show ? "Showing win panel" : "Hiding win panel");
    }

    private void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }
}
