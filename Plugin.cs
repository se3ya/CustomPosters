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
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "seeya.customposters";
        public const string PLUGIN_NAME = "CustomPosters";
        public const string PLUGIN_VERSION = "1.0.0";

        // shipWindows config
        public static bool IsShipWindowsInstalled { get; private set; }
        public static bool IsWindow2Enabled { get; private set; }

        // widerShipMod config
        public static bool IsWiderShipModInstalled { get; private set; }
        public static string WiderShipExtendedSide { get; private set; }

        public static ManualLogSource StaticLogger { get; private set; }

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

                IsShipWindowsInstalled = CheckIfShipWindowsInstalled();
                if (IsShipWindowsInstalled)
                {
                    StaticLogger.LogInfo("ShipWindows detected.");
                    IsWindow2Enabled = CheckIfWindow2Enabled();
                }

                IsWiderShipModInstalled = CheckIfWiderShipModInstalled();
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

        public static List<string> PosterFolders = new();
        public static readonly List<string> PosterFiles = new();
        public static readonly List<string> TipFiles = new();
        public static Random Rand = new();

        private static bool CheckIfShipWindowsInstalled()
        {
            foreach (var folder in Directory.GetDirectories(Paths.PluginPath))
            {
                if (folder.Contains("ShipWindows"))
                {
                    // Check if the ShipWindows DLL exists (ignore .old files)
                    var dllFiles = Directory.GetFiles(folder, "*.dll");
                    foreach (var dllFile in dllFiles)
                    {
                        if (!dllFile.EndsWith(".old"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool CheckIfWindow2Enabled()
        {
            try
            {
                var shipWindowsConfigPath = Path.Combine(Paths.ConfigPath, "TestAccount666.ShipWindows.cfg");
                if (!File.Exists(shipWindowsConfigPath))
                {
                    StaticLogger.LogWarning("ShipWindows config file not found. Assuming Window2 is disabled.");
                    return false;
                }

                var configLines = File.ReadAllLines(shipWindowsConfigPath);
                foreach (var line in configLines)
                {
                    if (line.Contains("EnableWindow2") && line.Contains("true"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticLogger.LogError($"Failed to read ShipWindows config: {ex.Message}");
            }
            return false;
        }

        private static bool CheckIfWiderShipModInstalled()
        {
            foreach (var folder in Directory.GetDirectories(Paths.PluginPath))
            {
                if (folder.Contains("mborsh-Wider_Ship_Mod"))
                {
                    // Check if the WiderShipMod DLL exists (ignore .old files)
                    var dllFiles = Directory.GetFiles(folder, "*.dll");
                    foreach (var dllFile in dllFiles)
                    {
                        if (dllFile.EndsWith("WiderShipMod.dll") && !dllFile.EndsWith(".old"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

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
            return "Both"; // default to Both if config is missing
        }
    }
}
