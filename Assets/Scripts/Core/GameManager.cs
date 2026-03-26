using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState = GameState.PreWave;
    private int _lives = GameConstants.MAX_LIVES;

    public event System.Action<GameState> OnStateChanged;
    public event System.Action<int> OnLivesChanged;

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
    public int Lives => _lives;

    public void RestartGame()
    {
        _lives = GameConstants.MAX_LIVES;
        OnLivesChanged?.Invoke(_lives);
        GameLog.Info("GameManager", "RestartGame called — resetting to PreWave");
        SetState(GameState.PreWave);
    }

    public void LoseLife()
    {
        if (_lives == 0) return;
        _lives = Mathf.Max(0, _lives - 1);
        OnLivesChanged?.Invoke(_lives);
        if (_lives == 0) SetState(GameState.GameOver);
        GameLog.Info("GameManager", $"Life lost — {_lives} remaining");
    }

    private void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Keyboard.current?.backquoteKey.wasPressedThisFrame == true)
        {
            GameLog.Info("GameManager", "Backtick pressed — triggering GameOver");
            SetState(GameState.GameOver);
        }
        if (Keyboard.current?.f5Key.wasPressedThisFrame == true)
        {
            GameLog.Info("GameManager", "F5 pressed — triggering Win");
            SetState(GameState.Win);
        }
#endif
    }
}
