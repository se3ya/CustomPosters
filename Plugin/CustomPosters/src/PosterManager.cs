using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using CustomPosters.Data;
using CustomPosters.Utils;
using CustomPosters.Networking;

namespace CustomPosters
{
    internal static class PosterManager
    {
        private static bool _materialsUpdated = false;
        internal static string? _selectedPack = null;
        public static string? SelectedPack => _selectedPack;
        private static readonly List<GameObject> CreatedPosters = new List<GameObject>();
        private static int _sessionMapSeed = 0;
        private static bool _sessionSeedInitialized = false;

        public static bool IsNewLobby { get; set; } = true;

        private static bool ShouldActAsHost => !Plugin.ModConfig.EnableNetworking.Value || (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost);

        public static void ResetSession()
        {
            _sessionSeedInitialized = false;
            _sessionMapSeed = 0;
            _selectedPack = null;
            Plugin.Log.LogDebug("Session randomization reset.");
        }

        public static void SetPackForClients(string packName)
        {
            if (!Plugin.ModConfig.EnableNetworking.Value) return;
            if (NetworkManager.Singleton == null) return;
            if (NetworkManager.Singleton.IsHost) return;

            Plugin.Log.LogDebug($"Client received selected pack from host: {PathUtils.GetPrettyPath(packName)}");
            _selectedPack = packName;

            _materialsUpdated = false;
            StartOfRound instance = StartOfRound.Instance;
            if (instance != null && instance.inShipPhase)
            {
                instance.StartCoroutine(DelayedUpdateMaterialsAsync(instance));
            }
        }

        public static void OnRoundStart(StartOfRound instance)
        {
            _materialsUpdated = false;

            if (!ShouldActAsHost) 
            {
                return;
            }

            if (IsNewLobby)
            {
                InitializeSessionSeedIfNeeded();
                var seedToUse = ComputeSeedAndMaybeLoadSavePack(Plugin.ModConfig.KeepPackFor.Value);
                Plugin.Service.SetRandomSeed(seedToUse);
            }

            if (instance.inShipPhase)
            {
                instance.StartCoroutine(DelayedUpdateMaterialsAsync(instance));
            }
            IsNewLobby = false;
        }

        private static void InitializeSessionSeedIfNeeded()
        {
            var mode = Plugin.ModConfig.KeepPackFor.Value;
            if (_sessionSeedInitialized) return;

            _sessionMapSeed = (mode == PosterConfig.KeepFor.Session)
                ? StartOfRound.Instance.randomMapSeed
                : Environment.TickCount;
            _sessionSeedInitialized = true;
            Plugin.Log.LogDebug($"Seed: {_sessionMapSeed}");
        }

        private static int ComputeSeedAndMaybeLoadSavePack(PosterConfig.KeepFor mode)
        {
            switch (mode)
            {
                case PosterConfig.KeepFor.Session:
                    return _sessionMapSeed;

                case PosterConfig.KeepFor.SaveSlot:
                    string? saveIdForSeed = null;
                    try { saveIdForSeed = Utils.SavePersistenceManager.TryGetCurrentSaveId(); }
                    catch { }
                    var seed = !string.IsNullOrEmpty(saveIdForSeed)
                        ? Utils.HashUtils.DeterministicHash(saveIdForSeed!)
                        : Environment.TickCount;

                    TryLoadSavedPack();
                    return seed;

                case PosterConfig.KeepFor.Lobby:
                default:
                    _selectedPack = null;
                    return Environment.TickCount;
            }
        }

        private static void TryLoadSavedPack()
        {
            try
            {
                var saveId = Utils.SavePersistenceManager.TryGetCurrentSaveId();
                string? savedPack = null;
                if (!string.IsNullOrEmpty(saveId))
                {
                    savedPack = Utils.SavePersistenceManager.TryLoadSelectedPack(saveId!);
                }

                if (!string.IsNullOrEmpty(savedPack))
                {
                    _selectedPack = savedPack;
                    Plugin.Log.LogInfo($"Loaded saved pack for save '{saveId}': {PathUtils.GetPrettyPath(_selectedPack)}");
                }
                else
                {
                    _selectedPack = null;
                }
            }
            catch (Exception ex)
            {
                _selectedPack = null;
                Plugin.Log.LogDebug($"SaveSlot load skipped: {ex.Message}");
            }
        }

        private static async Task LoadTextureAsync(string filePath, Action<(Texture2D? texture, string? filePath)> onComplete)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Plugin.Log.LogError($"File not found: {PathUtils.GetPrettyPath(filePath)}");
                    onComplete?.Invoke((null, null));
                    return;
                }

                var cachedTexture = Plugin.Service.GetCachedTexture(filePath);
                if (cachedTexture != null)
                {
                    onComplete?.Invoke((cachedTexture, filePath));
                    return;
                }

                var fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                var texture = new Texture2D(2, 2);
                if (!texture.LoadImage(fileData))
                {
                    Plugin.Log.LogError($"Failed to load texture from {PathUtils.GetPrettyPath(filePath)}");
                    onComplete?.Invoke((null, null));
                    return;
                }

                texture.filterMode = FilterMode.Point;
                Plugin.Service.CacheTexture(filePath, texture);
                onComplete?.Invoke((texture, filePath));
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error loading file {PathUtils.GetPrettyPath(filePath)}: {ex.Message}");
                onComplete?.Invoke((null, null));
            }
        }

        private static void HideVanillaPosterPlane()
        {
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001 (Old)") ?? GameObject.Find("Environment/HangarShip/Plane.001");
            posterPlane?.SetActive(false);
        }

        private static void CleanUpPosters()
        {
            foreach (var poster in CreatedPosters)
            {
                if (poster == null) continue;
        
                var renderer = poster.GetComponent<PosterRenderer>();
                if (renderer != null) UnityEngine.Object.Destroy(renderer);
                UnityEngine.Object.Destroy(poster);
            }
            CreatedPosters.Clear();
        }

        private static GameObject? CreatePoster(string posterName)
        {
            GameObject? prefab = null;
            
            switch (posterName)
            {
                case "CustomTips":
                    prefab = Plugin.ModConfig.UseTipsVanillaModel 
                        ? AssetManager.TipsPrefab 
                        : AssetManager.PosterPrefab;
                    break;
                    
                case "Poster5":
                    prefab = Plugin.ModConfig.UsePoster5VanillaModel
                        ? AssetManager.Poster5Prefab 
                        : AssetManager.PosterPrefab;
                    break;
                    
                default:
                    prefab = AssetManager.PosterPrefab;
                    break;
            }

            if (prefab == null)
            {
                Plugin.Log.LogError($"Failed to find prefab for {posterName}. Falling back to default poster prefab.");
                prefab = AssetManager.PosterPrefab;
                
                if (prefab == null)
                {
                    Plugin.Log.LogError("Default poster prefab is also null!");
                    return null;
                }
            }

            return UnityEngine.Object.Instantiate(prefab);
        }

        public static double? GetVideoTimeForPoster(string posterName)
        {
            var posterObject = CreatedPosters.FirstOrDefault(p => p.name == posterName);
            return posterObject?.GetComponent<PosterRenderer>()?.GetCurrentVideoTime();
        }

        public static void SetVideoTimeForPoster(string posterName, double time)
        {
            var posterObject = CreatedPosters.FirstOrDefault(p => p.name == posterName);
            posterObject?.GetComponent<PosterRenderer>()?.SetVideoTime(time);
        }
        
        private static IEnumerator CreateCustomPostersAsync()
        {
            CleanUpPosters();

            var hangarShip = GameObject.Find("Environment/HangarShip");
            if (hangarShip == null)
            {
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
                bool shouldSelectPack = ShouldActAsHost;

                if (shouldSelectPack)
                {
                    var mode = Plugin.ModConfig.KeepPackFor.Value;
                    if (mode == PosterConfig.KeepFor.SaveSlot)
                    {
                        var curId = Utils.SavePersistenceManager.TryGetCurrentSaveId();
                        if (string.IsNullOrEmpty(curId))
                        {
                            _selectedPack = null;
                        }
                    }
                    bool needNewSelection = mode switch
                    {
                        PosterConfig.KeepFor.Lobby => true,
                        PosterConfig.KeepFor.Session => _selectedPack == null || !enabledPacks.Contains(_selectedPack),
                        PosterConfig.KeepFor.SaveSlot => _selectedPack == null || !enabledPacks.Contains(_selectedPack),
                        _ => true
                    };

                    if (needNewSelection)
                    {
                        var candidatePacks = new List<string>(enabledPacks);
                        string? chosen = null;

                        while (candidatePacks.Count > 0)
                        {
                            var candidate = PackSelector.SelectPackByChance(candidatePacks);
                            if (PackHasValidFiles(candidate))
                            {
                                chosen = candidate;
                                break;
                            }
                            candidatePacks.Remove(candidate);
                        }

                        _selectedPack = chosen;

                        if (!string.IsNullOrEmpty(_selectedPack) && mode == PosterConfig.KeepFor.SaveSlot)
                        {
                            try
                            {
                                var saveId = Utils.SavePersistenceManager.TryGetCurrentSaveId();
                                if (!string.IsNullOrEmpty(saveId))
                                {
                                    Utils.SavePersistenceManager.SaveSelectedPack(saveId!, _selectedPack!);
                                    Plugin.Log.LogInfo($"Saved selected pack for save '{saveId}'");
                                }
                                else
                                {
                                    Plugin.Log.LogDebug("PerSave: No valid saveId yet; not persisting selection.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Plugin.Log.LogDebug($"SaveSlot save skipped: {ex.Message}");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(_selectedPack) && Plugin.ModConfig.EnableNetworking.Value)
                    {
                        PosterSyncManager.SendPacket(_selectedPack!);
                    }
                }

                if (string.IsNullOrEmpty(_selectedPack))
                {
                    Plugin.Log.LogInfo(Plugin.ModConfig.EnableNetworking.Value ? "Client is waiting for host to select a pack..." : "No valid pack found, falling back to vanilla posters");
                    if (posterPlane != null)
                    {
                        posterPlane.SetActive(true);
                    }
                    yield break;
                }

                var shortPackName = PathUtils.GetDisplayPackName(_selectedPack);
                Plugin.Log.LogInfo($"Using pack: {shortPackName}");
                packsToUse = new List<string> { _selectedPack };
            }
            else
            {
                packsToUse = enabledPacks;
                Plugin.Log.LogInfo("PerPoster mode enabled.");
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
                            (Texture2D? texture, string? filePath) textureResult = (null, null);

                            var loadTask = LoadTextureAsync(file, result => textureResult = result);

                            yield return new WaitUntil(() => loadTask.IsCompleted);

                            if (textureResult.texture != null && textureResult.filePath != null)
                            {
                                var posterName = Path.GetFileNameWithoutExtension(textureResult.filePath).ToLower();
                                if (!allTextures.ContainsKey(posterName))
                                {
                                    allTextures[posterName] = new List<(Texture2D, string)>();
                                }
                                allTextures[posterName].Add((textureResult.texture, textureResult.filePath));
                            }
                            else
                            {
                                Plugin.Log.LogWarning($"Failed to load texture from {file}");
                            }
                        }
                    }
                    yield return null;
                }
            }

            var prioritizedContent = new Dictionary<string, (Texture2D? texture, string filePath, bool isVideo)>();       
            // prioritize textures
            foreach (var kvp in allTextures)
            {
                var selected = PackSelector.SelectContentByChance(
                    kvp.Value,
                    t => Plugin.ModConfig.GetFileChance(t.filePath),
                    t => Plugin.Service.GetFilePriority(t.filePath)
                );
                prioritizedContent[kvp.Key] = (selected.texture, selected.filePath, false);
            }

            // prioritize videos
            foreach (var kvp in allVideos)
            {
                var selected = PackSelector.SelectContentByChance(
                    kvp.Value,
                    v => Plugin.ModConfig.GetFileChance(v),
                    v => Plugin.Service.GetFilePriority(v)
                );
                prioritizedContent[kvp.Key] = (null, selected, true);
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
                var poster = CreatePoster(posterData[i].Name);
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
                    Plugin.Log.LogWarning($"No enabled texture found for {posterData[i].Name}. Destroying the poster");
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
                    Plugin.Log.LogWarning("Re-enabled vanilla Plane.001 poster due to no textures loaded");
                }
            }
        }

        private static bool PackHasValidFiles(string pack)
        {
            try
            {
                var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" };

                var postersPath = Path.Combine(pack, "posters");
                var tipsPath = Path.Combine(pack, "tips");
                var nestedPostersPath = Path.Combine(pack, "CustomPosters", "posters");
                var nestedTipsPath = Path.Combine(pack, "CustomPosters", "tips");

                var paths = new[] { postersPath, tipsPath, nestedPostersPath, nestedTipsPath }
                    .Where(p => Directory.Exists(p)).ToList();

                foreach (var p in paths)
                {
                    if (Directory.EnumerateFiles(p).Any(f => validExtensions.Contains(Path.GetExtension(f).ToLower())))
                        return true;
                }

                if (File.Exists(Path.Combine(pack, "CustomTips.png")))
                    return true;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogDebug($"PackHasValidFiles error for {pack}: {ex.Message}");
            }

            return false;
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
            StartOfRound? instance = StartOfRound.Instance;
            if (instance != null && instance.inShipPhase)
            {
                instance.StartCoroutine(DelayedUpdateMaterialsAsync(instance));
            }
        }
    }
}