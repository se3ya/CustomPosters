using HarmonyLib;
using CustomPosters.Networking;
using Unity.Netcode;

namespace CustomPosters.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void OnStartPatch()
        {
            PosterManager.ResetSession();

            if (Plugin.ModConfig.EnableNetworking.Value)
            {
                if (NetworkManager.Singleton != null)
                {
                    Plugin.Log.LogInfo("Networking is enabled.");
                    NetworkManager.Singleton.OnClientConnectedCallback += PosterSyncManager.OnClientConnected;
                }
                else
                {
                    Plugin.Log.LogWarning("NetworkManager.Singleton is null, cannot register callbacks.");
                }
            }
            else
            {
                Plugin.Log.LogInfo("Networking is disabled.");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("StartHost")]
        private static void OnStartHostPatch()
        {
            PosterManager.IsNewLobby = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("JoinLobby")]
        private static void OnJoinLobbyPatch()
        {
            PosterManager.IsNewLobby = true;
        }
    }
}