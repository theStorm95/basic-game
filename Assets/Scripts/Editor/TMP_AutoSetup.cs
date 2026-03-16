using UnityEditor;
using TMPro.EditorUtilities;

[InitializeOnLoad]
public static class TMP_AutoSetup
{
    static TMP_AutoSetup()
    {
        // Import TMP essential resources silently if not already present
        string[] settings = AssetDatabase.FindAssets("t:TMP_Settings");
        if (settings.Length == 0)
        {
            string packagePath = TMP_EditorUtility.packageFullPath;
            AssetDatabase.ImportPackage(packagePath + "/Package Resources/TMP Essential Resources.unitypackage", false);
        }
    }
}
