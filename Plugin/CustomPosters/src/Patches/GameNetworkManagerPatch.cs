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

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += PosterSyncManager.OnClientConnected;
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
