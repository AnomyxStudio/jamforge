using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

namespace JamForge
{
    [Preserve]
    public class JamForgeEditor
    {
        [MenuItem("JamForge/Setup Config File %#k")]
        public static void SetupConfig()
        {
            var jamConfig = JamForgeConfig.Load();
            if (jamConfig != null)
            {
                Debug.Log("JamForgeConfig already exists.");
                return;
            }

            jamConfig = JamForgeConfig.Create();

            const string resourcesFolder = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(resourcesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            AssetDatabase.CreateAsset(jamConfig, $"{resourcesFolder}/JamForgeConfig.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("JamForgeConfig created.");
        }
    }
}
