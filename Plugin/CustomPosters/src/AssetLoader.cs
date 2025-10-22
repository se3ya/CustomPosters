using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomPosters
{
    internal static class AssetManager
    {
        public static AssetBundle? Bundle { get; private set; }
        public static GameObject? PosterPrefab { get; private set; }
        public static GameObject? Poster5Prefab { get; private set; }
        public static GameObject? TipsPrefab { get; private set; }

        public static void LoadAssets()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assetDir = Path.GetDirectoryName(assembly.Location);
            var bundlePath = Path.Combine(assetDir, "customposters");

            Bundle = AssetBundle.LoadFromFile(bundlePath);

            if (Bundle == null)
            {
                Plugin.Log.LogError("Failed to load AssetBundle.");
                return;
            }

            PosterPrefab = Bundle.LoadAsset<GameObject>("CustomPosterPrefab");
            Poster5Prefab = Bundle.LoadAsset<GameObject>("Poster5");
            TipsPrefab = Bundle.LoadAsset<GameObject>("Tips");

            if (PosterPrefab == null)
            {
                Plugin.Log.LogError("Failed to load prefab from the AssetBundle.");
            }
            else
            {
                Plugin.Log.LogInfo("Loaded assetbundle.");
            }

            if (Poster5Prefab == null)
            {
                Plugin.Log.LogError("Failed to load Poster5 prefab from the AssetBundle.");
            }

            if (TipsPrefab == null)
            {
                Plugin.Log.LogError("Failed to load Tips prefab from the AssetBundle.");
            }
        }
    }
}