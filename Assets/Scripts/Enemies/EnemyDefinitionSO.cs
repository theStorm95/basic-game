using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinition", menuName = "BasicTD/EnemyDefinition")]
public class EnemyDefinitionSO : ScriptableObject
{
    [field: SerializeField] public float BaseHealth { get; private set; } = 100f;
    [field: SerializeField] public float BaseSpeed  { get; private set; } = 3f;
}
