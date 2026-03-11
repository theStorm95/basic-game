using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState = GameState.PreWave;

    public event System.Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
    }

    public void SetState(GameState newState)
    {
        _currentState = newState;
        OnStateChanged?.Invoke(newState);
        GameLog.Info("GameManager", $"State changed to {newState}");
    }

    public GameState CurrentState => _currentState;
}
