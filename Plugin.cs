using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;

namespace CustomPosters
{
    [BepInPlugin(Plugin.PLUGIN_GUID, Plugin.PLUGIN_NAME, Plugin.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "seeya.customposters";
        public const string PLUGIN_NAME = "CustomPosters";
        public const string PLUGIN_VERSION = "1.0.0";

        private void Awake()
        {
            try
            {
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

                PosterConfig.Init(Logger);
                CustomPosters.Patches.Init(Logger);

                var harmony = new Harmony(Plugin.PLUGIN_GUID);
                harmony.PatchAll(typeof(CustomPosters.Patches));

                Logger.LogInfo($"Plugin {Plugin.PLUGIN_NAME} ({Plugin.PLUGIN_VERSION}) is loaded!");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to initialize plugin: {ex.Message}");
            }
        }

        public static List<string> PosterFolders = new();
        public static readonly List<string> PosterFiles = new();
        public static readonly List<string> TipFiles = new();
        public static Random Rand = new();
    }
}