using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomPosters.Data;
using UnityEngine;

namespace CustomPosters
{
    internal static class PosterManager
    {
        private static bool _materialsUpdated = false;
        private static string? _selectedPack = null;
        private static readonly List<GameObject> CreatedPosters = new List<GameObject>();
        private static int _sessionMapSeed = 0;
        private static bool _sessionSeedInitialized = false;

        public static bool IsNewLobby { get; set; } = true;
        
        public static void ResetSession()
        {
            _sessionSeedInitialized = false;
            _sessionMapSeed = 0;
            Plugin.Log.LogDebug("Session randomization reset.");
        }
        
        public static void OnRoundStart(StartOfRound instance)
        {
            _materialsUpdated = false;

            if (IsNewLobby)
            {
                if (!_sessionSeedInitialized)
                {
                    _sessionMapSeed = Plugin.ModConfig.PerSession.Value ? StartOfRound.Instance.randomMapSeed : Environment.TickCount;
                    _sessionSeedInitialized = true;
                    Plugin.Log.LogDebug($"Seed: {_sessionMapSeed}");
                }

                int seedToUse;
                if (Plugin.ModConfig.PerSession.Value)
                {
                    seedToUse = _sessionMapSeed;
                }
                else
                {
                    seedToUse = Environment.TickCount;
                    _selectedPack = null;
                }

                Plugin.Service.SetRandomSeed(seedToUse);
            }

            if (instance.inShipPhase)
            {
                instance.StartCoroutine(DelayedUpdateMaterialsAsync(instance));
            }
            IsNewLobby = false;
        }

        private static IEnumerator LoadTextureAsync(string filePath, Action<(Texture2D? texture, string? filePath)> onComplete)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Plugin.Log.LogError($"File not found: {filePath}");
                    onComplete?.Invoke((null, null));
                    yield break;
                }

                var cachedTexture = Plugin.Service.GetCachedTexture(filePath);
                if (cachedTexture != null)
                {
                    onComplete?.Invoke((cachedTexture, filePath));
                    yield break;
                }

                var fileData = File.ReadAllBytes(filePath);
                var texture = new Texture2D(2, 2);
                if (!texture.LoadImage(fileData))
                {
                    Plugin.Log.LogError($"Failed to load texture from {filePath}");
                    onComplete?.Invoke((null, null));
                    yield break;
                }

                texture.filterMode = FilterMode.Point;
                Plugin.Service.CacheTexture(filePath, texture);
                onComplete?.Invoke((texture, filePath));
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error loading file {filePath}: {ex.Message}");
                onComplete?.Invoke((null, null));
            }
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

        private static void CleanUpPosters()
        {
            foreach (var poster in CreatedPosters)
            {
                if (poster != null)
                {
                    var renderer = poster.GetComponent<PosterRenderer>();
                    if (renderer != null) UnityEngine.Object.Destroy(renderer);
                    UnityEngine.Object.Destroy(poster);
                }
            }
            CreatedPosters.Clear();
        }

        private static GameObject CreatePoster()
        {
            if (AssetManager.PosterPrefab == null)
            {
                Plugin.Log.LogError("Cannot create poster because PosterPrefab is not loaded.");
                return null!;
            }

            var newPoster = UnityEngine.Object.Instantiate(AssetManager.PosterPrefab);
            return newPoster;
        }
        
        private static IEnumerator CreateCustomPostersAsync()
        {
            CleanUpPosters();

            var hangarShip = GameObject.Find("Environment/HangarShip");
            if (hangarShip == null)
            {
                Plugin.Log.LogError("HangarShip GameObject not found");
                yield break;
            }

            var postersParent = new GameObject("CustomPosters");
            postersParent.transform.SetParent(hangarShip.transform);
            postersParent.transform.localPosition = Vector3.zero;

            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            
            var posterData = PosterLayoutProvider.GetLayout();
            
            var enabledPacks = Plugin.Service.PosterFolders.Where(folder => Plugin.ModConfig.IsPackEnabled(folder)).ToList();
            if (enabledPacks.Count == 0)
            {
                Plugin.Log.LogWarning("No enabled packs found");
                if (posterPlane != null)
                {
                    posterPlane.SetActive(true);
                }
                yield break;
            }

            var enabledPackNames = enabledPacks.Select(pack => Path.GetFileName(pack)).ToList();

            List<string> packsToUse;
            if (Plugin.ModConfig.RandomizerModeSetting.Value == PosterConfig.RandomizerMode.PerPack)
            {
                if (!Plugin.ModConfig.PerSession.Value || _selectedPack == null || !enabledPacks.Contains(_selectedPack))
                {
                    var packChances = enabledPacks.Select(p => Plugin.ModConfig.GetPackChance(p)).ToList();
                    if (packChances.Any(c => c > 0))
                    {
                        var totalChance = packChances.Sum();
                        var randomValue = Plugin.Service.Rand.NextDouble() * totalChance;
                        double cumulative = 0;
                        for (int i = 0; i < enabledPacks.Count; i++)
                        {
                            cumulative += packChances[i];
                            if (randomValue <= cumulative)
                            {
                                _selectedPack = enabledPacks[i];
                                break;
                            }
                        }
                        _selectedPack ??= enabledPacks[0];
                    }
                    else
                    {
                        _selectedPack = enabledPacks[Plugin.Service.Rand.Next(enabledPacks.Count)];
                    }
                    var selectedPackName = Path.GetFileName(_selectedPack);
                    Plugin.Log.LogInfo($"PerPack randomization enabled. Using pack: {selectedPackName}");
                }
                packsToUse = new List<string> { _selectedPack! };
            }
            else
            {
                if (!Plugin.ModConfig.PerSession.Value)
                {
                    _selectedPack = null;
                }
                packsToUse = enabledPacks;
                Plugin.Log.LogInfo("PerPoster - true, combining enabled packs");
            }

            var allTextures = new Dictionary<string, List<(Texture2D texture, string filePath)>>();
            var allVideos = new Dictionary<string, List<string>>();
            foreach (var pack in packsToUse)
            {
                string packName = Path.GetFileName(pack);
                string postersPath = Path.Combine(pack, "posters");
                string tipsPath = Path.Combine(pack, "tips");
                string nestedPostersPath = Path.Combine(pack, "CustomPosters", "posters");
                string nestedTipsPath = Path.Combine(pack, "CustomPosters", "tips");

                var filesToLoad = new List<string>();
                var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" };

                var pathsToCheck = new[] { postersPath, tipsPath, nestedPostersPath, nestedTipsPath }
                    .Where(p => Directory.Exists(p))
                    .Select(p => Path.GetFullPath(p).Replace('\\', '/'))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var path in pathsToCheck)
                {
                    var files = Directory.GetFiles(path)
                        .Where(f => validExtensions.Contains(Path.GetExtension(f).ToLower()) && Plugin.ModConfig.IsFileEnabled(f))
                        .Select(f => Path.GetFullPath(f).Replace('\\', '/'))
                        .ToList();

                    filesToLoad.AddRange(files);
                }

                filesToLoad = filesToLoad.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                const int batchSize = 5;
                for (int i = 0; i < filesToLoad.Count; i += batchSize)
                {
                    var batch = filesToLoad.Skip(i).Take(batchSize).ToList();
                    foreach (var file in batch)
                    {
                        if (Path.GetExtension(file).ToLower() == ".mp4")
                        {
                            var posterName = Path.GetFileNameWithoutExtension(file).ToLower();
                            if (!allVideos.ContainsKey(posterName))
                            {
                                allVideos[posterName] = new List<string>();
                            }
                            allVideos[posterName].Add(file);
                            Plugin.Service.CacheVideo(file);
                        }
                        else
                        {
                            yield return LoadTextureAsync(file, (result) =>
                            {
                                if (result.texture != null && result.filePath != null)
                                {
                                    var posterName = Path.GetFileNameWithoutExtension(result.filePath).ToLower();
                                    if (!allTextures.ContainsKey(posterName))
                                    {
                                        allTextures[posterName] = new List<(Texture2D, string)>();
                                    }
                                    allTextures[posterName].Add((result.texture, result.filePath));
                                }
                                else
                                {
                                    Plugin.Log.LogWarning($"Failed to load texture from {file}");
                                }
                            });
                        }
                    }
                    yield return null;
                }
            }

            var prioritizedContent = new Dictionary<string, (Texture2D? texture, string filePath, bool isVideo)>();
            foreach (var kvp in allTextures)
            {
                if (kvp.Value.Count > 1)
                {
                    var fileChances = kvp.Value.Select(t => Plugin.ModConfig.GetFileChance(t.filePath)).ToList();
                    if (fileChances.Any(c => c > 0))
                    {
                        var totalChance = fileChances.Sum();
                        var randomValue = Plugin.Service.Rand.NextDouble() * totalChance;
                        double cumulative = 0;
                        for (int i = 0; i < kvp.Value.Count; i++)
                        {
                            cumulative += fileChances[i];
                            if (randomValue <= cumulative)
                            {
                                prioritizedContent[kvp.Key] = (kvp.Value[i].texture, kvp.Value[i].filePath, false);
                                break;
                            }
                        }
                        if (!prioritizedContent.ContainsKey(kvp.Key))
                        {
                            prioritizedContent[kvp.Key] = (kvp.Value[0].texture, kvp.Value[0].filePath, false);
                        }
                    }
                    else
                    {
                        var selected = kvp.Value.OrderBy(t => Plugin.Service.GetFilePriority(t.filePath)).First();
                        prioritizedContent[kvp.Key] = (selected.texture, selected.filePath, false);
                    }
                }
                else
                {
                    prioritizedContent[kvp.Key] = (kvp.Value[0].texture, kvp.Value[0].filePath, false);
                }
            }

            foreach (var kvp in allVideos)
            {
                if (kvp.Value.Count > 1)
                {
                    var fileChances = kvp.Value.Select(v => Plugin.ModConfig.GetFileChance(v)).ToList();
                    if (fileChances.Any(c => c > 0))
                    {
                        var totalChance = fileChances.Sum();
                        var randomValue = Plugin.Service.Rand.NextDouble() * totalChance;
                        double cumulative = 0;
                        for (int i = 0; i < kvp.Value.Count; i++)
                        {
                            cumulative += fileChances[i];
                            if (randomValue <= cumulative)
                            {
                                prioritizedContent[kvp.Key] = (null, kvp.Value[i], true);
                                break;
                            }
                        }
                        if (!prioritizedContent.ContainsKey(kvp.Key))
                        {
                            prioritizedContent[kvp.Key] = (null, kvp.Value[0], true);
                        }
                    }
                    else
                    {
                        var selected = kvp.Value.OrderBy(v => Plugin.Service.GetFilePriority(v)).First();
                        prioritizedContent[kvp.Key] = (null, selected, true);
                    }
                }
                else
                {
                    prioritizedContent[kvp.Key] = (null, kvp.Value[0], true);
                }
            }

            if (allTextures.Count == 0 && allVideos.Count == 0)
            {
                Plugin.Log.LogWarning("No textures found in enabled packs");
                if (posterPlane != null)
                {
                    posterPlane.SetActive(true);
                }
                yield break;
            }

            bool anyPosterLoaded = false;

            for (int i = 0; i < posterData.Length; i++)
            {
                var poster = CreatePoster();
                if (poster == null)
                {
                    continue;
                }
                
                poster.name = posterData[i].Name;
                poster.transform.SetParent(postersParent.transform);
                poster.transform.position = posterData[i].Position;
                poster.transform.rotation = Quaternion.Euler(posterData[i].Rotation);
                poster.transform.localScale = posterData[i].Scale;

                var posterKey = posterData[i].Name.ToLower();
                if (prioritizedContent.TryGetValue(posterKey, out var content) && Plugin.ModConfig.IsFileEnabled(content.filePath))
                {
                    var renderer = poster.AddComponent<PosterRenderer>();
                    var (texture, filePath, isVideo) = content;
                    var posterMeshRenderer = poster.GetComponent<MeshRenderer>();
                    renderer.Initialize(texture, isVideo ? filePath : null, posterMeshRenderer?.material);

                    CreatedPosters.Add(poster);
                    anyPosterLoaded = true;
                }
                else
                {
                    Plugin.Log.LogWarning($"No enabled texture or video found for {posterData[i].Name}. Destroying the poster");
                    UnityEngine.Object.Destroy(poster);
                }
                yield return null;
            }

            if (anyPosterLoaded)
            {
                if (posterPlane != null)
                {
                    UnityEngine.Object.Destroy(posterPlane);
                }
                var vanillaPlane = hangarShip.transform.Find("Plane")?.gameObject;
                if (vanillaPlane != null)
                {
                    UnityEngine.Object.Destroy(vanillaPlane);
                }
                Plugin.Log.LogInfo("Posters created successfully!");
            }
            else
            {
                if (posterPlane != null)
                {
                    posterPlane.SetActive(true);
                    Plugin.Log.LogWarning("Re-enabled vanilla Plane.001 poster due to no custom posters loaded");
                }
            }
        }


        private static IEnumerator DelayedUpdateMaterialsAsync(StartOfRound instance)
        {
            if (_materialsUpdated)
                yield break;

            yield return new WaitForEndOfFrame();

            HideVanillaPosterPlane();
            yield return instance.StartCoroutine(CreateCustomPostersAsync());

            _materialsUpdated = true;
        }


        public static void ChangePosterPack(string packName)
        {
            if (string.IsNullOrEmpty(packName))
            {
                var enabledPacks = Plugin.Service.GetEnabledPackNames();
                if (enabledPacks.Count == 0) return;

                int currentIndex = enabledPacks.FindIndex(p => p.Equals(_selectedPack, StringComparison.OrdinalIgnoreCase));
                _selectedPack = enabledPacks[(currentIndex + 1) % enabledPacks.Count];
            }
            else
            {
                if (Plugin.Service.GetEnabledPackNames().Contains(packName, StringComparer.OrdinalIgnoreCase))
                {
                    _selectedPack = packName;
                }
                else
                {
                    Plugin.Log.LogWarning($"Attempted to select invalid pack: {packName}");
                    return;
                }
            }

            Plugin.Service.SetRandomSeed(Environment.TickCount);
            Plugin.Log.LogInfo($"Changed poster pack to - {_selectedPack}");

            _materialsUpdated = false;
            StartOfRound? instance = MonoBehaviour.FindObjectOfType<StartOfRound>();
            if (instance != null && instance.inShipPhase)
            {
                instance.StartCoroutine(DelayedUpdateMaterialsAsync(instance));
            }
        }
    }
}