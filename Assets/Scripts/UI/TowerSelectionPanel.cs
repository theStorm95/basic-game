using UnityEngine;
using UnityEngine.UI;

public class TowerSelectionPanel : MonoBehaviour
{
    [SerializeField] private TowerDefinitionSO[] _towerDefinitions;
    [SerializeField] private Button[] _towerButtons;
    [SerializeField] private Text[] _buttonLabels;

    private static readonly Color DeselectedColor = Color.white;

    private void Awake()
    {
        Debug.Assert(_towerDefinitions != null && _towerDefinitions.Length == 4,
            "[TowerSelectionPanel] _towerDefinitions must have exactly 4 entries");
        Debug.Assert(_towerButtons != null && _towerButtons.Length == 4,
            "[TowerSelectionPanel] _towerButtons must have exactly 4 entries");
        Debug.Assert(_buttonLabels != null && _buttonLabels.Length == 4,
            "[TowerSelectionPanel] _buttonLabels must have exactly 4 entries");
    }

    private void Start()
    {
        Debug.Assert(TowerPlacer.Instance != null, "[TowerSelectionPanel] TowerPlacer.Instance is null in Start");

        for (int i = 0; i < _towerButtons.Length; i++)
        {
            int index = i;
            TowerDefinitionSO def = _towerDefinitions[index];

            _buttonLabels[index].text = $"{def.DisplayName}\n{def.PlacementCost}g";

            _towerButtons[index].onClick.AddListener(() =>
                TowerPlacer.Instance.EnterPlacementMode(def.TowerType));
        }
    }

    private void OnEnable()
    {
        if (TowerPlacer.Instance == null) return;
        TowerPlacer.Instance.OnPlacementModeEntered += HandlePlacementModeEntered;
        TowerPlacer.Instance.OnPlacementModeExited += HandlePlacementModeExited;
    }

    private void OnDisable()
    {
        if (TowerPlacer.Instance == null) return;
        TowerPlacer.Instance.OnPlacementModeEntered -= HandlePlacementModeEntered;
        TowerPlacer.Instance.OnPlacementModeExited -= HandlePlacementModeExited;
    }

    private void HandlePlacementModeEntered(TowerType selectedType)
    {
        for (int i = 0; i < _towerDefinitions.Length; i++)
        {
            bool isSelected = _towerDefinitions[i].TowerType == selectedType;
            SetButtonHighlight(_towerButtons[i], isSelected ? _towerDefinitions[i].ButtonColor : DeselectedColor);
        }
    }

    private void HandlePlacementModeExited()
    {
        for (int i = 0; i < _towerButtons.Length; i++)
            SetButtonHighlight(_towerButtons[i], DeselectedColor);
    }

    private void SetButtonHighlight(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }
}
