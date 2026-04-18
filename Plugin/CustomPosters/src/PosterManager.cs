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
            _selectedPack = LocalPackPath(packName);
            if (string.IsNullOrEmpty(_selectedPack))
            {
                Plugin.Log.LogWarning("Could not resolve host selected pack on client. Keeping vanilla posters.");
                return;
            }

            _materialsUpdated = false;
            StartOfRound instance = StartOfRound.Instance;
            if (instance != null && instance.inShipPhase)
            {
                instance.StartCoroutine(DelayedUpdateMaterialsAsync(instance));
            }
        }

        private static string? LocalPackPath(string hostPackIdentifier)
        {
            try
            {
                var hostDisplay = PathUtils.GetDisplayPackName(hostPackIdentifier);
                var hostFolder = Path.GetFileName(hostPackIdentifier.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                foreach (var localPath in Plugin.Service.PosterFolders)
                {
                    if (string.Equals(PathUtils.GetDisplayPackName(localPath), hostDisplay, StringComparison.OrdinalIgnoreCase))
                        return localPath;
                }
                foreach (var localPath in Plugin.Service.PosterFolders)
                {
                    if (string.Equals(Path.GetFileName(localPath), hostFolder, StringComparison.OrdinalIgnoreCase))
                        return localPath;
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogDebug($"LocalPackPath error: {ex.Message}");
            }
            return null;
        }

        public static void OnRoundStart(StartOfRound instance)
        {
            _materialsUpdated = false;

            if (IsNewLobby && ShouldActAsHost)
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
                    catch (Exception) { /* save id not available */ }
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
                case Constants.PosterNameCustomTips:
                    prefab = Plugin.ModConfig.UseTipsVanillaModel
                        ? AssetManager.TipsPrefab
                        : AssetManager.PosterPrefab;
                    break;

                case Constants.PosterNamePoster5:
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

            if (!TryResolvePacksToUse(posterPlane, out var packsToUse))
            {
                yield break;
            }

            var allTextures = new Dictionary<string, List<(Texture2D texture, string filePath)>>();
            var allVideos = new Dictionary<string, List<string>>();
            yield return LoadContentFromPacks(packsToUse, allTextures, allVideos);

            if (allTextures.Count == 0 && allVideos.Count == 0)
            {
                Plugin.Log.LogWarning("No textures found in enabled packs");
                if (posterPlane != null) posterPlane.SetActive(true);
                yield break;
            }

            var prioritizedContent = BuildPrioritizedContent(allTextures, allVideos);

            bool anyPosterLoaded = false;
            yield return SpawnPostersFromLayout(
                posterData,
                postersParent,
                prioritizedContent,
                loaded => anyPosterLoaded = loaded);

            FinalizeRender(anyPosterLoaded, hangarShip, posterPlane);
        }

        private static bool TryResolvePacksToUse(GameObject? posterPlane, out List<string> packsToUse)
        {
            packsToUse = new List<string>();

            var enabledPacks = Plugin.Service.PosterFolders
                .Where(folder => Plugin.ModConfig.IsPackEnabled(folder))
                .ToList();

            bool usingForcedHostPack = Plugin.ModConfig.EnableNetworking.Value
                && !ShouldActAsHost
                && !string.IsNullOrEmpty(_selectedPack);

            if (enabledPacks.Count == 0 && !usingForcedHostPack)
            {
                Plugin.Log.LogWarning("No enabled packs found");
                if (posterPlane != null) posterPlane.SetActive(true);
                return false;
            }

            if (Plugin.ModConfig.RandomizerModeSetting.Value == PosterConfig.RandomizerMode.PerPack)
            {
                return TryResolvePerPackSelection(enabledPacks, posterPlane, out packsToUse);
            }

            Plugin.Log.LogInfo("PerPoster mode enabled.");
            packsToUse = enabledPacks;
            return true;
        }

        private static bool TryResolvePerPackSelection(
            List<string> enabledPacks,
            GameObject? posterPlane,
            out List<string> packsToUse)
        {
            packsToUse = new List<string>();

            if (ShouldActAsHost)
            {
                SelectHostPack(enabledPacks);
                BroadcastSelectedPackIfNeeded();
            }

            if (string.IsNullOrEmpty(_selectedPack))
            {
                Plugin.Log.LogInfo(Plugin.ModConfig.EnableNetworking.Value
                    ? "Client is waiting for host to select a pack..."
                    : "No valid pack found, falling back to vanilla posters");
                if (posterPlane != null) posterPlane.SetActive(true);
                return false;
            }

            Plugin.Log.LogInfo($"Using pack: {PathUtils.GetDisplayPackName(_selectedPack)}");
            packsToUse = new List<string> { _selectedPack! };
            return true;
        }

        private static void SelectHostPack(List<string> enabledPacks)
        {
            var mode = Plugin.ModConfig.KeepPackFor.Value;

            if (mode == PosterConfig.KeepFor.SaveSlot
                && string.IsNullOrEmpty(Utils.SavePersistenceManager.TryGetCurrentSaveId()))
            {
                _selectedPack = null;
            }

            bool needNewSelection = mode switch
            {
                PosterConfig.KeepFor.Lobby => true,
                PosterConfig.KeepFor.Session => _selectedPack == null || !enabledPacks.Contains(_selectedPack),
                PosterConfig.KeepFor.SaveSlot => _selectedPack == null || !enabledPacks.Contains(_selectedPack),
                _ => true
            };

            if (!needNewSelection) return;

            _selectedPack = PickFirstValidPack(enabledPacks);

            if (!string.IsNullOrEmpty(_selectedPack) && mode == PosterConfig.KeepFor.SaveSlot)
            {
                PersistSelectionForSaveSlot(_selectedPack!);
            }
        }

        private static string? PickFirstValidPack(List<string> candidates)
        {
            var pool = new List<string>(candidates);
            while (pool.Count > 0)
            {
                var candidate = PackSelector.SelectPackByChance(pool);
                if (PackHasValidFiles(candidate)) return candidate;
                pool.Remove(candidate);
            }
            return null;
        }

        private static void PersistSelectionForSaveSlot(string selectedPack)
        {
            try
            {
                var saveId = Utils.SavePersistenceManager.TryGetCurrentSaveId();
                if (!string.IsNullOrEmpty(saveId))
                {
                    Utils.SavePersistenceManager.SaveSelectedPack(saveId!, selectedPack);
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

        private static void BroadcastSelectedPackIfNeeded()
        {
            if (!string.IsNullOrEmpty(_selectedPack) && Plugin.ModConfig.EnableNetworking.Value)
            {
                PosterSyncManager.SendPacket(_selectedPack!);
            }
        }

        private static IEnumerator LoadContentFromPacks(
            List<string> packsToUse,
            Dictionary<string, List<(Texture2D texture, string filePath)>> allTextures,
            Dictionary<string, List<string>> allVideos)
        {
            const int batchSize = 5;

            foreach (var pack in packsToUse)
            {
                var filesToLoad = GatherValidFilesForPack(pack);

                for (int i = 0; i < filesToLoad.Count; i += batchSize)
                {
                    var batch = filesToLoad.Skip(i).Take(batchSize).ToList();
                    foreach (var file in batch)
                    {
                        if (file.IsVideo())
                        {
                            RegisterVideo(file, allVideos);
                        }
                        else
                        {
                            yield return LoadTextureToDictionary(file, allTextures);
                        }
                    }
                    yield return null;
                }
            }
        }

        private static List<string> GatherValidFilesForPack(string pack)
        {
            var candidatePaths = new[]
            {
                Path.Combine(pack, "posters"),
                Path.Combine(pack, "tips"),
                Path.Combine(pack, "CustomPosters", "posters"),
                Path.Combine(pack, "CustomPosters", "tips")
            };

            var pathsToCheck = candidatePaths
                .Where(Directory.Exists)
                .Select(p => Path.GetFullPath(p).NormalizePath())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var results = new List<string>();
            foreach (var path in pathsToCheck)
            {
                results.AddRange(Directory.GetFiles(path)
                    .Where(f => f.IsValidPosterFile() && Plugin.ModConfig.IsFileEnabled(f))
                    .Select(f => Path.GetFullPath(f).NormalizePath()));
            }

            return results.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static void RegisterVideo(string file, Dictionary<string, List<string>> allVideos)
        {
            var posterName = file.GetPosterName();
            if (!allVideos.ContainsKey(posterName))
            {
                allVideos[posterName] = new List<string>();
            }
            allVideos[posterName].Add(file);
            Plugin.Service.CacheVideo(file);
        }

        private static IEnumerator LoadTextureToDictionary(
            string file,
            Dictionary<string, List<(Texture2D texture, string filePath)>> allTextures)
        {
            (Texture2D? texture, string? filePath) result = (null, null);
            var loadTask = LoadTextureAsync(file, r => result = r);
            yield return new WaitUntil(() => loadTask.IsCompleted);

            if (result.texture == null || result.filePath == null)
            {
                Plugin.Log.LogWarning($"Failed to load texture from {file}");
                yield break;
            }

            var posterName = result.filePath.GetPosterName();
            if (!allTextures.ContainsKey(posterName))
            {
                allTextures[posterName] = new List<(Texture2D, string)>();
            }
            allTextures[posterName].Add((result.texture, result.filePath));
        }

        private static Dictionary<string, (Texture2D? texture, string filePath, bool isVideo)> BuildPrioritizedContent(
            Dictionary<string, List<(Texture2D texture, string filePath)>> allTextures,
            Dictionary<string, List<string>> allVideos)
        {
            var prioritized = new Dictionary<string, (Texture2D? texture, string filePath, bool isVideo)>();
            var allPosterNames = allTextures.Keys.Union(allVideos.Keys);

            foreach (var posterName in allPosterNames)
            {
                var combined = new List<(Texture2D? texture, string filePath, bool isVideo)>();

                if (allTextures.TryGetValue(posterName, out var textures))
                {
                    combined.AddRange(textures.Select(t => ((Texture2D?)t.texture, t.filePath, false)));
                }
                if (allVideos.TryGetValue(posterName, out var videos))
                {
                    combined.AddRange(videos.Select(v => ((Texture2D?)null, v, true)));
                }

                if (combined.Count == 0) continue;

                prioritized[posterName] = PackSelector.SelectContentByChance(
                    combined,
                    item => Plugin.ModConfig.GetFileChance(item.filePath),
                    item => Plugin.Service.GetFilePriority(item.filePath));
            }

            return prioritized;
        }

        private static IEnumerator SpawnPostersFromLayout(
            PosterData[] posterData,
            GameObject postersParent,
            Dictionary<string, (Texture2D? texture, string filePath, bool isVideo)> prioritizedContent,
            Action<bool> onCompleted)
        {
            bool anyPosterLoaded = false;

            for (int i = 0; i < posterData.Length; i++)
            {
                var poster = CreatePoster(posterData[i].Name);
                if (poster == null)
                {
                    continue;
                }

                ApplyPosterTransform(poster, posterData[i], postersParent.transform);

                if (TryAssignPosterContent(poster, posterData[i].Name, prioritizedContent))
                {
                    anyPosterLoaded = true;
                }

                yield return null;
            }

            onCompleted(anyPosterLoaded);
        }

        private static void ApplyPosterTransform(GameObject poster, PosterData data, Transform parent)
        {
            poster.name = data.Name;
            poster.transform.SetParent(parent);
            poster.transform.position = data.Position;
            poster.transform.rotation = Quaternion.Euler(data.Rotation);
            poster.transform.localScale = data.Scale;
        }

        private static bool TryAssignPosterContent(
            GameObject poster,
            string posterName,
            Dictionary<string, (Texture2D? texture, string filePath, bool isVideo)> prioritizedContent)
        {
            var key = posterName.ToLower();

            if (!prioritizedContent.TryGetValue(key, out var content)
                || !Plugin.ModConfig.IsFileEnabled(content.filePath))
            {
                Plugin.Log.LogWarning($"No enabled texture found for {posterName}. Destroying the poster");
                UnityEngine.Object.Destroy(poster);
                return false;
            }

            var renderer = poster.AddComponent<PosterRenderer>();
            var meshRenderer = poster.GetComponent<MeshRenderer>();
            renderer.Initialize(content.texture, content.isVideo ? content.filePath : null, meshRenderer?.material);
            CreatedPosters.Add(poster);
            return true;
        }

        private static void FinalizeRender(bool anyPosterLoaded, GameObject hangarShip, GameObject? posterPlane)
        {
            if (anyPosterLoaded)
            {
                if (posterPlane != null) UnityEngine.Object.Destroy(posterPlane);
                var vanillaPlane = hangarShip.transform.Find("Plane")?.gameObject;
                if (vanillaPlane != null) UnityEngine.Object.Destroy(vanillaPlane);
                Plugin.Log.LogInfo("Posters created successfully!");
                return;
            }

            if (posterPlane != null)
            {
                posterPlane.SetActive(true);
                Plugin.Log.LogWarning("Re-enabled vanilla Plane.001 poster due to no textures loaded");
            }
        }

        private static bool PackHasValidFiles(string pack)
        {
            try
            {
                var postersPath = Path.Combine(pack, "posters");
                var tipsPath = Path.Combine(pack, "tips");
                var nestedPostersPath = Path.Combine(pack, "CustomPosters", "posters");
                var nestedTipsPath = Path.Combine(pack, "CustomPosters", "tips");

                var paths = new[] { postersPath, tipsPath, nestedPostersPath, nestedTipsPath }
                    .Where(p => Directory.Exists(p)).ToList();

                foreach (var p in paths)
                {
                    if (Directory.EnumerateFiles(p).Any(f => f.IsValidPosterFile()))
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
