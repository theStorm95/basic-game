using UnityEngine;

[CreateAssetMenu(fileName = "TowerDefinition", menuName = "Tower Defense/Tower Definition")]
public class TowerDefinitionSO : ScriptableObject
{
    [SerializeField] private TowerType _towerType;
    [SerializeField] private string _displayName;
    [Min(0)][SerializeField] private int _placementCost;
    [SerializeField] private Color _buttonColor = Color.white;

    public TowerType TowerType => _towerType;
    public string DisplayName => _displayName;
    public int PlacementCost => _placementCost;
    public Color ButtonColor => _buttonColor;
}
