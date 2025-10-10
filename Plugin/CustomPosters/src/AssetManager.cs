using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomPosters
{
    internal static class AssetManager
    {
        public static AssetBundle? Bundle { get; private set; }
        public static GameObject? PosterPrefab { get; private set; }

        public static void LoadAssets()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assetDir = Path.GetDirectoryName(assembly.Location);

            Bundle = AssetBundle.LoadFromFile(Path.Combine(assetDir, "customposters"));

            if (Bundle == null)
            {
                Plugin.Log.LogError("Failed to load AssetBundle.");
                return;
            }

            PosterPrefab = Bundle.LoadAsset<GameObject>("CustomPosterPrefab");
            
            if (PosterPrefab == null)
            {
                Plugin.Log.LogError("Failed to load prefab from the AssetBundle.");
            }
            else
            {
                Plugin.Log.LogInfo("Loaded PosterPrefab.");
            }
        }
    }
}