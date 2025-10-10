using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomPosters
{
    internal class PosterConfig
    {
        [Serializable]
        public enum RandomizerMode
        {
            PerPack,
            PerPoster
        }

        [Serializable]
        public enum VideoAspectRatio
        {
            Stretch,
            FitInside,
            FitOutside,
            NoScaling
        }

        public static ConfigEntry<RandomizerMode> RandomizerModeSetting { get; private set; } = null!;
        public static ConfigEntry<bool> PerSession { get; private set; } = null!;
        public static ConfigEntry<bool> EnableTextureCaching { get; private set; } = null!;
        public static ConfigEntry<bool> EnableVideoAudio { get; private set; } = null!;

        private static ConfigFile _configFile = null!;
        private static readonly Dictionary<string, string> _filePathToCleanPackNameCache = new Dictionary<string, string>();

        public static void Initialize(ManualLogSource logger, ConfigFile config)
        {
            _configFile = config;
            _configFile.SaveOnConfigSet = false;

            RandomizerModeSetting = _configFile.Bind(
                "1. Settings",
                "RandomizerMode",
                RandomizerMode.PerPack,
                "Controls how textures are randomized. PerPack: Selects one pack randomly for all posters. PerPoster: Randomizes textures for each poster from all enabled packs."
            );

            PerSession = _configFile.Bind(
                "1. Settings",
                "PerSession",
                false,
                "When enabled, locks the randomization (PerPack or PerPoster) for the entire game session until the game is restarted. When disabled, randomization refreshes each time the lobby reloads."
            );

            EnableTextureCaching = _configFile.Bind(
                "1. Settings",
                "EnableTextureCaching",
                false,
                "If true, caches textures and video paths in memory to improve performance. Disable to reduce memory usage."
            );

            EnableVideoAudio = _configFile.Bind(
                "1. Settings",
                "EnableVideoAudio",
                false,
                "If true, enables audio playback for .mp4 poster videos. Disable to mute videos."
            );

            int packCounter = 2;

            foreach (var packPath in Plugin.Service.PosterFolders)
            {
                try
                {
                    var fullPackName = Path.GetFileName(packPath);
                    var cleanPackName = CleanPackName(fullPackName);
                    if (string.IsNullOrEmpty(cleanPackName)) continue;

                    var mainPackSection = $"{packCounter}. {cleanPackName}";
                    var chancesPackSection = $"{packCounter}. {cleanPackName} - Chances";

                    _configFile.Bind(mainPackSection, "Enabled", true, $"Enable or disable the {cleanPackName} pack");
                    _configFile.Bind(chancesPackSection, "Global chance", 0, new ConfigDescription($"Chance of selecting the {cleanPackName} pack in PerPack randomization mode [0-100]. Set to 0 to use equal probability with other packs.", new AcceptableValueRange<int>(0, 100)));

                    var allFiles = GetFilesFromPack(packPath);
                    foreach (var filePath in allFiles)
                    {
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                        var fileExt = Path.GetExtension(filePath).TrimStart('.').ToUpper();
                        var originalFileName = Path.GetFileName(filePath);

                        var formattedKey = $"{fileNameWithoutExt}-{fileExt}";

                        _configFile.Bind(mainPackSection, formattedKey, true, $"Enable or disable poster file {originalFileName} in pack {cleanPackName}");

                        _configFile.Bind(chancesPackSection, $"{formattedKey} Chance", 0, new ConfigDescription($"Chance of selecting poster {originalFileName} in PerPoster randomization mode [0-100]. Set to 0 to use equal probability with other posters.", new AcceptableValueRange<int>(0, 100)));

                        if (fileExt == "MP4")
                        {
                            _configFile.Bind(mainPackSection, $"{formattedKey} Volume", 20, new ConfigDescription($"Volume for video {originalFileName} (0-100).", new AcceptableValueRange<int>(0, 100)));
                            _configFile.Bind(mainPackSection, $"{formattedKey} MaxDistance", 4.0f, new ConfigDescription($"Maximum distance for audio playback of video {originalFileName} (1.0-5.0).", new AcceptableValueRange<float>(1.0f, 5.0f)));
                            _configFile.Bind(mainPackSection, $"{formattedKey} AspectRatio", VideoAspectRatio.Stretch, $"Aspect ratio mode for video {originalFileName}.");
                        }
                    }
                    packCounter++;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to generate config for pack at {packPath}: {ex.Message}");
                }
            }

            ClearOrphanedEntries();
            _configFile.Save();
            _configFile.SaveOnConfigSet = true;
        }

        private static string CleanPackName(string fullPackName)
        {
            int dashIndex = fullPackName.IndexOf('-');
            if (dashIndex > 0 && dashIndex < fullPackName.Length - 1)
            {
                return fullPackName.Substring(dashIndex + 1);
            }
            return fullPackName;
        }

        private static IEnumerable<string> GetFilesFromPack(string packPath)
        {
            var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" };
            var allFiles = new List<string>();

            var pathsToCheck = new[] { "posters", "tips", "CustomPosters/posters", "CustomPosters/tips" }
                .Select(subDir => Path.Combine(packPath, subDir));

            foreach (var path in pathsToCheck)
            {
                if (Directory.Exists(path))
                {
                    allFiles.AddRange(Directory.GetFiles(path).Where(f => validExtensions.Contains(Path.GetExtension(f).ToLower())));
                }
            }
            return allFiles.Distinct();
        }

        private static string? GetCleanPackNameFromFilePath(string filePath)
        {
            if (_filePathToCleanPackNameCache.TryGetValue(filePath, out var cachedName))
            {
                return cachedName;
            }

            string normalizedFilePath = Path.GetFullPath(filePath).Replace('\\', '/');
            string? foundPackPath = Plugin.Service.PosterFolders.FirstOrDefault(packPath =>
            {
                string normalizedPackPath = Path.GetFullPath(packPath).Replace('\\', '/');
                return normalizedFilePath.StartsWith(normalizedPackPath, StringComparison.OrdinalIgnoreCase);
            });

            if (foundPackPath != null)
            {
                var cleanName = CleanPackName(Path.GetFileName(foundPackPath));
                _filePathToCleanPackNameCache[filePath] = cleanName;
                return cleanName;
            }

            return null;
        }

        public static bool IsPackEnabled(string packPath)
        {
            var cleanPackName = CleanPackName(Path.GetFileName(packPath));
            if (string.IsNullOrEmpty(cleanPackName)) return false;
            
            return _configFile.Bind(cleanPackName, "Enabled", true).Value;
        }

        public static int GetPackChance(string packPath)
        {
            var cleanPackName = CleanPackName(Path.GetFileName(packPath));
            if (string.IsNullOrEmpty(cleanPackName)) return 0;

            var chancesPackSection = $"{cleanPackName} - Chances";
            return _configFile.Bind(chancesPackSection, "Global chance", 0).Value;
        }

        public static bool IsFileEnabled(string filePath)
        {
            var cleanPackName = GetCleanPackNameFromFilePath(filePath);
            if (string.IsNullOrEmpty(cleanPackName)) return false;
            
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var fileExt = Path.GetExtension(filePath).TrimStart('.').ToUpper();
            var formattedKey = $"{fileNameWithoutExt}-{fileExt}";

            return _configFile.Bind(cleanPackName, formattedKey, true).Value;
        }

        public static int GetFileChance(string filePath)
        {
            var cleanPackName = GetCleanPackNameFromFilePath(filePath);
            if (string.IsNullOrEmpty(cleanPackName)) return 0;

            var chancesPackSection = $"{cleanPackName} - Chances";
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var fileExt = Path.GetExtension(filePath).TrimStart('.').ToUpper();
            var formattedKey = $"{fileNameWithoutExt}-{fileExt}";

            return _configFile.Bind(chancesPackSection, $"{formattedKey} - Chance", 0).Value;
        }

        public static (int volume, float maxDistance, VideoAspectRatio aspectRatio) GetFileAudioSettings(string filePath)
        {
            var cleanPackName = GetCleanPackNameFromFilePath(filePath);
            if (string.IsNullOrEmpty(cleanPackName))
            {
                return (20, 4.0f, VideoAspectRatio.Stretch);
            }

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var fileExt = Path.GetExtension(filePath).TrimStart('.').ToUpper();
            var formattedKey = $"{fileNameWithoutExt}-{fileExt}";

            int volume = _configFile.Bind(cleanPackName, $"{formattedKey} - Volume", 20).Value;
            float maxDistance = _configFile.Bind(cleanPackName, $"{formattedKey} - MaxDistance", 4.0f).Value;
            VideoAspectRatio aspectRatio = _configFile.Bind(cleanPackName, $"{formattedKey} - AspectRatio", VideoAspectRatio.Stretch).Value;
            
            return (volume, maxDistance, aspectRatio);
        }

        private static void ClearOrphanedEntries()
        {
            try
            {
                PropertyInfo? orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
                if (orphanedEntriesProp != null && orphanedEntriesProp.GetValue(_configFile) is Dictionary<ConfigDefinition, string> orphanedEntries)
                {
                    orphanedEntries.Clear();
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning($"Could not clear orphaned config entries: {ex.Message}");
            }
        }
    }
}