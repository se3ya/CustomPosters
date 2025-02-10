using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace CustomPosters
{
    internal class Patches
    {
        private static ManualLogSource Logger { get; set; }

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
            // Check for the renamed Plane.001 (Old) first
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001 (Old)");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
                Logger.LogInfo("Vanilla poster plane (Plane.001 (Old)) hidden.");
                return;
            }

            // Fallback to the original Plane.001
            posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
                Logger.LogInfo("Vanilla poster plane (Plane.001) destroyed.");
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

            // Find and destroy the vanilla Plane.001
            var vanillaPlane = hangarShip.transform.Find("Plane.001")?.gameObject;
            if (vanillaPlane != null)
            {
                Logger.LogInfo("Destroying vanilla Plane.001.");
                UnityEngine.Object.Destroy(vanillaPlane);
            }
            else
            {
                Logger.LogWarning("Vanilla Plane.001 not found under HangarShip!");
            }

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

            // Reposition Poster4 if ShipWindows is installed and window2 is enabled
            if (Plugin.IsShipWindowsInstalled && Plugin.IsWindow2Enabled)
            {
                Logger.LogInfo("ShipWindows compatibility: Repositioning Poster4 due to window2 being enabled.");
                posterData[3].position = new Vector3(6.3616f, 3.3081f, -10.8221f);
                posterData[3].rotation = new Vector3(0, 0, 1.4166f);
                posterData[3].scale = new Vector3(0.7289f, 0.9989f, 1f);
            }

            bool allTexturesLoaded = true;

            // Get all enabled packs
            var enabledPacks = Plugin.PosterFolders.Where(folder => PosterConfig.IsPackEnabled(folder)).ToList();
            if (enabledPacks.Count == 0)
            {
                Logger.LogWarning("No enabled packs found!");
                return false;
            }

            // Randomly select a pack if PosterRandomizer is true
            string selectedPack = null;
            if (PosterConfig.PosterRandomizer.Value)
            {
                selectedPack = enabledPacks[Plugin.Rand.Next(enabledPacks.Count)];
                Logger.LogInfo($"Selected pack: {selectedPack}");
            }

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

                string texturePath;

                if (PosterConfig.PosterRandomizer.Value)
                {
                    // Use the selected pack for all posters
                    texturePath = poster.name == "CustomTips"
                        ? Path.Combine(selectedPack, "tips", "CustomTips.png")
                        : Path.Combine(selectedPack, "posters", $"{poster.name}.png");
                }
                else
                {
                    // Randomly choose a pack for each poster
                    var randomPack = enabledPacks[Plugin.Rand.Next(enabledPacks.Count)];
                    texturePath = poster.name == "CustomTips"
                        ? Path.Combine(randomPack, "tips", "CustomTips.png")
                        : Path.Combine(randomPack, "posters", $"{poster.name}.png");
                }

                var texture = LoadTextureFromFile(texturePath);
                if (texture != null)
                {
                    renderer.material.mainTexture = texture;
                }
                else
                {
                    Logger.LogWarning($"Failed to load texture for {poster.name} from {texturePath}");
                    allTexturesLoaded = false;
                    break;
                }
            }

            if (!allTexturesLoaded)
            {
                UnityEngine.Object.Destroy(postersParent);
                Logger.LogWarning("Custom posters creation aborted due to missing textures.");
                return false;
            }

            Logger.LogInfo("Custom posters created successfully.");
            return true;
        }

        private static Texture2D LoadTextureFromFile(string relativePath)
        {
            foreach (var folder in Plugin.PosterFolders)
            {
                string fullPath = Path.Combine(folder, relativePath);
                if (File.Exists(fullPath))
                {
                    var texture = new Texture2D(2, 2);
                    if (texture.LoadImage(File.ReadAllBytes(fullPath)))
                    {
                        texture.filterMode = FilterMode.Point;
                        Logger.LogInfo($"Loaded texture from {fullPath}");
                        return texture;
                    }
                    else
                    {
                        Logger.LogError($"Failed to load texture from {fullPath}");
                        return null;
                    }
                }
            }

            Logger.LogError($"Texture file not found: {relativePath}");
            return null;
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
