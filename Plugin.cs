using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomPosters
{
    [BepInPlugin(Plugin.PLUGIN_GUID, Plugin.PLUGIN_NAME, Plugin.PLUGIN_VERSION)]
    [BepInDependency("TestAccount666.ShipWindows", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("TestAccount666.ShipWindowsBeta", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MelanieMelicious.2StoryShip", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mborsh.WiderShipMod", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "seeya.customposters";
        public const string PLUGIN_NAME = "CustomPosters";
        public const string PLUGIN_VERSION = "1.4.0";

        // ShipWindows config
        public static bool IsShipWindowsInstalled { get; private set; }
        public static bool IsWindow2Enabled { get; private set; }

        // 2 Story Ship Mod config
        public static bool Is2StoryShipModInstalled { get; private set; }
        public static bool EnableRightWindows { get; private set; } = true;
        public static bool EnableLeftWindows { get; private set; } = true;

        // WiderShipMod config
        public static bool IsWiderShipModInstalled { get; private set; }
        public static string WiderShipExtendedSide { get; private set; }

        // Static reference to the logger
        public static ManualLogSource StaticLogger { get; private set; }

        private static void Check2StoryShipModConfig()
        {
            try
            {
                var configPath = Path.Combine(Paths.ConfigPath, "MelanieMelicious.2StoryShip.cfg");
                if (!File.Exists(configPath))
                {
                    StaticLogger.LogWarning("2 StoryShipMod config file not found. Using default window settings.");
                    return;
                }

                var configLines = File.ReadAllLines(configPath);
                foreach (var line in configLines)
                {
                    if (line.Contains("Enable Right Windows"))
                    {
                        EnableRightWindows = line.Contains("true");
                    }
                    else if (line.Contains("Enable Left Windows"))
                    {
                        EnableLeftWindows = line.Contains("true");
                    }
                }
            }
            catch (Exception ex)
            {
                StaticLogger.LogError($"Failed to read 2 StoryShipMod config: {ex.Message}");
            }
        }

        private void Awake()
        {
            try
            {
                StaticLogger = Logger;

                try
                {
                    // Search for CustomPosters directories excluding the plugins directory itself
                    PosterFolders = Directory.GetDirectories(Paths.PluginPath, "*", SearchOption.AllDirectories)
                        .Where(dir => dir.EndsWith(Plugin.PLUGIN_NAME, StringComparison.OrdinalIgnoreCase) &&
                                      !dir.Equals(Paths.PluginPath, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (PosterFolders.Count == 0)
                    {
                        Logger.LogDebug("No CustomPosters directories found");
                    }

                    foreach (var folder in PosterFolders)
                    {
                        string postersPath = Path.Combine(folder, "posters");
                        string tipsPath = Path.Combine(folder, "tips");

                        if (Directory.Exists(postersPath))
                        {
                            foreach (var file in Directory.GetFiles(postersPath))
                            {
                                if (Path.GetExtension(file) != ".old")
                                {
                                    PosterFiles.Add(file);
                                }
                            }
                        }

                        if (Directory.Exists(tipsPath))
                        {
                            foreach (var file in Directory.GetFiles(tipsPath))
                            {
                                if (Path.GetExtension(file) != ".old")
                                {
                                    TipFiles.Add(file);
                                }
                            }
                        }
                    }

                    // Check if 2 Story Ship Mod is installed using Chainloader
                    Is2StoryShipModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos
                        .ContainsKey("MelanieMelicious.2StoryShip");

                    if (Is2StoryShipModInstalled)
                    {
                        StaticLogger.LogInfo("2 Story Ship Mod detected.");
                        Check2StoryShipModConfig(); // Read its config
                    }

                    // Check if ShipWindows or ShipWindowsBeta is installed using Chainloader
                    IsShipWindowsInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos
                        .ContainsKey("TestAccount666.ShipWindows") ||
                        BepInEx.Bootstrap.Chainloader.PluginInfos
                        .ContainsKey("TestAccount666.ShipWindowsBeta");

                    if (IsShipWindowsInstalled)
                    {
                        StaticLogger.LogInfo("ShipWindows or ShipWindowsBeta detected.");
                        IsWindow2Enabled = CheckIfWindow2Enabled();
                    }

                    // Check if WiderShipMod is installed
                    IsWiderShipModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos
                        .ContainsKey("mborsh.WiderShipMod");

                    if (IsWiderShipModInstalled)
                    {
                        StaticLogger.LogInfo("WiderShipMod detected.");
                        WiderShipExtendedSide = GetWiderShipExtendedSide();
                        StaticLogger.LogInfo($"WiderShipMod Extended Side: {WiderShipExtendedSide}");
                    }

                    PosterConfig.Init(Logger);
                    CustomPosters.Patches.Init(Logger);

                    var harmony = new Harmony(Plugin.PLUGIN_GUID);
                    harmony.PatchAll(typeof(CustomPosters.Patches));

                    StaticLogger.LogInfo($"Plugin {Plugin.PLUGIN_NAME} ({Plugin.PLUGIN_VERSION}) is loaded!");
                }
                catch (Exception ex)
                {
                    StaticLogger.LogError($"Failed to initialize plugin: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                StaticLogger.LogError($"Failed to initialize plugin: {ex.Message}");
            }
        }

        public static List<string> PosterFolders = new();
        public static readonly List<string> PosterFiles = new();
        public static readonly List<string> TipFiles = new();
        public static Random Rand = new();

        /// <summary>
        /// Checks if window2 (Right Window in ShipWindowsBeta) is enabled in ShipWindows' config.
        /// </summary>
        private static bool CheckIfWindow2Enabled()
        {
            try
            {
                var betaInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("TestAccount666.ShipWindowsBeta");
                var shipWindowsBetaConfigPath = Path.Combine(Paths.ConfigPath, "TestAccount666.ShipWindowsBeta.cfg");

                if (betaInstalled && File.Exists(shipWindowsBetaConfigPath))
                {
                    StaticLogger.LogInfo("Found ShipWindowsBeta cfg");
                    var configLines = File.ReadAllLines(shipWindowsBetaConfigPath);
                    bool inRightWindowSection = false;

                    foreach (var line in configLines)
                    {
                        var trimmedLine = line.Trim();
                        if (trimmedLine.StartsWith("[Right Window (SideRight)]"))
                        {
                            inRightWindowSection = true;
                            continue;
                        }
                        if (inRightWindowSection && trimmedLine.StartsWith("[") && !trimmedLine.StartsWith("[Right Window (SideRight)]"))
                        {
                            inRightWindowSection = false;
                            continue;
                        }
                        if (inRightWindowSection && trimmedLine.Contains("1. Enabled") && trimmedLine.Contains("true"))
                        {
                            StaticLogger.LogInfo("ShipWindowsBeta - Right Window is enabled");
                            return true;
                        }
                    }
                    StaticLogger.LogInfo("ShipWindowsBeta - Right Window is disabled");
                    return false;
                }

                var regularInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("TestAccount666.ShipWindows");
                var shipWindowsConfigPath = Path.Combine(Paths.ConfigPath, "TestAccount666.ShipWindows.cfg");

                if (regularInstalled && File.Exists(shipWindowsConfigPath))
                {
                    StaticLogger.LogInfo("Found ShipWindows config file");
                    var configLines = File.ReadAllLines(shipWindowsConfigPath);
                    foreach (var line in configLines)
                    {
                        if (line.Contains("EnableWindow2") && line.Contains("true"))
                        {
                            StaticLogger.LogInfo("ShipWindows Window 2 is enabled");
                            return true;
                        }
                    }
                    StaticLogger.LogInfo("ShipWindows Window 2 is disabled");
                    return false;
                }

                StaticLogger.LogWarning("ShipWindows is installed but no relevant config file found.");
                return false;
            }
            catch (Exception ex)
            {
                StaticLogger.LogError($"Error checking Window2 config: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reads the Extended Side value from WiderShipMod's config.
        /// </summary>
        private static string GetWiderShipExtendedSide()
        {
            try
            {
                var widerShipConfigPath = Path.Combine(Paths.ConfigPath, "mborsh.WiderShipMod.cfg");
                if (!File.Exists(widerShipConfigPath))
                {
                    StaticLogger.LogWarning("WiderShipMod config file not found. Assuming Extended Side is 'Both'.");
                    return "Both";
                }

                var configLines = File.ReadAllLines(widerShipConfigPath);
                foreach (var line in configLines)
                {
                    if (line.Contains("Extended Side"))
                    {
                        if (line.Contains("Both")) return "Both";
                        if (line.Contains("Right")) return "Right";
                        if (line.Contains("Left")) return "Left";
                    }
                }
            }
            catch (Exception ex)
            {
                StaticLogger.LogError($"Failed to read WiderShipMod config: {ex.Message}");
            }
            return "Both";
        }
    }
}
