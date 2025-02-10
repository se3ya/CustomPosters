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

        public static void Init(ManualLogSource logger)
        {
            Logger = logger;

            var configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "CustomPosters.cfg"), true);

            foreach (var mod in Plugin.PosterFolders)
            {
                var startIdx = mod.IndexOf(@"plugins\", StringComparison.Ordinal);
                if (startIdx == -1)
                {
                    Logger.LogError($"Invalid mod folder path: 'plugins\\' not found in {mod}");
                    continue; // Skip this folder
                }
                startIdx += @"plugins\".Length;

                var endIdx = mod.IndexOf(@"\CustomPosters", startIdx, StringComparison.Ordinal);
                if (endIdx == -1)
                {
                    Logger.LogError($"Invalid mod folder path: '\\CustomPosters' not found in {mod}");
                    continue; // Skip this folder
                }

                var result = mod.Substring(startIdx, endIdx - startIdx);

                var conf = configFile.Bind(result, "Enabled", true, $"Enable or disable {result}");
                if (!conf.Value)
                {
                    try
                    {
                        string disabledPath = mod + ".Disabled";
                        if (!Directory.Exists(disabledPath))
                        {
                            Directory.Move(mod, disabledPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to disable {result}: {ex.Message}");
                    }
                }
            }
        }
    }
}