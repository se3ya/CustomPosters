using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace CustomPosters
{
    internal class PosterConfig
    {
        private static ManualLogSource Logger { get; set; }

        public static ConfigEntry<bool> PosterRandomizer { get; private set; }
        public static ConfigEntry<bool> LobbyRandom { get; private set; }

        private static ConfigFile configFile;

        public static void Init(ManualLogSource logger)
        {
            Logger = logger;

            configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "CustomPosters.cfg"), true);

            PosterRandomizer = configFile.Bind(
                "Settings", // Section
                "PosterRandomizer", // Key
                true, // Default value
                "If true, randomizes only poster packs. If false, combines all enabled packs and randomizes textures." // Description
            );

            foreach (var mod in Plugin.PosterFolders)
            {
                var startIdx = mod.IndexOf(@"plugins\", StringComparison.Ordinal);
                if (startIdx == -1)
                {
                    Logger.LogError($"Invalid mod folder path: 'plugins\\' not found in {mod}");
                    continue;
                }
                startIdx += @"plugins\".Length;

                var endIdx = mod.IndexOf(@"\CustomPosters", startIdx, StringComparison.Ordinal);
                if (endIdx == -1)
                {
                    Logger.LogError($"Invalid mod folder path: '\\CustomPosters' not found in {mod}");
                    continue;
                }

                var result = mod.Substring(startIdx, endIdx - startIdx);

                var conf = configFile.Bind(result, "Enabled", true, $"Enable or disable {result}");
            }
        }

        /// <summary>
        /// Checks if a specific pack is enabled in the configuration.
        /// </summary>
        /// <param name="folder">The folder path of the pack.</param>
        /// <returns>True if the pack is enabled, false otherwise.</returns>
        public static bool IsPackEnabled(string folder)
        {
            var startIdx = folder.IndexOf(@"plugins\", StringComparison.Ordinal);
            if (startIdx == -1) return false;

            startIdx += @"plugins\".Length;

            var endIdx = folder.IndexOf(@"\CustomPosters", startIdx, StringComparison.Ordinal);
            if (endIdx == -1) return false;

            var packName = folder.Substring(startIdx, endIdx - startIdx);

            // Check if the pack is enabled in the configuration
            return configFile.Bind(packName, "Enabled", true).Value;
        }
    }
}
