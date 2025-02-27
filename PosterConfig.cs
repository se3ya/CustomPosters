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
                try
                {
                    // get the mod folder name skipping if its the plugins directory itself
                    var modName = Path.GetFileName(Path.GetDirectoryName(mod));
                    if (string.IsNullOrEmpty(modName) || modName.Equals("plugins", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var conf = configFile.Bind(modName, "Enabled", true, $"Enable or disable {modName}");
                }
                catch (Exception ex)
                {
                    Logger.LogDebug($"Failed to parse mod path {mod}: {ex.Message}");
                    continue;
                }
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

            // check if the pack is enabled in the configuration
            return configFile.Bind(packName, "Enabled", true).Value;
        }
    }
}
