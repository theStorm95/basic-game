using System;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance { get; private set; }

    public bool IsInPlacementMode { get; private set; }
    public TowerType SelectedType { get; private set; }

    public event Action<TowerType> OnPlacementModeEntered;
    public event Action OnPlacementModeExited;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void EnterPlacementMode(TowerType type)
    {
        IsInPlacementMode = true;
        SelectedType = type;
        OnPlacementModeEntered?.Invoke(type);
        GameLog.Info("TowerPlacer", $"Entered placement mode: {type}");
    }

    public void ExitPlacementMode()
    {
        IsInPlacementMode = false;
        OnPlacementModeExited?.Invoke();
        GameLog.Info("TowerPlacer", "Exited placement mode");
    }
}
