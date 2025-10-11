using HarmonyLib;

namespace CustomPosters.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        private static void OnRoundStartPatch(StartOfRound __instance)
        {
            PosterManager.OnRoundStart(__instance);
        }
    }
}
