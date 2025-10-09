using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomPosters
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TestAccount666.ShipWindows", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MelanieMelicious.2StoryShip", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mborsh.WiderShipMod", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("AndreyMrovol.BiggerShip", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; } = null!;
        internal static ManualLogSource Log { get; private set; } = null!;
        public static PosterService Service { get; private set; } = null!;

        private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            Instance = this;

            Log = base.Logger;

            Log.LogInfo($"Initializing {MyPluginInfo.PLUGIN_NAME}");

            Service = new PosterService();

            PosterConfig.Initialize(Log, Config);

            Log.LogDebug("Applying patches");
            _harmony.PatchAll(typeof(GameLifecyclePatches));
            Log.LogInfo("Patches applied successfully");

            Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME} is loaded!");
        }
    }
}
