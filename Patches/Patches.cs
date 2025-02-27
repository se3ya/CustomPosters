using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using BepInEx;
using BepInEx.Logging;
using DunGen;
using HarmonyLib;
using UnityEngine;

namespace CustomPosters
{
    internal class Patches
    {
        private static ManualLogSource Logger { get; set; }
        private static bool _materialsUpdated = false;
        private static string _selectedPack = null;
        private static Material _copiedMaterial = null;
        private static readonly List<GameObject> CreatedPosters = new();
        private static readonly Dictionary<string, Texture2D> LoadedTextures = new();
        private static AssetBundle _lethalPostersBundle;
        private static GameObject _poster5Prefab;
        private static GameObject _tipsPrefab;

        public static void Init(ManualLogSource logger)
        {
            Logger = logger;
            LoadAssetBundles();
        }

        private static void LoadAssetBundles()
        {
            try
            {
                string bundlesPath = Path.Combine(Paths.PluginPath, "CustomPosters", "bundles");
                string bundleFile = Path.Combine(bundlesPath, "lethalposters");
                
                if (!Directory.Exists(bundlesPath))
                {
                    Logger.LogError($"Bundles directory not found at: {bundlesPath}");
                    return;
                }

                if (!File.Exists(bundleFile))
                {
                    Logger.LogError($"Bundle file not found at: {bundleFile}");
                    return;
                }

                _lethalPostersBundle = AssetBundle.LoadFromFile(bundleFile);

                if (_lethalPostersBundle != null)
                {
                    Logger.LogInfo("Bundle loaded successfully");
                    
                    _poster5Prefab = _lethalPostersBundle.LoadAsset<GameObject>("Poster5Prefab");
                    _tipsPrefab = _lethalPostersBundle.LoadAsset<GameObject>("PosterTipsPrefab");
                    
                    if (_poster5Prefab == null) Logger.LogError("Failed to load Poster5Prefab");
                    if (_tipsPrefab == null) Logger.LogError("Failed to load PosterTips");
                }
                else
                {
                    Logger.LogError($"Failed to load bundle from: {bundleFile}");
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to load asset bundle: {e.Message}\nStack trace: {e.StackTrace}");
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void StartPatch(StartOfRound __instance)
        {
            _materialsUpdated = false;
            CopyPlane001Material();
            __instance.StartCoroutine(DelayedUpdateMaterialsAsync());
        }

        [HarmonyPatch(typeof(RoundManager), "GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        private static void GenerateNewLevelClientRpcPatch(int randomSeed, StartOfRound __instance)
        {
            if (!_materialsUpdated)
            {
                __instance.StartCoroutine(DelayedUpdateMaterialsAsync());
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
        [HarmonyPostfix]
        private static void OnShipLandedMiscEventsPatch(StartOfRound __instance)
        {
            if (!_materialsUpdated)
            {
                __instance.StartCoroutine(DelayedUpdateMaterialsAsync());
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnClientDisconnect")]
        [HarmonyPostfix]
        private static void OnClientDisconnectPatch(ulong clientId)
        {
            _materialsUpdated = false;
            Logger.LogInfo("Lobby left. Resetting materials.");
            CleanUpPosters();
        }

        private static IEnumerator LoadTextureAsync(string filePath, Action<Texture2D> onComplete)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Logger.LogError($"Texture file not found: {filePath}");
                    onComplete?.Invoke(null);
                    yield break;
                }

                var texture = new Texture2D(2, 2);
                var fileData = File.ReadAllBytes(filePath);

                if (texture.LoadImage(fileData))
                {
                    texture.filterMode = FilterMode.Point;
                    onComplete?.Invoke(texture);
                }
                else
                {
                    Logger.LogError($"Failed to load texture from {filePath}");
                    onComplete?.Invoke(null);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading texture from {filePath}: {ex.Message}");
                onComplete?.Invoke(null);
            }
        }

        private static void CopyPlane001Material()
        {
            var vanillaPlaneMaterial = GameObject.Find("Environment/HangarShip/Plane.001");
            if (vanillaPlaneMaterial == null)
            {
                Logger.LogError("Poster plane (Plane.001) not found under HangarShip!");
                return;
            }

            var originalRenderer = vanillaPlaneMaterial.GetComponent<MeshRenderer>();
            if (originalRenderer == null || originalRenderer.materials.Length == 0)
            {
                Logger.LogError("Poster plane renderer or materials not found!");
                return;
            }

            _copiedMaterial = new Material(originalRenderer.material);
            Logger.LogInfo("Copied material from Plane.001.");
        }

        private static void HideVanillaPosterPlane()
        {
            var vanillaPosterPlane = GameObject.Find("Environment/HangarShip/Plane.001 (Old)");
            if (vanillaPosterPlane != null)
            {
                vanillaPosterPlane.SetActive(false);
                return;
            }

            vanillaPosterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (vanillaPosterPlane != null)
            {
                vanillaPosterPlane.SetActive(false);
            }
        }

        private static void CleanUpPosters()
        {
            if (CreatedPosters.Count == 0) return;

            Logger.LogInfo("Cleaning up existing posters.");
            foreach (var poster in CreatedPosters)
            {
                if (poster != null)
                {
                    UnityEngine.Object.Destroy(poster);
                }
            }
            CreatedPosters.Clear();
            LoadedTextures.Clear();
        }

        private static IEnumerator CreateCustomPostersAsync()
        {
            CleanUpPosters();
            LoadedTextures.Clear();

            var environment = GameObject.Find("Environment");
            if (environment == null)
            {
                Logger.LogError("Environment GameObject not found in the scene hierarchy!");
                yield break;
            }

            var hangarShip = environment.transform.Find("HangarShip")?.gameObject;
            if (hangarShip == null)
            {
                Logger.LogError("HangarShip GameObject not found under Environment!");
                yield break;
            }

            var postersParent = new GameObject("CustomPosters");
            postersParent.transform.SetParent(hangarShip.transform);
            postersParent.transform.localPosition = Vector3.zero;

            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane == null)
            {
                Logger.LogError("Poster plane (Plane.001) not found under HangarShip!");
                yield break;
            }

            var originalRenderer = posterPlane.GetComponent<MeshRenderer>();
            if (originalRenderer == null || originalRenderer.materials.Length == 0)
            {
                Logger.LogError("Poster plane renderer or materials not found!");
                yield break;
            }

            var originalMaterial = originalRenderer.material;

            // Default poster positions
            var posterData = new (Vector3 position, Vector3 rotation, Vector3 scale, string name)[]
            {
        (new Vector3(4.1886f, 2.9318f, -16.8409f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1"),
        (new Vector3(6.4202f, 2.4776f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2"),
        (new Vector3(9.9186f, 2.8591f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3"),
        (new Vector3(5.2187f, 2.5963f, -11.0945f), new Vector3(0, 337.5868f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4"),
        (new Vector3(5.5386f, 2.5882f, -17.3341f), new Vector3(0, 358.9184f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5"),
        (new Vector3(2.8647f, 2.7774f, -11.7541f), new Vector3(359.2899f, 89.687f, 269.7103f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips")
            };

            // Adjust poster positions based on WiderShipMod's Extended Side
            if (Plugin.IsWiderShipModInstalled)
            {
                Logger.LogInfo($"Adjusting poster positions for WiderShipMod Extended Side: {Plugin.WiderShipExtendedSide}");

                switch (Plugin.WiderShipExtendedSide)
                {
                    case "Both":
                        posterData[0] = (new Vector3(4.6877f, 2.9407f, -19.62f), new Vector3(0, 118.2274f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                        posterData[3] = (new Vector3(5.5699f, 2.5963f, -10.3268f), new Vector3(0, 62.0324f, 2.6799f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                        posterData[4] = (new Vector3(5.3302f, 2.5882f, -18.3793f), new Vector3(0, 277.3203f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                        posterData[5] = (new Vector3(3.0947f, 2.8174f, -6.7583f), new Vector3(359.2899f, 89.687f, 269.7103f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        break;

                    case "Right":
                        posterData[0] = (new Vector3(4.2224f, 2.9318f, -16.8609f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                        posterData[1] = (new Vector3(6.4202f, 2.4776f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                        posterData[2] = (new Vector3(9.9426f, 2.8591f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                        posterData[3] = (new Vector3(5.5699f, 2.5963f, -10.3268f), new Vector3(0, 62.0324f, 2.6799f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                        posterData[4] = (new Vector3(5.5386f, 2.5882f, -17.3341f), new Vector3(0, 358.9184f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                        posterData[5] = (new Vector3(3.0947f, 2.8174f, -6.7583f), new Vector3(359.2899f, 89.687f, 269.7103f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        break;

                    case "Left":
                        posterData[0] = (new Vector3(4.6777f, 2.9007f, -19.63f), new Vector3(0, 118.2274f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                        posterData[1] = (new Vector3(6.4202f, 2.2577f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster2");
                        posterData[2] = (new Vector3(9.7197f, 2.8151f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                        posterData[3] = (new Vector3(6.4449f, 3.0961f, -10.8221f), new Vector3(0, 0.026f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                        posterData[4] = (new Vector3(5.3302f, 2.5482f, -18.3793f), new Vector3(0, 276.3203f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                        posterData[5] = (new Vector3(2.8647f, 2.7774f, -11.7541f), new Vector3(359.2899f, 89.687f, 269.7103f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        break;
                }
            }

            // Reposition posters based on 2 Story Ship Mod compatibility
            if (Plugin.Is2StoryShipModInstalled)
            {
                if (Plugin.IsShipWindowsInstalled)
                {
                    // If ShipWindows and 2 Story Mod are detected, ignore both configs and use specific positions
                    Logger.LogInfo("ShipWindows and 2 Story Ship Mod detected. Ignoring both configs and repositioning posters.");
                    posterData[0] = (new Vector3(6.5923f, 2.9318f, -17.4766f), new Vector3(0, 179.2201f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                    posterData[1] = (new Vector3(9.0884f, 2.4776f, -8.8229f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                    posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                    posterData[4] = (new Vector3(10.2813f, 2.7482f, -8.8271f), new Vector3(0, 0.9014f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                    posterData[5] = (new Vector3(2.5679f, 2.6763f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                }
                if (Plugin.IsShipWindowsInstalled && Plugin.IsWiderShipModInstalled)
                {
                    // If ShipWindows and WiderShipMod are detected, ignore 2 Story Mod config
                    Logger.LogInfo("ShipWindows and WiderShipMod detected with 2 Story Ship Mod. Using default 2 Story Ship Mod positions.");
                    posterData[0] = (new Vector3(6.5923f, 2.9318f, -22.4766f), new Vector3(0, 179.2201f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                    posterData[1] = (new Vector3(9.0884f, 2.4776f, -5.8265f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                    posterData[2] = (new Vector3(10.1364f, 2.8591f, -22.4788f), new Vector3(0, 180.3376f, 0), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                    posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                    posterData[4] = (new Vector3(7.8577f, 2.7482f, -22.4503f), new Vector3(0, 337.7817f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                    posterData[5] = (new Vector3(-5.7911f, 2.541f, -17.597f), new Vector3(359.2899f, 0.3053f, 269.7103f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                }
                else if (Plugin.IsWiderShipModInstalled)
                {
                    // If WiderShipMod is detected with 2 Story Mod, use specific positions
                    Logger.LogInfo("WiderShipMod detected with 2 Story Ship Mod. Using WiderShipMod-compatible positions.");
                    posterData[0] = (new Vector3(6.3172f, 2.9407f, -22.4766f), new Vector3(0, 180f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                    posterData[1] = (new Vector3(9.5975f, 2.5063f, -5.8245f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                    posterData[2] = (new Vector3(10.1364f, 2.8591f, -22.4788f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                    posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                    posterData[4] = (new Vector3(7.5475f, 2.5882f, -22.4803f), new Vector3(0, 180f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                    posterData[5] = (new Vector3(-5.8111f, 2.541f, -17.577f), new Vector3(0, 270.0942f, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                }
                else
                {
                    // If only 2 Story Mod is detected, use its config
                    Logger.LogInfo("2 Story Ship Mod detected. Repositioning posters based on window settings.");

                    // If all windows are enabled (default behavior)
                    if (Plugin.EnableRightWindows && Plugin.EnableLeftWindows)
                    {
                        Logger.LogInfo("All windows are enabled. Repositioning posters to default 2 Story Ship Mod positions.");
                        posterData[0] = (new Vector3(10.1567f, 2.75f, -8.8293f), new Vector3(0, 0, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                        posterData[1] = (new Vector3(9.0884f, 2.4776f, -8.8229f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                        posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                        posterData[4] = (new Vector3(6.1473f, 2.8195f, -17.4529f), new Vector3(0, 338.821f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                        posterData[5] = (new Vector3(2.5679f, 2.6763f, -11.7541f), new Vector3(359.2899f, 89.687f, 269.7103f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                    }
                    else
                    {
                        // Reposition posters if right windows are disabled
                        if (!Plugin.EnableRightWindows)
                        {
                            Logger.LogInfo("Right windows are disabled. Repositioning posters.");
                            posterData[0] = (new Vector3(4.0286f, 2.9318f, -16.7774f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                            posterData[1] = (new Vector3(9.0884f, 2.4776f, -8.8229f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                            posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 0), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                            posterData[4] = (new Vector3(5.3282f, 2.7482f, -17.2754f), new Vector3(0, 202.3357f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                            posterData[5] = (new Vector3(2.5679f, 2.6763f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        }

                        // Reposition posters if left windows are disabled
                        if (!Plugin.EnableLeftWindows)
                        {
                            Logger.LogInfo("Left windows are disabled. Repositioning posters.");
                            posterData[0] = (new Vector3(9.8324f, 2.9318f, -8.8257f), new Vector3(0, 0, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                            posterData[1] = (new Vector3(7.3648f, 2.4776f, -8.8229f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                            posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                            posterData[4] = (new Vector3(6.1473f, 2.8195f, -17.4729f), new Vector3(0, 179.7123f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                            posterData[5] = (new Vector3(2.5679f, 2.6763f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        }
                    }
                }
            }

            // reposition Poster4 and Poster2 if ShipWindows is installed and window2 is enabled but only if WiderShipMod is not installed
            if (Plugin.IsShipWindowsInstalled && Plugin.IsWindow2Enabled && !Plugin.IsWiderShipModInstalled && !Plugin.Is2StoryShipModInstalled)
            {
                Logger.LogInfo("ShipWindows/Beta compatibility: Repositioning Poster4 & Poster2 due to window 2 being enabled.");
                posterData[3].position = new Vector3(6.4449f, 3.0961f, -10.8221f); // New position
                posterData[1].position = new Vector3(6.4202f, 2.2577f, -10.8226f); // New position
                posterData[5].position = new Vector3(2.5679f, 2.6763f, -11.7541f); // New position
                posterData[5].rotation = new Vector3(359.2899f, 89.687f, 269.7103f); // New rotation
                posterData[4].position = new Vector3(5.5286f, 2.5882f, -17.3341f); // New position
                posterData[4].rotation = new Vector3(0, 359.3425f, 359.8f); // New rotation
                posterData[3].rotation = new Vector3(0, 0, 358.0874f); // New rotation
                posterData[3].scale = new Vector3(0.7289f, 0.9989f, 1f); // New scale
            }

            var enabledPacks = Plugin.PosterFolders.Where(folder => PosterConfig.IsPackEnabled(folder)).ToList();
            var vanillaPosterPlane = GameObject.Find("Environment/HangarShip/Plane.001");

            if (enabledPacks.Count == 0)
            {
                Logger.LogWarning("No enabled packs found!");
                // show vanilla poster if no packs are enabled
                if (vanillaPosterPlane != null)
                {
                    Logger.LogInfo("No packs enabled, showing vanilla Plane.001");
                    vanillaPosterPlane.SetActive(true);
                }
                yield break;
            }

            var enabledPackNames = enabledPacks.Select(pack => Path.GetFileName(Path.GetDirectoryName(pack))).ToList();
            Logger.LogInfo($"Enabled poster packs: {string.Join(", ", enabledPackNames)}");

            List<string> packsToUse;
            if (PosterConfig.PosterRandomizer.Value)
            {
                _selectedPack = enabledPacks[Plugin.Rand.Next(enabledPacks.Count)];
                packsToUse = new List<string> { _selectedPack };
                var selectedPackName = Path.GetFileName(Path.GetDirectoryName(_selectedPack));
                Logger.LogInfo($"PosterRandomizer enabled. Using pack: {selectedPackName}");
            }
            else
            {
                packsToUse = enabledPacks;
                Logger.LogInfo("Using all enabled packs");
            }

            // first hide Plane.001 before attempting to load textures
            if (vanillaPosterPlane != null)
            {
                vanillaPosterPlane.SetActive(false);
            }

            var allTextures = new Dictionary<string, List<(Texture2D texture, string filePath)>>();
            foreach (var pack in packsToUse)
            {
                string postersPath = Path.Combine(pack, "posters");
                string tipsPath = Path.Combine(pack, "tips");

                if (Directory.Exists(postersPath))
                {
                    foreach (var file in Directory.GetFiles(postersPath, "*.png"))
                    {
                        yield return LoadTextureAsync(file, (texture) =>
                        {
                            if (texture != null)
                            {
                                var posterName = Path.GetFileNameWithoutExtension(file);
                                if (!allTextures.ContainsKey(posterName))
                                {
                                    allTextures[posterName] = new List<(Texture2D texture, string filePath)>();
                                }
                                allTextures[posterName].Add((texture, file));
                                if (!LoadedTextures.ContainsKey(posterName))
                                {
                                    LoadedTextures[posterName] = texture;
                                }
                            }
                        });
                    }
                }

                if (Directory.Exists(tipsPath))
                {
                    foreach (var file in Directory.GetFiles(tipsPath, "*.png"))
                    {
                        yield return LoadTextureAsync(file, (texture) =>
                        {
                            if (texture != null)
                            {
                                var posterName = Path.GetFileNameWithoutExtension(file);
                                if (!allTextures.ContainsKey(posterName))
                                {
                                    allTextures[posterName] = new List<(Texture2D texture, string filePath)>();
                                }
                                allTextures[posterName].Add((texture, file));
                                if (!LoadedTextures.ContainsKey(posterName))
                                {
                                    LoadedTextures[posterName] = texture;
                                }
                            }
                        });
                    }
                }
            }

            if (allTextures.Count == 0)
            {
                Logger.LogWarning("No textures found in enabled packs!");
                if (vanillaPosterPlane != null)
                {
                    Logger.LogInfo("Showing vanilla Plane.001 due to missing textures");
                    vanillaPosterPlane.SetActive(true);
                }
                yield break;
            }

            bool anyTextureLoaded = false;

            for (int i = 0; i < posterData.Length; i++)
            {
                var poster = GameObject.CreatePrimitive(PrimitiveType.Quad);
                poster.name = posterData[i].name;
                poster.transform.SetParent(postersParent.transform);

                poster.transform.position = posterData[i].position;
                poster.transform.rotation = Quaternion.Euler(posterData[i].rotation);
                poster.transform.localScale = posterData[i].scale;

                var renderer = poster.GetComponent<MeshRenderer>();

                if (allTextures.TryGetValue(poster.name, out var textures) && textures.Count > 0)
                {
                    var textureData = textures[Plugin.Rand.Next(textures.Count)];

                    var material = new Material(_copiedMaterial);
                    material.mainTexture = textureData.texture;

                    renderer.material = material;
                    anyTextureLoaded = true;

                    Logger.LogInfo($"Loaded texture for {poster.name} from {textureData.filePath}");

                    CreatedPosters.Add(poster);
                }
                else
                {
                    Logger.LogError($"No textures found for {poster.name}. Disabling the poster.");
                    poster.SetActive(false);
                }

                yield return null;
            }

            if (!anyTextureLoaded)
            {
                UnityEngine.Object.Destroy(postersParent);
                Logger.LogWarning("No custom posters were created due to missing textures.");
                if (vanillaPosterPlane != null)
                {
                    Logger.LogInfo("Showing vanilla Plane.001 due to missing textures");
                    vanillaPosterPlane.SetActive(true);
                }
                yield break;
            }

            // if we got here, textures loaded successfully, destroy Plane.001
            if (vanillaPosterPlane != null)
            {
                Logger.LogInfo("Textures loaded successfully, destroying vanilla Plane.001");
                UnityEngine.Object.Destroy(vanillaPosterPlane);
            }

            Logger.LogInfo("Custom posters created successfully.");
        }

        private static IEnumerator DelayedUpdateMaterialsAsync()
        {
            if (_materialsUpdated)
            {
                yield break;
            }

            Logger.LogInfo("Updating materials for custom posters");

            // Find and hide the vanilla plane
            var vanillaPlaneMaterials = GameObject.Find("Environment/HangarShip/Plane.001");
            if (vanillaPlaneMaterials != null)
            {
                vanillaPlaneMaterials.SetActive(false);
            }

            HideVanillaPosterPlane();

            yield return CreateCustomPostersAsync();

            ReplacePosters();

            _materialsUpdated = true;
        }

        private static void ReplacePosters()
        {
            try
            {
                if (_copiedMaterial == null)
                {
                    Logger.LogError("Copied material is null! Make sure CopyPlane001Material was called.");
                    return;
                }

                GameObject poster5 = GameObject.Find("Poster5");
                if (poster5 != null && _poster5Prefab != null)
                {
                    Vector3 originalPosition = poster5.transform.position;
                    Quaternion originalRotation = poster5.transform.rotation;
                    Transform originalParent = poster5.transform.parent;

                    GameObject.Destroy(poster5);

                    GameObject newPoster = GameObject.Instantiate(_poster5Prefab, originalPosition, originalRotation);
                    newPoster.transform.parent = originalParent;
                    
                    var meshColliders = newPoster.GetComponentsInChildren<MeshCollider>();
                    foreach (var collider in meshColliders)
                    {
                        GameObject.Destroy(collider);
                    }
                    
                    var renderer = newPoster.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        Material newMaterial = new Material(_copiedMaterial);
                        
                        if (LoadedTextures.TryGetValue("Poster5", out var texture))
                        {
                            newMaterial.mainTexture = texture;
                            Logger.LogInfo("Applied previously loaded Poster5 texture");
                        }
                        else
                        {
                            Logger.LogError("Poster5 texture not found in loaded textures!");
                        }
                        
                        renderer.material = newMaterial;
                    }
                    
                    CreatedPosters.Add(newPoster);
                    Logger.LogInfo("Successfully replaced Poster5 at original position");
                }

                GameObject tips = GameObject.Find("CustomTips");
                if (tips != null && _tipsPrefab != null)
                {
                    Vector3 originalPosition = tips.transform.position;
                    Quaternion originalRotation = tips.transform.rotation;
                    Transform originalParent = tips.transform.parent;

                    GameObject.Destroy(tips);

                    GameObject newPoster = GameObject.Instantiate(_tipsPrefab, originalPosition, originalRotation);
                    newPoster.transform.parent = originalParent;
                    
                    var meshColliders = newPoster.GetComponentsInChildren<MeshCollider>();
                    foreach (var collider in meshColliders)
                    {
                        GameObject.Destroy(collider);
                    }
                    
                    var renderer = newPoster.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        Material newMaterial = new Material(_copiedMaterial);
                        
                        if (LoadedTextures.TryGetValue("CustomTips", out var texture))
                        {
                            newMaterial.mainTexture = texture;
                            Logger.LogInfo("Applied previously loaded CustomTips texture");
                        }
                        else
                        {
                            Logger.LogError("CustomTips texture not found in loaded textures!");
                        }
                        
                        renderer.material = newMaterial;
                    }
                    
                    CreatedPosters.Add(newPoster);
                    Logger.LogInfo("Successfully replaced CustomTips at original position");
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to replace posters: {e.Message}\nStack trace: {e.StackTrace}");
            }
        }
    }
}
