using System;
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

        private static string _selectedPack = null;

        public static void Init(ManualLogSource logger)
        {
            Logger = logger;
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void StartPatch()
        {
            UpdateMaterials();
        }

        [HarmonyPatch(typeof(RoundManager), "GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        private static void GenerateNewLevelClientRpcPatch(int randomSeed)
        {
            UpdateMaterials();
        }

        private static void HideVanillaPosterPlane()
        {
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001 (Old)");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
                Logger.LogInfo("Vanilla poster plane (Plane.001 (Old)) hidden.");
                return;
            }

            posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
                Logger.LogInfo("Vanilla poster plane (Plane.001) hidden.");
            }
            else
            {
                Logger.LogWarning("Vanilla poster plane (Plane.001) not found!");
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

            var shipWindowsPlane = hangarShip.transform.Find("Plane.001")?.gameObject;
            if (shipWindowsPlane == null)
            {
                Logger.LogError("ShipWindows Plane.001 not found under HangarShip!");
                return false;
            }

            var originalRenderer = shipWindowsPlane.GetComponent<MeshRenderer>();
            if (originalRenderer == null || originalRenderer.materials.Length == 0)
            {
                Logger.LogError("ShipWindows Plane.001 renderer or materials not found!");
                return false;
            }

            var originalMaterial = originalRenderer.materials[0];

            var posterData = new (Vector3 position, Vector3 rotation, Vector3 scale, string name)[]
            {
                (new Vector3(4.1886f, 2.9318f, -16.8409f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1"),
                (new Vector3(6.4202f, 2.4776f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2"),
                (new Vector3(9.9186f, 2.8591f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3"),
                (new Vector3(5.2187f, 2.5963f, -11.0945f), new Vector3(0, 337.5868f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4"),
                (new Vector3(5.5286f, 2.5882f, -17.3541f), new Vector3(0, 201.1556f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5"),
                (new Vector3(3.0647f, 2.8174f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips")
            };

            if (Plugin.IsShipWindowsInstalled && Plugin.IsWindow2Enabled)
            {
                Logger.LogInfo("ShipWindows compatibility: Repositioning Poster4 due to window2 being enabled.");
                posterData[3].position = new Vector3(6.3616f, 3.3081f, -10.8221f);
                posterData[3].rotation = new Vector3(0, 0, 1.4166f);
                posterData[3].scale = new Vector3(0.7289f, 0.9989f, 1f);
            }

            var enabledPacks = Plugin.PosterFolders.Where(folder => PosterConfig.IsPackEnabled(folder)).ToList();
            if (enabledPacks.Count == 0)
            {
                Logger.LogWarning("No enabled packs found!");
                return false;
            }

            var enabledPackNames = enabledPacks.Select(pack => Path.GetFileName(Path.GetDirectoryName(pack))).ToList();
            Logger.LogInfo($"Enabled poster packs: {string.Join(", ", enabledPackNames)}");

            // Handle PosterRandomizer logic
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
                renderer.material = new Material(originalMaterial);

                if (allTextures.TryGetValue(poster.name, out var textures) && textures.Count > 0)
                {
                    var textureData = textures[Plugin.Rand.Next(textures.Count)];
                    renderer.material.mainTexture = textureData.texture;
                    anyTextureLoaded = true;

                    var modFolderName = Path.GetFileName(Path.GetDirectoryName(textureData.filePath));
                    var relativePath = poster.name == "CustomTips"
                        ? Path.Combine("tips", "CustomTips.png")
                        : Path.Combine("posters", poster.name + ".png");
                    var simplifiedPath = Path.Combine(modFolderName, relativePath);

                    Logger.LogInfo($"Loaded texture for {poster.name} from {simplifiedPath}");
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
                    var directoryName = Path.GetFileName(Path.GetDirectoryName(fullPath));
                    var fileName = Path.GetFileName(fullPath);
                    var simplifiedPath = Path.Combine(directoryName, fileName);
                    Logger.LogError($"Failed to load texture from {simplifiedPath}");
                    return (null, null);
                }
            }

            var errorDirectoryName = Path.GetFileName(Path.GetDirectoryName(fullPath));
            var errorFileName = Path.GetFileName(fullPath);
            var simplifiedErrorPath = Path.Combine(errorDirectoryName, errorFileName);
            Logger.LogError($"Texture file not found: {simplifiedErrorPath}");
            return (null, null);
        }

        private static void UpdateMaterials()
        {
            Logger.LogInfo("Updating materials for custom posters");

            if (Plugin.PosterFolders.Count == 0)
            {
                Logger.LogInfo("No CustomPosters folder found. Leaving Plane.001 active.");
                return;
            }

            HideVanillaPosterPlane();

            if (!CreateCustomPosters())
            {
                ShowVanillaPosterPlane();
            }
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
