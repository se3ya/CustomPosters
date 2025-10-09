using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using UnityEngine;

namespace CustomPosters
{
    public class PosterService
    {
        private readonly List<string> _posterFolders = new List<string>();
        private readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        private readonly Dictionary<string, string> _videoCache = new Dictionary<string, string>(); //! Cache video file paths
        private readonly string[] _validExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" };
        private System.Random _rand = null!;
        public IReadOnlyList<string> PosterFolders => _posterFolders.AsReadOnly();

        public bool IsShipWindowsInstalled { get; private set; }
        public bool IsWindow2Enabled { get; private set; }
        public bool IsWiderShipModInstalled { get; private set; }
        public string WiderShipExtendedSide { get; private set; } = "Both";
        public bool Is2StoryShipModInstalled { get; private set; }
        public bool IsBiggerShipInstalled { get; private set; }
        public bool EnableRightWindows { get; private set; }
        public bool EnableLeftWindows { get; private set; }
        public Dictionary<string, bool> ShipWindowsStates { get; private set; } = new Dictionary<string, bool>();

        public System.Random Rand => _rand;

        public PosterService()
        {
            try
            {
                var pluginPath = Paths.PluginPath;
                var modFolderNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "CustomPosters",
                    "seechela-CustomPosters"
                };

                foreach (var folder in Directory.GetDirectories(pluginPath))
                {
                    var folderName = Path.GetFileName(folder);
                    if (folderName.Equals("plugins", StringComparison.OrdinalIgnoreCase) || modFolderNames.Contains(folderName))
                    {
                        continue;
                    }

                    var postersPath = Path.Combine(folder, "posters").Replace('\\', '/').ToLower();
                    var tipsPath = Path.Combine(folder, "tips").Replace('\\', '/').ToLower();
                    var customPostersFolder = Path.Combine(folder, "CustomPosters");
                    var nestedPostersPath = Path.Combine(customPostersFolder, "posters").Replace('\\', '/').ToLower();
                    var nestedTipsPath = Path.Combine(customPostersFolder, "tips").Replace('\\', '/').ToLower();

                    if (Directory.Exists(postersPath) || Directory.Exists(tipsPath) ||
                        Directory.Exists(nestedPostersPath) || Directory.Exists(nestedTipsPath))
                    {
                        if (Directory.Exists(folder))
                        {
                            var subDirs = Directory.GetDirectories(folder).Select(Path.GetFileName);
                        }
                        _posterFolders.Add(folder);
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to initialize PosterService: {ex.Message}");
            }

            InitializeShipWindows();
            InitializeWiderShipMod();
            Initialize2StoryShipMod();
            InitializeBiggerShipMod();
            SetRandomSeed(Environment.TickCount);
        }

        private void InitializeBiggerShipMod()
        {
            IsBiggerShipInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("AndreyMrovol.BiggerShip");
            if (IsBiggerShipInstalled)
            {
                Plugin.Log.LogInfo("BiggerShip mod detected. Applying compatibility logic.");
            }
        }

        private void InitializeShipWindows()
        {
            IsShipWindowsInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("TestAccount666.ShipWindows");
            if (!IsShipWindowsInstalled)
            {
                return;
            }

            var configPath = Path.Combine(Paths.ConfigPath, "TestAccount666.ShipWindows.cfg");
            if (!File.Exists(configPath))
            {
                Plugin.Log.LogWarning("ShipWindows config file not found");
                return;
            }

            try
            {
                var configLines = File.ReadAllLines(configPath);
                bool inRightWindowSection = false;
                foreach (var line in configLines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("[Right Window (SideRight)]"))
                    {
                        inRightWindowSection = true;
                        continue;
                    }
                    if (inRightWindowSection && trimmedLine.StartsWith("["))
                    {
                        inRightWindowSection = false;
                        continue;
                    }
                    if (inRightWindowSection && trimmedLine.StartsWith("1. Enabled = "))
                    {
                        if (bool.TryParse(trimmedLine.Replace("1. Enabled = ", "").Trim(), out bool enabled))
                        {
                            IsWindow2Enabled = enabled;
                            Plugin.Log.LogInfo($"ShipWindows Right Window Enabled - {IsWindow2Enabled}");
                            break;
                        }
                    }
                }
                if (!inRightWindowSection && !IsWindow2Enabled)
                {
                    Plugin.Log.LogDebug("Right Window section not found or disabled");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to read ShipWindows config: {ex.Message}");
            }
        }

        private void InitializeWiderShipMod()
        {
            IsWiderShipModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("mborsh.WiderShipMod");
            if (!IsWiderShipModInstalled)
            {
                return;
            }

            try
            {
                var widerShipType = Type.GetType("WiderShipMod");
                if (widerShipType == null)
                {
                    ReadWiderShipConfigFile();
                    return;
                }

                var extendedSideField = widerShipType.GetField("ExtendedSide", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (extendedSideField == null)
                {
                    ReadWiderShipConfigFile();
                    return;
                }

                WiderShipExtendedSide = (string)extendedSideField.GetValue(null);
                Plugin.Log.LogInfo($"WiderShipMod detected with Extended Side - {WiderShipExtendedSide}");
            }
            catch (Exception)
            {
                ReadWiderShipConfigFile();
            }
        }

        private void ReadWiderShipConfigFile()
        {
            var configPath = Path.Combine(Paths.ConfigPath, "mborsh.WiderShipMod.cfg");
            if (!File.Exists(configPath))
            {
                Plugin.Log.LogError("WiderShipMod config file not found, defaulting ExtendedSide to 'Both'");
                WiderShipExtendedSide = "Both";
                return;
            }

            try
            {
                var configLines = File.ReadAllLines(configPath);
                foreach (var line in configLines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("Extended Side = ", StringComparison.OrdinalIgnoreCase))
                    {
                        WiderShipExtendedSide = trimmedLine.Substring("Extended Side = ".Length).Trim();
                        Plugin.Log.LogInfo($"WiderShipMod detected with Extended Side - {WiderShipExtendedSide}");
                        return;
                    }
                }
                Plugin.Log.LogWarning("Extended Side not found in WiderShipMod config, defaulting to 'Both'");
                WiderShipExtendedSide = "Both";
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to read WiderShipMod config: {ex.Message}, defaulting ExtendedSide to 'Both'");
                WiderShipExtendedSide = "Both";
            }
        }

        private void Initialize2StoryShipMod()
        {
            Is2StoryShipModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("MelanieMelicious.2StoryShip");
            if (!Is2StoryShipModInstalled)
            {
                return;
            }

            try
            {
                var storyShipType = Type.GetType("2StoryShip");
                if (storyShipType == null)
                {
                    Read2StoryShipConfigFile();
                    return;
                }

                var rightWindowsField = storyShipType.GetField("EnableRightWindows", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var leftWindowsField = storyShipType.GetField("EnableLeftWindows", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (rightWindowsField == null || leftWindowsField == null)
                {
                    Plugin.Log.LogWarning("Failed to find EnableRightWindows or EnableLeftWindows fields in 2StoryShipConfig");
                    Read2StoryShipConfigFile();
                    return;
                }

                EnableRightWindows = (bool)rightWindowsField.GetValue(null);
                EnableLeftWindows = (bool)leftWindowsField.GetValue(null);
                Plugin.Log.LogInfo($"2StoryShipMod detected. RightWindows - {EnableRightWindows}, LeftWindows - {EnableLeftWindows}");
            }
            catch (Exception)
            {
                Read2StoryShipConfigFile();
            }
        }

        private void Read2StoryShipConfigFile()
        {
            var configPath = Path.Combine(Paths.ConfigPath, "MelanieMelicious.2StoryShip.cfg");
            if (!File.Exists(configPath))
            {
                Plugin.Log.LogError("2StoryShipMod config file not found, defaulting RightWindows and LeftWindows to true");
                EnableRightWindows = true;
                EnableLeftWindows = true;
                Plugin.Log.LogInfo($"2StoryShipMod detected. RightWindows - {EnableRightWindows}, LeftWindows - {EnableLeftWindows}");
                return;
            }

            try
            {
                var configLines = File.ReadAllLines(configPath);
                bool rightWindowsSet = false, leftWindowsSet = false;
                foreach (var line in configLines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("Enable Right Windows = ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (bool.TryParse(trimmedLine.Substring("Enable Right Windows = ".Length).Trim(), out bool rightEnabled))
                        {
                            EnableRightWindows = rightEnabled;
                            rightWindowsSet = true;
                        }
                    }
                    else if (trimmedLine.StartsWith("Enable Left Windows = ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (bool.TryParse(trimmedLine.Substring("Enable Left Windows = ".Length).Trim(), out bool leftEnabled))
                        {
                            EnableLeftWindows = leftEnabled;
                            leftWindowsSet = true;
                        }
                    }
                    if (rightWindowsSet && leftWindowsSet)
                        break;
                }

                if (!rightWindowsSet || !leftWindowsSet)
                {
                    Plugin.Log.LogWarning($"One or both window settings not found in 2StoryShipMod config, defaulting unset values to true");
                    if (!rightWindowsSet) EnableRightWindows = true;
                    if (!leftWindowsSet) EnableLeftWindows = true;
                }

                Plugin.Log.LogInfo($"2StoryShipMod detected. RightWindows - {EnableRightWindows}, LeftWindows - {EnableLeftWindows}");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to read 2StoryShipMod config: {ex.Message}, defaulting RightWindows and LeftWindows to true");
                EnableRightWindows = true;
                EnableLeftWindows = true;
                Plugin.Log.LogInfo($"2StoryShipMod detected. RightWindows - {EnableRightWindows}, LeftWindows - {EnableLeftWindows}");
            }
        }

        public void SetRandomSeed(int seed)
        {
            _rand = new System.Random(seed);
        }

        public Texture2D? GetCachedTexture(string filePath)
        {
            if (!PosterConfig.EnableTextureCaching.Value) return null;
            if (_textureCache.TryGetValue(filePath, out var texture))
            {
                Plugin.Log.LogDebug($"Retrieved cached texture: {filePath}");
                return texture;
            }
            return null;
        }

        public void CacheTexture(string filePath, Texture2D texture)
        {
            if (!PosterConfig.EnableTextureCaching.Value) return;
            if (!_textureCache.ContainsKey(filePath))
            {
                _textureCache[filePath] = texture;
            }
        }

        public string? GetCachedVideo(string filePath)
        {
            if (!PosterConfig.EnableTextureCaching.Value) return null;
            if (_videoCache.TryGetValue(filePath, out var videoPath))
            {
                Plugin.Log.LogDebug($"Retrieved cached video: {filePath}");
                return videoPath;
            }
            return null;
        }

        public void CacheVideo(string filePath)
        {
            if (!PosterConfig.EnableTextureCaching.Value) return;
            if (!_videoCache.ContainsKey(filePath))
            {
                if (!File.Exists(filePath))
                {
                    Plugin.Log.LogError($"Cannot cache video, file does not exist: {filePath}");
                    return;
                }
                _videoCache[filePath] = filePath;
                Plugin.Log.LogDebug($"Cached video: {filePath}");
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
            .Where(f => PosterConfig.IsPackEnabled(f))
            .Select(f => Path.GetFullPath(f).Replace('\\', '/'))
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