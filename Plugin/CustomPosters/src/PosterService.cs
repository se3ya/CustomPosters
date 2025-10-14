using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using UnityEngine;
using CustomPosters.Utils;

namespace CustomPosters
{
    public class PosterService
    {
        private readonly List<string> _posterFolders = new List<string>();
        private readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        private readonly Dictionary<string, string> _videoCache = new Dictionary<string, string>();
        private System.Random _rand = null!;
        public IReadOnlyList<string> PosterFolders => _posterFolders.AsReadOnly();

        public bool IsBiggerShipInstalled { get; private set; }
        public bool IsShipWindowsInstalled { get; private set; }
        public bool IsWindow2Enabled { get; private set; }
        public bool IsWiderShipModInstalled { get; private set; }
        public string WiderShipExtendedSide { get; private set; } = "Both";
        public bool Is2StoryShipModInstalled { get; private set; }
        public bool EnableRightWindows { get; private set; }
        public bool EnableLeftWindows { get; private set; }
        public Dictionary<string, bool> ShipWindowsStates { get; private set; } = new Dictionary<string, bool>();

        public System.Random Rand => _rand;

        public PosterService()
        {
            try
            {
                var pluginPath = Paths.PluginPath;
                var modFolderNames = new HashSet<string>(Constants.ExcludedModFolderNames, StringComparer.OrdinalIgnoreCase);

                foreach (var folder in Directory.GetDirectories(pluginPath))
                {
                    var folderName = Path.GetFileName(folder);
                    if (modFolderNames.Contains(folderName))
                    {
                        continue;
                    }

                    if (IsValidPosterPack(folder))
                    {
                        _posterFolders.Add(folder);
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error scanning for poster packs: {ex.Message}");
            }

            InitializeBiggerShip();
            InitializeShipWindows();
            InitializeWiderShipMod();
            Initialize2StoryShipMod();
            SetRandomSeed(Environment.TickCount);
        }

        private bool IsValidPosterPack(string folderPath)
        {
            var pathsToCheck = Constants.PosterPackSubdirectories
                .Select(subDir => Path.Combine(folderPath, subDir));

            foreach (var path in pathsToCheck)
            {
                if (Directory.Exists(path) && 
                    Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                     .Any(file => Constants.AllValidExtensions.Contains(Path.GetExtension(file).ToLowerInvariant())))
                {
                    return true;
                }
            }

            return false;
        }

        private void InitializeBiggerShip()
        {
            IsBiggerShipInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Constants.BiggerShipGUID);
            if (IsBiggerShipInstalled)
            {
                Plugin.Log.LogInfo("Detected BiggerShip");
                return;
            }
        }

        private void InitializeShipWindows()
        {
            IsShipWindowsInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Constants.ShipWindowsGUID);
            if (!IsShipWindowsInstalled) return;

            var configPath = Path.Combine(Paths.ConfigPath, "TestAccount666.ShipWindows.cfg");
            IsWindow2Enabled = ConfigFileReader.ReadBoolFromSection(
                configPath, 
                "Right Window (SideRight)", 
                "1. Enabled = ", 
                false
            );
    
            Plugin.Log.LogInfo($"Detected ShipWindows, Right Window - {IsWindow2Enabled}");
        }

        private void InitializeWiderShipMod()
        {
            IsWiderShipModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Constants.WiderShipModGUID);
            if (!IsWiderShipModInstalled) return;

            try
            {
                var widerShipType = Type.GetType("WiderShipMod");
                var extendedSideField = widerShipType?.GetField("ExtendedSide", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        
                if (extendedSideField != null)
                {
                    WiderShipExtendedSide = (string)extendedSideField.GetValue(null);
                }
                else
                {
                    ReadWiderShipConfigFile();
                }
            }
            catch
            {
                ReadWiderShipConfigFile();
            }
    
            Plugin.Log.LogInfo($"Detected WiderShip, ES - {WiderShipExtendedSide}");
        }

        private void ReadWiderShipConfigFile()
        {
            var configPath = Path.Combine(Paths.ConfigPath, "mborsh.WiderShipMod.cfg");
            WiderShipExtendedSide = ConfigFileReader.ReadStringValue(
                configPath,
                "Extended Side = ",
                "Both"
            );
        }

        private void Initialize2StoryShipMod()
        {
            Is2StoryShipModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Constants.TwoStoryShipGUID);
            if (!Is2StoryShipModInstalled) return;

            try
            {
                var storyShipType = Type.GetType("2StoryShip");
                if (storyShipType != null)
                {
                    var rightField = storyShipType.GetField("EnableRightWindows",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var leftField = storyShipType.GetField("EnableLeftWindows",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                    if (rightField != null && leftField != null)
                    {
                        EnableRightWindows = (bool)rightField.GetValue(null);
                        EnableLeftWindows = (bool)leftField.GetValue(null);
                    }
                    else
                    {
                        Read2StoryShipConfigFile();
                    }
                }
                else
                {
                    Read2StoryShipConfigFile();
                }
            }
            catch
            {
                Read2StoryShipConfigFile();
            }

            Plugin.Log.LogInfo($"Detected 2StoryShipMod, RW - {EnableRightWindows}, LW - {EnableLeftWindows}");
        }

        private void Read2StoryShipConfigFile()
        {
            var configPath = Path.Combine(Paths.ConfigPath, "MelanieMelicious.2StoryShip.cfg");
            var values = ConfigFileReader.ReadMultipleValues(
                configPath,
                "Enable Right Windows = ",
                "Enable Left Windows = "
            );

            EnableRightWindows = values.TryGetValue("Enable Right Windows = ", out var rightValue)
                ? bool.Parse(rightValue)
                : true;
            EnableLeftWindows = values.TryGetValue("Enable Left Windows = ", out var leftValue)
                ? bool.Parse(leftValue)
                : true;
        }

        public void SetRandomSeed(int seed)
        {
            _rand = new System.Random(seed);
        }

        public Texture2D? GetCachedTexture(string filePath)
        {
            if (!Plugin.ModConfig.EnableTextureCaching.Value) return null;
            if (_textureCache.TryGetValue(filePath, out var texture))
            {
                Plugin.Log.LogDebug($"Retrieved cached texture: {PathUtils.GetPrettyPath(filePath)}");
                return texture;
            }
            return null;
        }

        public void CacheTexture(string filePath, Texture2D texture)
        {
            if (!Plugin.ModConfig.EnableTextureCaching.Value) return;
            if (!_textureCache.ContainsKey(filePath))
            {
                _textureCache[filePath] = texture;
            }
        }

        public string? GetCachedVideo(string filePath)
        {
            if (!Plugin.ModConfig.EnableTextureCaching.Value) return null;
            if (_videoCache.TryGetValue(filePath, out var videoPath))
            {
                Plugin.Log.LogDebug($"Retrieved cached video: {PathUtils.GetPrettyPath(filePath)}");
                return videoPath;
            }
            return null;
        }

        public void CacheVideo(string filePath)
        {
            if (!Plugin.ModConfig.EnableTextureCaching.Value) return;
            if (!_videoCache.ContainsKey(filePath))
            {
                if (!File.Exists(filePath))
                {
                    Plugin.Log.LogError($"Cannot cache video, file does not exist: {PathUtils.GetPrettyPath(filePath)}");
                    return;
                }
                _videoCache[filePath] = filePath;
                Plugin.Log.LogDebug($"Cached video: {PathUtils.GetPrettyPath(filePath)}");
            }
        }

        public void ClearCache()
        {
            foreach (var texture in _textureCache.Values)
            {
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }
            }
            _textureCache.Clear();
            _videoCache.Clear();
            Plugin.Log.LogInfo("Cleared texture and video cache");
        }

        public List<string> GetEnabledPackNames()
        {
            var enabledPacks = PosterFolders
            .Where(f => Plugin.ModConfig.IsPackEnabled(f))
            .Select(f => Path.GetFullPath(f).NormalizePath())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

            var packNames = enabledPacks.Select(Path.GetFileName).ToList();
            Plugin.Log.LogDebug($"Enabled pack names: {string.Join(", ", packNames)}");
            return enabledPacks;
        }

        public int GetFilePriority(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            int basePriority = extension switch
            {
                ".png" => 1,
                ".jpg" => 2,
                ".jpeg" => 3,
                ".bmp" => 4,
                ".mp4" => 5,
                _ => int.MaxValue
            };
            return basePriority * 1000 + Rand.Next(0, 1000);
        }
    }
}