using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using CustomPosters.Utils;

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

        public ConfigEntry<bool> EnableNetworking { get; private set; } = null!;
        public ConfigEntry<RandomizerMode> RandomizerModeSetting { get; private set; } = null!;
        public ConfigEntry<bool> PerSession { get; private set; } = null!;
        public ConfigEntry<bool> EnableTextureCaching { get; private set; } = null!;
        public ConfigEntry<bool> EnableVideoAudio { get; private set; } = null!;

        [Flags]
        public enum VanillaModelOption
        {
            None = 0,
            Poster5 = 1,
            Tips = 2,
            Both = Poster5 | Tips
        }

        public ConfigEntry<VanillaModelOption> VanillaModelSelection { get; private set; } = null!;
        
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

            EnableNetworking = _configFile.Bind(
                "1. Settings",
                "Enable Networking",
                true,
                "If true, posters are synced with all players (requires all players to have the mod).\n" +
                "If false, the mod is client-side only, allowing to play vanilla lobby."
            );

            RandomizerModeSetting = _configFile.Bind(
                "1. Settings",
                "RandomizerMode",
                RandomizerMode.PerPack,
                "Controls how textures are randomized.\n" +
                "PerPack: Selects one pack randomly for all posters.\n" +
                "PerPoster: Randomizes textures for each poster from all enabled packs."
            );

            PerSession = _configFile.Bind(
                "1. Settings",
                "PerSession",
                false,
                "When enabled, locks the randomization (PerPack or PerPoster) for the entire game session until the game is restarted.\n" +
                "When disabled, randomization refreshes each time the lobby reloads."
            );


            EnableTextureCaching = _configFile.Bind(
                "1. Settings",
                "EnableTextureCaching",
                false,
                "If true, caches textures and video paths in memory to improve performance.\n" +
                "Disable to reduce memory usage."
            );


            EnableVideoAudio = _configFile.Bind(
                "1. Settings",
                "EnableVideoAudio",
                false,
                "If true, enables audio playback for .mp4 poster videos.\n" +
                "Disable to mute videos."
            );

            VanillaModelSelection = _configFile.Bind(
                "1. Settings",
                "Vanilla Model",
                VanillaModelOption.Both,
                new ConfigDescription(
                    "Choose which posters use the extracted vanilla Lethal Company model.\n" +
                    "None: All posters use simple quad models.\n" +
                    "Poster5: Only Poster5 uses vanilla model.\n" +
                    "Tips: Only Tips poster uses vanilla model.\n" +
                    "Both: Both Poster5 and Tips use vanilla models."
                )
            );

            int packCounter = 2;
            foreach (var packPath in Plugin.Service.PosterFolders)
            {
                try
                {
                    var fullPackName = Path.GetFileName(packPath);
                    var packName = PackName(fullPackName);
                    if (string.IsNullOrEmpty(packName)) continue;

                    var mainPackSection = $"{packCounter}. {packName}";
                    var chancesPackSection = $"{packCounter}. {packName} - Chances";

                    var enabledEntry = _configFile.Bind(
                        mainPackSection,
                        "Enabled",
                        true,
                        $"Enable or disable the {packName} pack"
                    );
                    _packEnabledEntries[packName] = enabledEntry;

                    var chanceEntry = _configFile.Bind(
                        chancesPackSection,
                        "Global chance",
                        0,
                        new ConfigDescription(
                            $"Chance of selecting the {packName} pack in PerPack randomization mode.\n" +
                            $"Set to 0 to use equal probability with other packs.",
                            new AcceptableValueRange<int>(0, 100)
                        )
                    );
                    _packChanceEntries[packName] = chanceEntry;

                    var allFiles = GetFilesFromPack(packPath);
                    foreach (var filePath in allFiles)
                    {
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                        var fileExt = Path.GetExtension(filePath).TrimStart('.').ToUpper();
                        var fileName = Path.GetFileName(filePath);
                        var formattedKey = $"{fileNameWithoutExt}-{fileExt}";

                        var fileEnabledEntry = _configFile.Bind(
                            mainPackSection,
                            formattedKey,
                            true,
                            $"Enable or disable poster file '{fileName}' in pack '{packName}'"
                        );

                        var fileChanceEntry = _configFile.Bind(
                            chancesPackSection,
                            $"{formattedKey} Chance",
                            0,
                            new ConfigDescription(
                                $"Chance of selecting poster '{fileName}' in PerPoster mode.\n" +
                                $"Set to 0 to use equal probability with other posters.",
                                new AcceptableValueRange<int>(0, 100)
                            )
                        );

                        var fileConfig = new FileConfig(fileEnabledEntry, fileChanceEntry);

                        if (fileExt == "MP4")
                        {
                            fileConfig.Volume = _configFile.Bind(
                                mainPackSection,
                                $"{formattedKey} Volume",
                                Constants.DefaultVideoVolume,
                                new ConfigDescription(
                                    $"Volume for video '{fileName}'.",
                                    new AcceptableValueRange<int>(0, 100)
                                )
                            );

                            fileConfig.MaxDistance = _configFile.Bind(
                                mainPackSection,
                                $"{formattedKey} MaxDistance",
                                Constants.DefaultVideoMaxDistance,
                                new ConfigDescription(
                                    $"Maximum distance for audio playback of video '{fileName}'",
                                    new AcceptableValueRange<float>(1.0f, 5.0f)
                                )
                            );

                            fileConfig.AspectRatio = _configFile.Bind(
                                mainPackSection,
                                $"{formattedKey} AspectRatio",
                                VideoAspectRatio.Stretch,
                                $"Aspect ratio mode for video '{fileName}'.\n" +
                                $"Stretch: Stretches video to fit poster area.\n" +
                                $"FitInside: Fits video inside poster area without cropping.\n" +
                                $"FitOutside: Fits video outside poster area, may crop edges.\n" +
                                $"NoScaling: Uses original video size without scaling."
                            );
                        }

                        _fileConfigs[filePath] = fileConfig;
                    }
                    packCounter++;
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Failed to generate config for pack at {PathUtils.GetPrettyPath(packPath)}: {ex.Message}");
                }
            }

            ClearOrphanedEntries();
            _configFile.Save();
            _configFile.SaveOnConfigSet = true;

            Plugin.Log.LogInfo($"Configuration initialized with {_packEnabledEntries.Count} packs and {_fileConfigs.Count} files");
        }
        
        public bool UsePoster5VanillaModel =>
            VanillaModelSelection.Value.HasFlag(VanillaModelOption.Poster5);

        public bool UseTipsVanillaModel =>
            VanillaModelSelection.Value.HasFlag(VanillaModelOption.Tips);

        public bool IsPackEnabled(string packPath)
        {
            var packName = PackName(Path.GetFileName(packPath));
            if (_packEnabledEntries.TryGetValue(packName, out var entry))
            {
                return entry.Value;
            }
            return true;
        }

        public int GetPackChance(string packPath)
        {
            var packName = PackName(Path.GetFileName(packPath));
            if (_packChanceEntries.TryGetValue(packName, out var entry))
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
                int volume = config.Volume?.Value ?? 10;
                float maxDistance = config.MaxDistance?.Value ?? 3.5f;
                VideoAspectRatio aspectRatio = config.AspectRatio?.Value ?? VideoAspectRatio.Stretch;
                return (volume, maxDistance, aspectRatio);
            }
            return (10, 3.5f, VideoAspectRatio.Stretch);
        }

        private static string PackName(string fullPackName)
        {
            return PathUtils.GetPackName(fullPackName);
        }

        private static IEnumerable<string> GetFilesFromPack(string packPath)
        {
            var allFiles = new List<string>();
            var pathsToCheck = Constants.PosterPackSubdirectories
                .Select(subDir => Path.Combine(packPath, subDir));

            foreach (var path in pathsToCheck)
            {
                if (Directory.Exists(path))
                {
                    allFiles.AddRange(
                        Directory.GetFiles(path)
                            .Where(f => Constants.AllValidExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    );
                }
            }
            return allFiles.Distinct().Select(f => Path.GetFullPath(f));
        }
        
        private void ClearOrphanedEntries()
        {
            try
            {
                PropertyInfo? orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
                if (orphanedEntriesProp != null && 
                    orphanedEntriesProp.GetValue(_configFile) is Dictionary<ConfigDefinition, string> orphanedEntries)
                {
                    orphanedEntries.Clear();
                    Plugin.Log.LogDebug("Cleared orphaned config entries");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning($"Could not clear orphaned config entries: {ex.Message}");
            }
        }
    }
}