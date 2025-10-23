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
        [Serializable]
        public enum KeepFor { Lobby, Session, SaveSlot }

        public ConfigEntry<bool> EnableNetworking { get; private set; } = null!;
        public ConfigEntry<RandomizerMode> RandomizerModeSetting { get; private set; } = null!;
        public ConfigEntry<KeepFor> KeepPackFor { get; private set; } = null!;
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
                "Sync posters with all players (requires all to have the mod); false = client-side, makes compatible with vanilla lobbies."
            );

            RandomizerModeSetting = _configFile.Bind(
                "1. Settings",
                "RandomizerMode",
                RandomizerMode.PerPack,
                "Controls randomization: PerPack selects one pack for all posters; PerPoster randomizes each poster from enabled packs."
            );

            KeepPackFor = _configFile.Bind(
                "1. Settings",
                "Keep pack for",
                KeepFor.Lobby,
                "Keeping selection: Lobby = reroll each lobby; Session = until game restart; Save slot = per save file."
            );

            VanillaModelSelection = _configFile.Bind(
                "1. Settings",
                "Vanilla Model",
                VanillaModelOption.Both,
                new ConfigDescription(
                "Choose vanilla LC mesh: None (use quads), Poster5, Tips, or Both."
                )
            );

            EnableVideoAudio = _configFile.Bind(
                "1. Settings",
                "EnableVideoAudio",
                false,
                "Enable audio for .mp4 poster videos; disable to mute."
            );

            EnableTextureCaching = _configFile.Bind(
                "1. Settings",
                "EnableTextureCaching",
                false,
                "Cache textures/videos in memory to improve performance; disable to reduce memory usage."
            );

            int packCounter = 2;
            foreach (var packPath in Plugin.Service.PosterFolders)
            {
                try
                {
                    var packName = PackName(packPath);
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
                            $"Chance of selecting the {packName} pack in PerPack mode; 0 = equal probability with other packs.",
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
                                $"Chance of selecting poster '{fileName}' in PerPoster mode; 0 = equal probability with other posters.",
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
                                $"Aspect ratio for video '{fileName}': Stretch (fill), FitInside (no crop), FitOutside (may crop), NoScaling (original size)."
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

            Plugin.Log.LogInfo($"Found {_packEnabledEntries.Count} packs and {_fileConfigs.Count} poster files");
        }
        
        public bool UsePoster5VanillaModel =>
            VanillaModelSelection.Value.HasFlag(VanillaModelOption.Poster5);

        public bool UseTipsVanillaModel =>
            VanillaModelSelection.Value.HasFlag(VanillaModelOption.Tips);

        public bool IsPackEnabled(string packPath)
        {
            var packName = PackName(packPath);
            if (_packEnabledEntries.TryGetValue(packName, out var entry))
            {
                return entry.Value;
            }
            return true;
        }

        public int GetPackChance(string packPath)
        {
            var packName = PackName(packPath);
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

        private static string PackName(string packPath)
        {
            return PathUtils.GetDisplayPackName(packPath);
        }

        private static IEnumerable<string> GetFilesFromPack(string packPath)
        {
            var allFiles = new List<string>();

            // posters directory
            var postersPath = Path.Combine(packPath, "posters");
            if (Directory.Exists(postersPath))
            {
                allFiles.AddRange(
                    Directory.GetFiles(postersPath)
                        .Where(f => Constants.AllValidExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                );
            }

            // tips directory
            var tipsPath = Path.Combine(packPath, "tips");
            if (Directory.Exists(tipsPath))
            {
                allFiles.AddRange(
                    Directory.GetFiles(tipsPath)
                        .Where(f => Constants.AllValidExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                );
            }

            var customTips = Path.Combine(packPath, "CustomTips.png");
            if (File.Exists(customTips)) allFiles.Add(customTips);

            allFiles.AddRange(
                Directory.GetFiles(packPath)
                    .Where(f => Constants.AllValidExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
            );

            return allFiles.Distinct(StringComparer.OrdinalIgnoreCase).Select(f => Path.GetFullPath(f));
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