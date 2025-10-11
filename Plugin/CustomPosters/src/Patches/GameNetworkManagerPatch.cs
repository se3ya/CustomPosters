using HarmonyLib;

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
