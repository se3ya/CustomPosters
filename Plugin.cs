using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomPosters
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TestAccount666.ShipWindows", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MelanieMelicious.2StoryShip", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mborsh.WiderShipMod", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; } = null!;
        public static ManualLogSource Log => Instance.Logger;
        public static PosterService Service { get; private set; } = null!;

        private readonly Harmony _harmony = new(PluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            Instance = this;
            Log.LogInfo($"Initializing {PluginInfo.PLUGIN_NAME}");

            Service = new PosterService();

            PosterConfig.Initialize(Log);

            Log.LogDebug("Applying patches");
            _harmony.PatchAll(typeof(Patches));
            Log.LogInfo("Patches applied successfully");

            Log.LogInfo($"{PluginInfo.PLUGIN_NAME} is loaded!");
        }

        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "seeya.customposters";
            public const string PLUGIN_NAME = "CustomPosters";
            public const string PLUGIN_VERSION = "2.1.0";
        }
    }
}
