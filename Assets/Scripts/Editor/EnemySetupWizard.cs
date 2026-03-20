using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// One-shot setup wizard for Story 2-1: Enemy Spawn and Path Movement.
/// Run via: BasicTD > Setup > Story 2-1 Enemy System
/// Idempotent — safe to run multiple times.
/// </summary>
public static class EnemySetupWizard
{
    private const string EnemyDefinitionPath = "Assets/Data/Enemies/EnemyDefinition.asset";
    private const string EnemyPrefabPath     = "Assets/Prefabs/Enemies/Enemy.prefab";
    private const string EnemySpritePath     = "Assets/Art/Enemies/enemy-base.png";

    [MenuItem("BasicTD/Setup/Story 2-1 Enemy System")]
    public static void RunSetup()
    {
        bool anyErrors = false;

        // ── Step 1: EnemyDefinition ScriptableObject ──────────────────────────
        EnemyDefinitionSO definition = AssetDatabase.LoadAssetAtPath<EnemyDefinitionSO>(EnemyDefinitionPath);
        if (definition == null)
        {
            definition = ScriptableObject.CreateInstance<EnemyDefinitionSO>();
            AssetDatabase.CreateAsset(definition, EnemyDefinitionPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[EnemySetupWizard] Created EnemyDefinition.asset at " + EnemyDefinitionPath);
        }
        else
        {
            Debug.Log("[EnemySetupWizard] EnemyDefinition.asset already exists — skipping.");
        }

        // ── Step 2: Enemy Prefab ───────────────────────────────────────────────
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefabPath);
        if (existingPrefab == null)
        {
            // Load the sprite (Multiple sprite mode — load sub-asset)
            Sprite sprite = null;
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(EnemySpritePath);
            foreach (Object asset in allAssets)
            {
                if (asset is Sprite s) { sprite = s; break; }
            }
            if (sprite == null)
            {
                Debug.LogError("[EnemySetupWizard] Could not load sprite at " + EnemySpritePath +
                               ". Make sure enemy-base.png is imported as Sprite.");
                anyErrors = true;
            }
            else
            {
                // Build prefab in memory
                GameObject go = new GameObject("Enemy");
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                go.AddComponent<Enemy>();

                // Save as prefab
                GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(go, EnemyPrefabPath);
                Object.DestroyImmediate(go);

                if (prefabAsset != null)
                    Debug.Log("[EnemySetupWizard] Created Enemy.prefab at " + EnemyPrefabPath);
                else
                {
                    Debug.LogError("[EnemySetupWizard] Failed to create Enemy.prefab.");
                    anyErrors = true;
                }
            }
        }
        else
        {
            Debug.Log("[EnemySetupWizard] Enemy.prefab already exists — skipping.");
        }

        // Reload prefab reference (may have just been created)
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefabPath);

        // ── Step 3: Scene GameObjects ─────────────────────────────────────────
        // EnemyPool
        EnemyPool enemyPool = Object.FindFirstObjectByType<EnemyPool>();
        if (enemyPool == null)
        {
            GameObject poolGO = new GameObject("EnemyPool");
            enemyPool = poolGO.AddComponent<EnemyPool>();
            Debug.Log("[EnemySetupWizard] Created EnemyPool GameObject in scene.");
        }
        else
        {
            Debug.Log("[EnemySetupWizard] EnemyPool already exists in scene — skipping creation.");
        }

        // Wire prefab into EnemyPool
        if (enemyPrefab != null)
        {
            SerializedObject soPool = new SerializedObject(enemyPool);
            SerializedProperty prefabProp = soPool.FindProperty("_enemyPrefab");
            if (prefabProp != null && prefabProp.objectReferenceValue == null)
            {
                prefabProp.objectReferenceValue = enemyPrefab.GetComponent<Enemy>();
                soPool.ApplyModifiedProperties();
                Debug.Log("[EnemySetupWizard] Wired Enemy.prefab → EnemyPool._enemyPrefab.");
            }
        }

        // EnemyManager
        EnemyManager enemyManager = Object.FindFirstObjectByType<EnemyManager>();
        if (enemyManager == null)
        {
            GameObject managerGO = new GameObject("EnemyManager");
            enemyManager = managerGO.AddComponent<EnemyManager>();
            Debug.Log("[EnemySetupWizard] Created EnemyManager GameObject in scene.");
        }
        else
        {
            Debug.Log("[EnemySetupWizard] EnemyManager already exists in scene — skipping creation.");
        }

        // Wire definition into EnemyManager
        if (definition != null)
        {
            SerializedObject soManager = new SerializedObject(enemyManager);
            SerializedProperty defProp = soManager.FindProperty("_enemyDefinition");
            if (defProp != null && defProp.objectReferenceValue == null)
            {
                defProp.objectReferenceValue = definition;
                soManager.ApplyModifiedProperties();
                Debug.Log("[EnemySetupWizard] Wired EnemyDefinition.asset → EnemyManager._enemyDefinition.");
            }
        }

        // ── Save scene ────────────────────────────────────────────────────────
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (!anyErrors)
            Debug.Log("[EnemySetupWizard] ✅ Story 2-1 setup complete! Run Play Mode to verify.");
        else
            Debug.LogError("[EnemySetupWizard] ⚠️ Setup completed with errors — check log above.");
    }
}
