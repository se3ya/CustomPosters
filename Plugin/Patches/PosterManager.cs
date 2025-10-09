using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomPosters.Data; // <-- IMPORTANT: Add this using statement
using UnityEngine;

namespace CustomPosters
{
    internal static class PosterManager
    {
        private static bool _materialsUpdated = false;
        private static string? _selectedPack = null;
        private static Material? _copiedMaterial = null;
        private static readonly List<GameObject> CreatedPosters = new List<GameObject>();
        private static int _sessionMapSeed = 0;
        private static bool _sessionSeedInitialized = false;

        public static bool IsNewLobby { get; set; } = true;
        
        public static void ResetSession()
        {
            _sessionSeedInitialized = false;
            _sessionMapSeed = 0;
            Plugin.Log.LogDebug("Reset session seed initialization");
        }
        
        public static void OnRoundStart(StartOfRound instance)
        {
            _materialsUpdated = false;

            if (!Plugin.Service.IsBiggerShipInstalled)
            {
                CopyPlane001Material();
            }

            if (IsNewLobby)
            {
                if (!_sessionSeedInitialized)
                {
                    _sessionMapSeed = PosterConfig.PerSession.Value ? StartOfRound.Instance.randomMapSeed : Environment.TickCount;
                    _sessionSeedInitialized = true;
                    Plugin.Log.LogDebug($"Initialized session with map seed: {_sessionMapSeed}");
                }

                int seedToUse;
                if (PosterConfig.PerSession.Value)
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

        private static void CopyPlane001Material()
        {
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane == null)
            {
                Plugin.Log.LogError("Poster plane Plane.001 not found under HangarShip");
                return;
            }

            var originalRenderer = posterPlane.GetComponent<MeshRenderer>();
            if (originalRenderer == null || originalRenderer.materials.Length == 0)
            {
                Plugin.Log.LogError("Poster plane renderer or materials not found");
                return;
            }

            _copiedMaterial = new Material(originalRenderer.material);
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
            var newPoster = GameObject.CreatePrimitive(PrimitiveType.Quad);
            if (newPoster == null)
            {
                Plugin.Log.LogError("Failed to create new poster GameObject");
            }
            return newPoster!;
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
            
            // --- THIS IS THE BIG CHANGE ---
            // All the hardcoded positions and complex if-statements have been removed.
            // We now get the correct layout with one simple line from our new provider class.
            var posterData = PosterLayoutProvider.GetLayout();
            // ------------------------------
            
            var enabledPacks = Plugin.Service.PosterFolders.Where(folder => PosterConfig.IsPackEnabled(folder)).ToList();
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
            if (PosterConfig.RandomizerModeSetting.Value == PosterConfig.RandomizerMode.PerPack)
            {
                if (!PosterConfig.PerSession.Value || _selectedPack == null || !enabledPacks.Contains(_selectedPack))
                {
                    var packChances = enabledPacks.Select(p => PosterConfig.GetPackChance(p)).ToList();
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
                if (!PosterConfig.PerSession.Value)
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
                        .Where(f => validExtensions.Contains(Path.GetExtension(f).ToLower()) && PosterConfig.IsFileEnabled(f))
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
                    var fileChances = kvp.Value.Select(t => PosterConfig.GetFileChance(t.filePath)).ToList();
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
                    var fileChances = kvp.Value.Select(v => PosterConfig.GetFileChance(v)).ToList();
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
                
                // CHANGED: Using uppercase properties from our new PosterData struct
                poster.name = posterData[i].Name;
                poster.transform.SetParent(postersParent.transform);
                poster.transform.position = posterData[i].Position;
                poster.transform.rotation = Quaternion.Euler(posterData[i].Rotation);
                poster.transform.localScale = posterData[i].Scale;

                var posterKey = posterData[i].Name.ToLower();
                if (prioritizedContent.TryGetValue(posterKey, out var content) && PosterConfig.IsFileEnabled(content.filePath))
                {
                    var renderer = poster.AddComponent<PosterRenderer>();
                    var (texture, filePath, isVideo) = content;
                    renderer.Initialize(texture, isVideo ? filePath : null, _copiedMaterial);

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
                Plugin.Log.LogInfo("Custom posters created successfully");
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

        private static IEnumerator UpdateBiggerShipPostersAsync()
        {
            CleanUpPosters();
            
            var posterParent = GameObject.Find("Environment/HangarShip/Plane.001/Posters");
            if (posterParent == null)
            {
                Plugin.Log.LogError("BiggerShip compatibility error: Could not find the 'Posters' parent object.");
                yield break;
            }

            var posterNames = new List<string>();
            foreach (Transform posterTransform in posterParent.transform)
            {
                posterNames.Add(posterTransform.name.ToLower());
            }

            if (posterNames.Count == 0)
            {
                Plugin.Log.LogWarning("BiggerShip compatibility: No child poster objects found.");
                yield break;
            }

            var enabledPacks = Plugin.Service.PosterFolders.Where(folder => PosterConfig.IsPackEnabled(folder)).ToList();
            if (enabledPacks.Count == 0)
            {
                Plugin.Log.LogWarning("No enabled packs found");
                yield break;
            }

            List<string> packsToUse;
            if (PosterConfig.RandomizerModeSetting.Value == PosterConfig.RandomizerMode.PerPack)
            {
                if (!PosterConfig.PerSession.Value || _selectedPack == null || !enabledPacks.Contains(_selectedPack))
                {
                    _selectedPack = enabledPacks[Plugin.Service.Rand.Next(enabledPacks.Count)];
                }
                packsToUse = new List<string> { _selectedPack! };
            }
            else
            {
                packsToUse = enabledPacks;
            }

            var allTextures = new Dictionary<string, List<(Texture2D texture, string filePath)>>();
            var allVideos = new Dictionary<string, List<string>>();

            foreach (var pack in packsToUse)
            {
                var pathsToCheck = new[] { Path.Combine(pack, "posters"), Path.Combine(pack, "tips"), Path.Combine(pack, "CustomPosters", "posters"), Path.Combine(pack, "CustomPosters", "tips") }
                    .Where(Directory.Exists);

                foreach (var path in pathsToCheck)
                {
                    var files = Directory.GetFiles(path)
                        .Where(f => new[] { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" }.Contains(Path.GetExtension(f).ToLower()) && PosterConfig.IsFileEnabled(f))
                        .ToList();

                    foreach (var file in files)
                    {
                        if (Path.GetExtension(file).ToLower() == ".mp4")
                        {
                            var videoName = Path.GetFileNameWithoutExtension(file).ToLower();
                            if (!allVideos.ContainsKey(videoName)) allVideos[videoName] = new List<string>();
                            allVideos[videoName].Add(file);
                            Plugin.Service.CacheVideo(file);
                        }
                        else
                        {
                            yield return LoadTextureAsync(file, result =>
                            {
                                if (result.texture != null && result.filePath != null)
                                {
                                    var textureName = Path.GetFileNameWithoutExtension(result.filePath).ToLower();
                                    if (!allTextures.ContainsKey(textureName)) allTextures[textureName] = new List<(Texture2D, string)>();
                                    allTextures[textureName].Add((result.texture, result.filePath));
                                }
                            });
                        }
                    }
                }
            }

            var prioritizedContent = new Dictionary<string, (Texture2D? texture, string filePath, bool isVideo)>();
            foreach (var kvp in allTextures)
            {
                var selected = kvp.Value.OrderBy(t => Plugin.Service.GetFilePriority(t.filePath)).First();
                prioritizedContent[kvp.Key] = (selected.Item1, selected.Item2, false);
            }
            foreach (var kvp in allVideos)
            {
                var selected = kvp.Value.OrderBy(v => Plugin.Service.GetFilePriority(v)).First();
                prioritizedContent[kvp.Key] = (null, selected, true);
            }
            
            foreach (Transform posterTransform in posterParent.transform)
            {
                var posterObject = posterTransform.gameObject;
                var posterKey = posterObject.name.ToLower();

                if (prioritizedContent.TryGetValue(posterKey, out var content))
                {
                    var meshRenderer = posterObject.GetComponent<MeshRenderer>();
                    if (meshRenderer == null)
                    {
                        Plugin.Log.LogWarning($"BiggerShip compatibility: Poster '{posterObject.name}' has no MeshRenderer.");
                        continue;
                    }

                    var rendererComponent = posterObject.AddComponent<PosterRenderer>();
                    rendererComponent.Initialize(content.texture, content.isVideo ? content.filePath : null, meshRenderer.material);
                    Plugin.Log.LogInfo($"Applied custom content to BiggerShip poster: {posterObject.name}");
                }
                else
                {
                    Plugin.Log.LogDebug($"No custom content found for BiggerShip poster: {posterObject.name}");
                }
                yield return null;
            }

            Plugin.Log.LogInfo("Finished applying textures to BiggerShip posters.");
        }

        private static IEnumerator DelayedUpdateMaterialsAsync(StartOfRound instance)
        {
            if (_materialsUpdated)
                yield break;

            yield return new WaitForEndOfFrame();

            if (Plugin.Service.IsBiggerShipInstalled)
            {
                yield return instance.StartCoroutine(UpdateBiggerShipPostersAsync());
            }
            else
            {
                var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
                if (posterPlane != null)
                {
                    posterPlane.SetActive(false);
                }

                HideVanillaPosterPlane();
                yield return instance.StartCoroutine(CreateCustomPostersAsync());
            }

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