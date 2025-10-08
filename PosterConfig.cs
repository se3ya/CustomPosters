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

        public static ConfigEntry<RandomizerMode> RandomizerModeSetting { get; set; } = null!;
        public static ConfigEntry<bool> PerSession { get; set; } = null!;
        public static ConfigEntry<bool> EnableTextureCaching { get; set; } = null!;
        public static ConfigEntry<bool> EnableVideoAudio { get; set; } = null!;

        private static ConfigFile configFile = null!;

        [Serializable]
        public enum VideoAspectRatio
        {
            Stretch,
            FitInside,
            FitOutside,
            NoScaling
        }

        public static void Initialize(ManualLogSource logger, ConfigFile config)
        {
            configFile = config;
            configFile.SaveOnConfigSet = false;

            MigrateOldConfigEntries(logger, configFile.ConfigFilePath);

            RandomizerModeSetting = configFile.Bind(
                "Settings",
                "RandomizerMode",
                RandomizerMode.PerPack,
                "Controls how textures are randomized. PerPack: Selects one pack randomly for all posters. PerPoster: Randomizes textures for each poster from all enabled packs."
            );

            PerSession = configFile.Bind(
                "Settings",
                "PerSession",
                false,
                "When enabled, locks the randomization (PerPack or PerPoster) for the entire game session until the game is restarted. When disabled, randomization refreshes each time the lobby reloads."
            );

            EnableTextureCaching = configFile.Bind(
                "Settings",
                "EnableTextureCaching",
                false,
                "If true, caches textures and video paths in memory to improve performance. Disable to reduce memory usage."
            );

            EnableVideoAudio = configFile.Bind(
                "Settings",
                "EnableVideoAudio",
                false,
                "If true, enables audio playback for .mp4 poster videos. Disable to mute videos."
            );

            var modFolderNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "CustomPosters",
                "seechela-CustomPosters"
            };

            foreach (var mod in Plugin.Service.PosterFolders)
            {
                try
                {
                    var modName = Path.GetFileName(mod);
                    if (string.IsNullOrEmpty(modName) || modName.Equals("plugins", StringComparison.OrdinalIgnoreCase) || modFolderNames.Contains(modName))
                    {
                        continue;
                    }

                    configFile.Bind(modName, "Enabled", true, $"Enable or disable the {modName} pack");
                    configFile.Bind(
                        modName,
                        "Chance",
                        0,
                        new ConfigDescription(
                            $"Chance of selecting the {modName} pack in PerPack randomization mode [0-100]. Set to 0 to use equal probability with other packs.",
                            new AcceptableValueRange<int>(0, 100)
                        )
                    );

                    var postersPath = Path.Combine(mod, "posters");
                    var tipsPath = Path.Combine(mod, "tips");
                    var customPostersFolder = Path.Combine(mod, "CustomPosters");
                    var nestedPostersPath = Path.Combine(customPostersFolder, "posters");
                    var nestedTipsPath = Path.Combine(customPostersFolder, "tips");

                    var posterPaths = new[] { postersPath, nestedPostersPath }.Where(Directory.Exists);
                    foreach (var path in posterPaths)
                    {
                        var posterFiles = Directory.GetFiles(path)
                            .Where(f => new[] { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" }.Contains(Path.GetExtension(f).ToLower()))
                            .ToList();
                        foreach (var file in posterFiles)
                        {
                            var fileName = Path.GetFileName(file);
                            configFile.Bind(modName, fileName, true, $"Enable or disable poster file {fileName} in pack {modName}");
                            configFile.Bind(
                                modName,
                                $"{fileName}.Chance",
                                0,
                                new ConfigDescription(
                                    $"Chance of selecting poster {fileName} in PerPoster randomization mode [0-100]. Set to 0 to use equal probability with other posters.",
                                    new AcceptableValueRange<int>(0, 100)
                                )
                            );
                            if (Path.GetExtension(file).ToLower() == ".mp4")
                            {
                                configFile.Bind(
                                    modName,
                                    $"{fileName}.Volume",
                                    30,
                                    new ConfigDescription(
                                        $"Volume for video {fileName} (0-100).",
                                        new AcceptableValueRange<int>(0, 100)
                                    )
                                );
                                configFile.Bind(
                                    modName,
                                    $"{fileName}.MaxDistance",
                                    4.0f,
                                    new ConfigDescription(
                                        $"Maximum distance for audio playback of video {fileName} (1.0-5.0).",
                                        new AcceptableValueRange<float>(1.0f, 5.0f)
                                    )
                                );
                                configFile.Bind(
                                    modName,
                                    $"{fileName}.AspectRatio",
                                    VideoAspectRatio.Stretch,
                                    $"Aspect ratio mode for video {fileName}. Options: Stretch, FitInside, FitOutside, NoScaling."
                                );
                            }
                        }
                    }

                    var tipPaths = new[] { tipsPath, nestedTipsPath }.Where(Directory.Exists);
                    foreach (var path in tipPaths)
                    {
                        var tipFiles = Directory.GetFiles(path)
                            .Where(f => new[] { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" }.Contains(Path.GetExtension(f).ToLower()))
                            .ToList();
                        foreach (var file in tipFiles)
                        {
                            var fileName = Path.GetFileName(file);
                            configFile.Bind(modName, fileName, true, $"Enable or disable tip file {fileName} in pack {modName}");
                            configFile.Bind(
                                modName,
                                $"{fileName}.Chance",
                                0,
                                new ConfigDescription(
                                    $"Chance of selecting tip {fileName} in PerPoster randomization mode [0-100]. Set to 0 to use equal probability with other tips.",
                                    new AcceptableValueRange<int>(0, 100)
                                )
                            );
                            if (Path.GetExtension(file).ToLower() == ".mp4")
                            {
                                configFile.Bind(
                                    modName,
                                    $"{fileName}.Volume",
                                    30,
                                    new ConfigDescription(
                                        $"Volume for video {fileName} (0-100).",
                                        new AcceptableValueRange<int>(0, 100)
                                    )
                                );
                                configFile.Bind(
                                    modName,
                                    $"{fileName}.MaxDistance",
                                    4.0f,
                                    new ConfigDescription(
                                        $"Maximum distance for audio playback of video {fileName} (1.0-5.0).",
                                        new AcceptableValueRange<float>(1.0f, 5.0f)
                                    )
                                );
                                configFile.Bind(
                                    modName,
                                    $"{fileName}.AspectRatio",
                                    VideoAspectRatio.Stretch,
                                    $"Aspect ratio mode for video {fileName}. Options: Stretch, FitInside, FitOutside, NoScaling."
                                );
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to parse mod path {mod}: {ex.Message}");
                }
            }

            ClearOrphanedEntries();
            configFile.Save();
            configFile.SaveOnConfigSet = true;
        }

        private static string? GetModNameFromFilePath(string filePath)
        {
            string normalizedFilePath = Path.GetFullPath(filePath).Replace('\\', '/').ToLower();

            string? mod = Plugin.Service.PosterFolders.FirstOrDefault(f =>
            {
                string normalizedPackPath = Path.GetFullPath(f).Replace('\\', '/').ToLower();
                string normalizedCustomPostersPath = Path.Combine(normalizedPackPath, "CustomPosters").Replace('\\', '/');
                return normalizedFilePath.Contains(normalizedPackPath) || normalizedFilePath.Contains(normalizedCustomPostersPath);
            });

            return mod != null ? Path.GetFileName(mod) : null;
        }

        public static (int volume, float maxDistance, VideoAspectRatio aspectRatio) GetFileAudioSettings(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            
            string? modName = GetModNameFromFilePath(filePath);

            if (modName == null)
            {
                return (30, 4.0f, VideoAspectRatio.Stretch);
            }

            int volume = configFile.Bind(
                modName,
                $"{fileName}.Volume",
                30,
                new ConfigDescription("", new AcceptableValueRange<int>(0, 100))
            ).Value;
            float maxDistance = configFile.Bind(
                modName,
                $"{fileName}.MaxDistance",
                4.0f,
                new ConfigDescription("", new AcceptableValueRange<float>(1.0f, 5.0f))
            ).Value;
            VideoAspectRatio aspectRatio = configFile.Bind(
                modName,
                $"{fileName}.AspectRatio",
                VideoAspectRatio.Stretch
            ).Value;
            return (volume, maxDistance, aspectRatio);
        }

        private static void MigrateOldConfigEntries(ManualLogSource logger, string configPath)
        {
            if (!File.Exists(configPath)) return;

            try
            {
                var config = new ConfigFile(configPath, false);
                var entriesToMigrate = new List<(ConfigDefinition oldDef, ConfigDefinition newDef, bool value)>();

                foreach (var entry in config)
                {
                    var section = entry.Key.Section;
                    var key = entry.Key.Key;

                    if (key.StartsWith("Poster_", StringComparison.OrdinalIgnoreCase) || key.StartsWith("Tip_", StringComparison.OrdinalIgnoreCase))
                    {
                        var newKey = key.StartsWith("Poster_") ? key.Substring(7) : key.Substring(4);
                        var oldDef = new ConfigDefinition(section, key);
                        var newDef = new ConfigDefinition(section, newKey);
                        var value = (bool)entry.Value.BoxedValue;
                        entriesToMigrate.Add((oldDef, newDef, value));
                    }
                }

                foreach (var (oldDef, newDef, value) in entriesToMigrate)
                {
                    config.Bind(newDef, value, new ConfigDescription($"Migrated from {oldDef.Key}"));
                    config.Remove(oldDef);
                }

                if (entriesToMigrate.Count > 0)
                {
                    config.Save();
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to migrate old config entries: {ex.Message}");
            }
        }

        public static bool IsPackEnabled(string packPath)
        {
            string modName = Path.GetFileName(packPath);
            if (string.IsNullOrEmpty(modName))
            {
                return false;
            }
            bool isEnabled = configFile.Bind(modName, "Enabled", true).Value;
            Plugin.Log.LogDebug($"Pack {modName} enabled - {isEnabled}");
            return isEnabled;
        }

        public static int GetPackChance(string packPath)
        {
            string modName = Path.GetFileName(packPath);
            if (string.IsNullOrEmpty(modName))
            {
                return 0;
            }
            int chance = configFile.Bind(modName, "Chance", 0, new ConfigDescription("", new AcceptableValueRange<int>(0, 100))).Value;
            return chance;
        }

        public static bool IsFileEnabled(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            
            string? modName = GetModNameFromFilePath(filePath);

            if (modName == null)
            {
                return false;
            }

            bool isPackEnabled = configFile.Bind(modName, "Enabled", true).Value;
            if (!isPackEnabled)
            {
                return false;
            }

            return configFile.Bind(modName, fileName, true).Value;
        }

        public static int GetFileChance(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            string? modName = GetModNameFromFilePath(filePath);

            if (modName == null)
            {
                return 0;
            }

            return configFile.Bind(
                modName,
                $"{fileName}.Chance",
                0,
                new ConfigDescription("", new AcceptableValueRange<int>(0, 100))
            ).Value;
        }

        private static void ClearOrphanedEntries()
        {
            PropertyInfo? orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
            if (orphanedEntriesProp != null)
            {
                var orphanedEntries = (Dictionary<ConfigDefinition, string>?)orphanedEntriesProp.GetValue(configFile);
                orphanedEntries?.Clear();
            }
        }
    }
}