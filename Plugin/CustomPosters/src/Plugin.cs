using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomPosters
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TestAccount666.ShipWindows", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MelanieMelicious.2StoryShip", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mborsh.WiderShipMod", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; } = null!;
        internal static ManualLogSource Log { get; private set; } = null!;
        public static PosterService Service { get; private set; } = null!;
        public static PosterConfig ModConfig { get; private set; } = null!;

        private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            Instance = this;

            Log = base.Logger;

            Log.LogInfo($"Initializing {MyPluginInfo.PLUGIN_NAME}");

            AssetManager.LoadAssets();  

            ModConfig = new PosterConfig(Config);
            Service = new PosterService();
            
            ModConfig.Initialize();

            Log.LogDebug("Applying patches");
            _harmony.PatchAll(typeof(GameLifecyclePatches));
            Log.LogDebug("Patches applied successfully");

            Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME} is loaded!");
        }
    }
}
