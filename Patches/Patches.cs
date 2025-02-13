using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
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

        public static void Init(ManualLogSource logger)
        {
            Logger = logger;
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void StartPatch(StartOfRound __instance)
        {
            _materialsUpdated = false;

            CopyPlane001Material();

            __instance.StartCoroutine(DelayedUpdateMaterials());
        }

        [HarmonyPatch(typeof(RoundManager), "GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        private static void GenerateNewLevelClientRpcPatch(int randomSeed, StartOfRound __instance)
        {
            if (!_materialsUpdated)
            {
                __instance.StartCoroutine(DelayedUpdateMaterials());
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
        [HarmonyPostfix]
        private static void OnShipLandedMiscEventsPatch(StartOfRound __instance)
        {
            if (!_materialsUpdated)
            {
                __instance.StartCoroutine(DelayedUpdateMaterials());
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnClientDisconnect")]
        [HarmonyPostfix]
        private static void OnClientDisconnectPatch()
        {
            _materialsUpdated = false;
            Logger.LogInfo("Lobby left. Resetting materials update flag.");
        }

        private static void CopyPlane001Material()
        {
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane == null)
            {
                Logger.LogError("Poster plane (Plane.001) not found under HangarShip!");
                return;
            }

            var originalRenderer = posterPlane.GetComponent<MeshRenderer>();
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
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001 (Old)");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
                return;
            }

            posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
            }
        }

        private static bool CreateCustomPosters()
        {
            var environment = GameObject.Find("Environment");
            if (environment == null)
            {
                Logger.LogError("Environment GameObject not found in the scene hierarchy!");
                return false;
            }

            var hangarShip = environment.transform.Find("HangarShip")?.gameObject;
            if (hangarShip == null)
            {
                Logger.LogError("HangarShip GameObject not found under Environment!");
                return false;
            }

            var postersParent = new GameObject("CustomPosters");
            postersParent.transform.SetParent(hangarShip.transform);
            postersParent.transform.localPosition = Vector3.zero;

            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane == null)
            {
                Logger.LogError("Poster plane (Plane.001) not found under HangarShip!");
                return false;
            }

            var originalRenderer = posterPlane.GetComponent<MeshRenderer>();
            if (originalRenderer == null || originalRenderer.materials.Length == 0)
            {
                Logger.LogError("Poster plane renderer or materials not found!");
                return false;
            }

            var originalMaterial = originalRenderer.material;

            // default poster positions
            var posterData = new (Vector3 position, Vector3 rotation, Vector3 scale, string name)[]
            {
        (new Vector3(4.1886f, 2.9318f, -16.8409f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1"),
        (new Vector3(6.4202f, 2.4776f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2"),
        (new Vector3(9.9186f, 2.8591f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3"),
        (new Vector3(5.2187f, 2.5963f, -11.0945f), new Vector3(0, 337.5868f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4"),
        (new Vector3(5.5286f, 2.5882f, -17.3541f), new Vector3(0, 201.1556f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5"),
        (new Vector3(3.0647f, 2.8174f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips")
            };

            if (Plugin.IsWiderShipModInstalled)
            {
                Logger.LogInfo($"Adjusting poster positions for WiderShipMod Extended Side: {Plugin.WiderShipExtendedSide}");

                switch (Plugin.WiderShipExtendedSide)
                {
                    case "Both":
                        // reposition posters for Both extended side
                        posterData[0] = (new Vector3(4.6877f, 2.9407f, -19.62f), new Vector3(0, 118.2274f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1"); // Poster1
                        posterData[3] = (new Vector3(8.4715f, 2.8652f, -10.826f), new Vector3(0, 0.026f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4"); // Poster4
                        posterData[4] = (new Vector3(5.3602f, 2.5882f, -18.3793f), new Vector3(0, 118.0114f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5"); // Poster5
                        posterData[5] = (new Vector3(3.0947f, 2.8174f, -6.7253f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips"); // CustomTips
                        break;

                    case "Right":
                        // adjust positions for Right
                        posterData[0] = (new Vector3(4.2224f, 2.9318f, -16.8609f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1"); // Poster1
                        posterData[1] = (new Vector3(6.4202f, 2.4776f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2"); // Poster2
                        posterData[2] = (new Vector3(9.9426f, 2.8591f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3"); // Poster3
                        posterData[3] = (new Vector3(7.3425f, 2.9452f, -10.826f), new Vector3(0, 0.026f, 2.6456f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4"); // Poster4
                        posterData[4] = (new Vector3(5.5386f, 2.5882f, -17.3641f), new Vector3(0, 200.9099f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5"); // Poster5
                        posterData[5] = (new Vector3(3.0947f, 2.8174f, -6.733f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips"); // CustomTips
                        break;

                    case "Left":
                        // adjust positions for Left
                        posterData[0] = (new Vector3(4.6777f, 2.9007f, -19.63f), new Vector3(0, 118.2274f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1"); // Poster1
                        posterData[2] = (new Vector3(9.7197f, 2.8151f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3"); // Poster3
                        posterData[3] = (new Vector3(7.3025f, 2.8652f, -10.826f), new Vector3(0, 0.026f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4"); // Poster4
                        posterData[4] = (new Vector3(5.3602f, 2.5482f, -18.3793f), new Vector3(0, 118.0114f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5"); // Poster5
                        posterData[5] = (new Vector3(2.8647f, 2.7774f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips"); // CustomTips
                        break;
                }
            }

            // reposition Poster4 if ShipWindows is installed and window2 is enabled but only if WiderShipMod is not installed
            if (Plugin.IsShipWindowsInstalled && Plugin.IsWindow2Enabled && !Plugin.IsWiderShipModInstalled)
            {
                Logger.LogInfo("ShipWindows compatibility: Repositioning Poster4 due to window2 being enabled.");
                posterData[3].position = new Vector3(6.3616f, 3.3081f, -10.8221f); // New position
                posterData[3].rotation = new Vector3(0, 0, 1.4166f); // New rotation
                posterData[3].scale = new Vector3(0.7289f, 0.9989f, 1f); // New scale
            }

            var enabledPacks = Plugin.PosterFolders.Where(folder => PosterConfig.IsPackEnabled(folder)).ToList();
            if (enabledPacks.Count == 0)
            {
                Logger.LogWarning("No enabled packs found!");
                return false;
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
                Logger.LogInfo("PosterRandomizer disabled. Combining all enabled packs.");
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
                        var result = LoadTextureFromFile(file);
                        if (result.texture != null)
                        {
                            var posterName = Path.GetFileNameWithoutExtension(file);
                            if (!allTextures.ContainsKey(posterName))
                            {
                                allTextures[posterName] = new List<(Texture2D texture, string filePath)>();
                            }
                            allTextures[posterName].Add(result);
                        }
                    }
                }

                if (Directory.Exists(tipsPath))
                {
                    foreach (var file in Directory.GetFiles(tipsPath, "*.png"))
                    {
                        var result = LoadTextureFromFile(file);
                        if (result.texture != null)
                        {
                            var posterName = Path.GetFileNameWithoutExtension(file);
                            if (!allTextures.ContainsKey(posterName))
                            {
                                allTextures[posterName] = new List<(Texture2D texture, string filePath)>();
                            }
                            allTextures[posterName].Add(result);
                        }
                    }
                }
            }

            if (allTextures.Count == 0)
            {
                Logger.LogWarning("No textures found in enabled packs!");
                return false;
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
                }
                else
                {
                    Logger.LogError($"No textures found for {poster.name}. Disabling the poster.");
                    poster.SetActive(false);
                }
            }

            if (!anyTextureLoaded)
            {
                UnityEngine.Object.Destroy(postersParent);
                Logger.LogWarning("No custom posters were created due to missing textures.");
                return false;
            }

            var vanillaPlane = hangarShip.transform.Find("Plane.001")?.gameObject;
            if (vanillaPlane != null)
            {
                Logger.LogInfo("Destroying vanilla Plane.001.");
                UnityEngine.Object.Destroy(vanillaPlane);
            }

            Logger.LogInfo("Custom posters created successfully.");
            return true;
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            if (obj == null) return "null";

            string path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }

        private static (Texture2D texture, string filePath) LoadTextureFromFile(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                var texture = new Texture2D(2, 2);
                if (texture.LoadImage(File.ReadAllBytes(fullPath)))
                {
                    texture.filterMode = FilterMode.Point;
                    return (texture, fullPath);
                }
                else
                {
                    Logger.LogError($"Failed to load texture from {fullPath}");
                    return (null, null);
                }
            }

            Logger.LogError($"Texture file not found: {fullPath}");
            return (null, null);
        }

        private static IEnumerator DelayedUpdateMaterials()
        {
            if (_materialsUpdated)
            {
                yield break;
            }

            yield return new WaitForSeconds(1);

            Logger.LogInfo("Updating materials for custom posters");

            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
            }

            HideVanillaPosterPlane();

            if (!CreateCustomPosters())
            {
                Logger.LogWarning("Failed to create custom posters. Re-enabling Plane.001.");
                ShowVanillaPosterPlane();
            }

            _materialsUpdated = true;
        }

        private static void ShowVanillaPosterPlane()
        {
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001 (Old)");
            if (posterPlane != null)
            {
                posterPlane.SetActive(true);
                Logger.LogInfo("Vanilla poster plane (Plane.001 (Old)) re-enabled.");
                return;
            }

            posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane != null)
            {
                posterPlane.SetActive(true);
                Logger.LogInfo("Vanilla poster plane (Plane.001) re-enabled.");
            }
            else
            {
                Logger.LogWarning("Vanilla poster plane (Plane.001) not found!");
            }
        }
    }
}
