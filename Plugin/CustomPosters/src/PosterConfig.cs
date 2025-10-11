using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;

namespace CustomPosters
{
    internal class FileConfig
    {
        public ConfigEntry<bool> Enabled { get; }
        public ConfigEntry<int> Chance { get; }
        public ConfigEntry<int>? Volume { get; set; }
        public ConfigEntry<float>? MaxDistance { get; set; }
        public ConfigEntry<PosterConfig.VideoAspectRatio>? AspectRatio { get; set; }

        public FileConfig(ConfigEntry<bool> enabled, ConfigEntry<int> chance)
        {
            Enabled = enabled;
            Chance = chance;
        }
    }

    public class PosterConfig
    {
        [Serializable]
        public enum RandomizerMode { PerPack, PerPoster }
        [Serializable]
        public enum VideoAspectRatio { Stretch, FitInside, FitOutside, NoScaling }

        public ConfigEntry<RandomizerMode> RandomizerModeSetting { get; private set; } = null!;
        public ConfigEntry<bool> PerSession { get; private set; } = null!;
        public ConfigEntry<bool> EnableTextureCaching { get; private set; } = null!;
        public ConfigEntry<bool> EnableVideoAudio { get; private set; } = null!;
        
        private readonly Dictionary<string, ConfigEntry<bool>> _packEnabledEntries = new();
        private readonly Dictionary<string, ConfigEntry<int>> _packChanceEntries = new();
        private readonly Dictionary<string, FileConfig> _fileConfigs = new();
        
        private readonly ConfigFile _configFile;

        public PosterConfig(ConfigFile config)
        {
            _configFile = config;
        }
        
        public void Initialize()
        {
            _configFile.SaveOnConfigSet = false;

            RandomizerModeSetting = _configFile.Bind("1. Settings", "RandomizerMode", RandomizerMode.PerPack, "Controls how textures are randomized. PerPack: Selects one pack randomly for all posters. PerPoster: Randomizes textures for each poster from all enabled packs.");
            PerSession = _configFile.Bind("1. Settings", "PerSession", false, "When enabled, locks the randomization (PerPack or PerPoster) for the entire game session until the game is restarted. When disabled, randomization refreshes each time the lobby reloads.");
            EnableTextureCaching = _configFile.Bind("1. Settings", "EnableTextureCaching", false, "If true, caches textures and video paths in memory to improve performance. Disable to reduce memory usage.");
            EnableVideoAudio = _configFile.Bind("1. Settings", "EnableVideoAudio", false, "If true, enables audio playback for .mp4 poster videos. Disable to mute videos.");

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

                    var enabledEntry = _configFile.Bind(mainPackSection, "Enabled", true, $"Enable or disable the {cleanPackName} pack");
                    _packEnabledEntries[cleanPackName] = enabledEntry;

                    var chanceEntry = _configFile.Bind(chancesPackSection, "Global chance", 0, new ConfigDescription($"Chance of selecting the {cleanPackName} pack in PerPack randomization mode [0-100]. Set to 0 to use equal probability with other packs", new AcceptableValueRange<int>(0, 100)));
                    _packChanceEntries[cleanPackName] = chanceEntry;
                    
                    var allFiles = GetFilesFromPack(packPath);
                    foreach (var filePath in allFiles)
                    {
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                        var fileExt = Path.GetExtension(filePath).TrimStart('.').ToUpper();
                        var originalFileName = Path.GetFileName(filePath);
                        var formattedKey = $"{fileNameWithoutExt}-{fileExt}";

                        var fileEnabledEntry = _configFile.Bind(mainPackSection, formattedKey, true, $"Enable or disable poster file {originalFileName} in pack {cleanPackName}");
                        var fileChanceEntry = _configFile.Bind(chancesPackSection, $"{formattedKey} Chance", 0, new ConfigDescription($"Chance of selecting poster {originalFileName} in PerPoster randomization mode [0-100]. Set to 0 to use equal probability with other posters.", new AcceptableValueRange<int>(0, 100)));

                        var fileConfig = new FileConfig(fileEnabledEntry, fileChanceEntry);

                        if (fileExt == "MP4")
                        {
                            fileConfig.Volume = _configFile.Bind(mainPackSection, $"{formattedKey} Volume", 20, new ConfigDescription($"Volume for video {originalFileName} (0-100).", new AcceptableValueRange<int>(0, 100)));
                            fileConfig.MaxDistance = _configFile.Bind(mainPackSection, $"{formattedKey} MaxDistance", 4.0f, new ConfigDescription($"Maximum distance for audio playback of video {originalFileName} (1.0-5.0)", new AcceptableValueRange<float>(1.0f, 5.0f)));
                            fileConfig.AspectRatio = _configFile.Bind(mainPackSection, $"{formattedKey} AspectRatio", VideoAspectRatio.Stretch, $"Aspect ratio mode for video {originalFileName}. [Stretch] - Stretches video to fit poster area. [FitInside] - Fits video inside poster area without cropping. [FitOutside] - Fits video outside poster area, cropping if necessary. [NoScaling] - Uses original video size without scaling.");
                        }
                        
                        _fileConfigs[filePath] = fileConfig;
                    }
                    packCounter++;
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Failed to generate config for pack at {packPath}: {ex.Message}");
                }
            }
            
            ClearOrphanedEntries();
            _configFile.Save();
            _configFile.SaveOnConfigSet = true;
        }

        public bool IsPackEnabled(string packPath)
        {
            var cleanPackName = CleanPackName(Path.GetFileName(packPath));
            if (_packEnabledEntries.TryGetValue(cleanPackName, out var entry))
            {
                return entry.Value;
            }
            return true;
        }

        public int GetPackChance(string packPath)
        {
            var cleanPackName = CleanPackName(Path.GetFileName(packPath));
            if (_packChanceEntries.TryGetValue(cleanPackName, out var entry))
            {
                return entry.Value;
            }
            return 0;
        }
        
        public bool IsFileEnabled(string filePath)
        {
            if (_fileConfigs.TryGetValue(Path.GetFullPath(filePath), out var config))
            {
                return config.Enabled.Value;
            }
            return true;
        }

        public int GetFileChance(string filePath)
        {
            if (_fileConfigs.TryGetValue(Path.GetFullPath(filePath), out var config))
            {
                return config.Chance.Value;
            }
            return 0;
        }

        public (int volume, float maxDistance, VideoAspectRatio aspectRatio) GetFileAudioSettings(string filePath)
        {
            if (_fileConfigs.TryGetValue(Path.GetFullPath(filePath), out var config))
            {
                int volume = config.Volume?.Value ?? 20;
                float maxDistance = config.MaxDistance?.Value ?? 4.0f;
                VideoAspectRatio aspectRatio = config.AspectRatio?.Value ?? VideoAspectRatio.Stretch;
                return (volume, maxDistance, aspectRatio);
            }
            return (20, 4.0f, VideoAspectRatio.Stretch);
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
            return allFiles.Distinct().Select(f => Path.GetFullPath(f));
        }
        
        private void ClearOrphanedEntries()
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