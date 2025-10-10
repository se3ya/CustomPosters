using HarmonyLib;

namespace CustomPosters
{
    [HarmonyPatch]
    internal class GameLifecyclePatches
    {
        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        [HarmonyPostfix]
        private static void GameNetworkManagerStartPatch()
        {
            PosterManager.ResetSession();
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void StartPatch(StartOfRound __instance)
        {
            PosterManager.OnRoundStart(__instance);
        }

        [HarmonyPatch(typeof(GameNetworkManager), "StartHost")]
        [HarmonyPostfix]
        private static void StartHostPatch()
        {
            PosterManager.IsNewLobby = true;
        }

        [HarmonyPatch(typeof(GameNetworkManager), "JoinLobby")]
        [HarmonyPostfix]
        private static void JoinLobbyPatch()
        {
            PosterManager.IsNewLobby = true;
        }
    }
}