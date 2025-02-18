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
        public const string PLUGIN_VERSION = "1.3.5";

        // ShipWindows config
        public static bool IsShipWindowsInstalled { get; private set; }
        public static bool IsWindow2Enabled { get; private set; }

        // 2 Story Ship Mod config
        public static bool Is2StoryShipModInstalled { get; private set; }
        public static bool EnableRightWindows { get; private set; } = true; // Default to true
        public static bool EnableLeftWindows { get; private set; } = true;  // Default to true

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
                    StaticLogger.LogWarning("2 Story Ship Mod config file not found. Using default window settings.");
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
                StaticLogger.LogError($"Failed to read 2 Story Ship Mod config: {ex.Message}");
            }
        }

        private void Awake()
        {
            try
            {
                StaticLogger = Logger;

                PosterFolders = Directory.GetDirectories(Paths.PluginPath, Plugin.PLUGIN_NAME, SearchOption.AllDirectories).ToList();

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

                // Check if WiderShipMod is installed using Chainloader
                IsWiderShipModInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos
                    .ContainsKey("mborsh.WiderShipMod");

                if (IsWiderShipModInstalled)
                {
                    StaticLogger.LogInfo("WiderShipMod detected.");
                    WiderShipExtendedSide = GetWiderShipExtendedSide();
                    StaticLogger.LogInfo($"WiderShipMod Extended Side: {WiderShipExtendedSide}");
                }

                // Initialize config and patches
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

        public static List<string> PosterFolders = new();
        public static readonly List<string> PosterFiles = new();
        public static readonly List<string> TipFiles = new();
        public static Random Rand = new();

        /// <summary>
        /// Checks if window2 is enabled in ShipWindows' config.
        /// </summary>
        private static bool CheckIfWindow2Enabled()
        {
            try
            {
                // Check ShipWindows config
                var shipWindowsConfigPath = Path.Combine(Paths.ConfigPath, "TestAccount666.ShipWindows.cfg");
                if (File.Exists(shipWindowsConfigPath))
                {
                    var configLines = File.ReadAllLines(shipWindowsConfigPath);
                    foreach (var line in configLines)
                    {
                        if (line.Contains("EnableWindow2") && line.Contains("true"))
                        {
                            return true;
                        }
                    }
                }

                // Check ShipWindowsBeta config
                var shipWindowsBetaConfigPath = Path.Combine(Paths.ConfigPath, "TestAccount666.ShipWindowsBeta.cfg");
                if (File.Exists(shipWindowsBetaConfigPath))
                {
                    var configLines = File.ReadAllLines(shipWindowsBetaConfigPath);
                    bool isInLeftWindowSection = false;

                    foreach (var line in configLines)
                    {
                        // Check if we're in the [Left Window (SideLeft)] section
                        if (line.Trim().StartsWith("[") && line.Trim().EndsWith("]"))
                        {
                            isInLeftWindowSection = line.Trim().Equals("[Left Window (SideLeft)]", StringComparison.OrdinalIgnoreCase);
                        }

                        // If we're in the [Left Window (SideLeft)] section, check for the "Enabled" key
                        if (isInLeftWindowSection && line.Trim().StartsWith("1. Enabled", StringComparison.OrdinalIgnoreCase))
                        {
                            // Extract the value after the equals sign
                            var parts = line.Split('=');
                            if (parts.Length > 1 && parts[1].Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }
                    }
                }

                StaticLogger.LogWarning("ShipWindows or ShipWindowsBeta config file not found. Assuming Window 2 is disabled.");
                return false;
            }
            catch (Exception ex)
            {
                StaticLogger.LogError($"Failed to read ShipWindows or ShipWindowsBeta config: {ex.Message}");
            }
            return false;
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
            return "Both"; // Default to "Both" if config is missing or invalid
        }
    }
}
