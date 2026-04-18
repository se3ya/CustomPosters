using CustomPosters.Data.PosterLayouts;

namespace CustomPosters.Data
{
    internal static class PosterLayoutProvider
    {
        public static PosterData[] GetLayout()
        {
            bool shipWindows = Plugin.Service.IsShipWindowsInstalled;
            bool rightWindow = Plugin.Service.IsRightWindowEnabled;
            bool widerShip = Plugin.Service.IsWiderShipModInstalled;
            string? widerShipSide = Plugin.Service.WiderShipExtendedSide;
            bool twoStoryShip = Plugin.Service.Is2StoryShipModInstalled;
            bool enableRightWindows = Plugin.Service.EnableRightWindows;
            bool enableLeftWindows = Plugin.Service.EnableLeftWindows;
            bool twoStoryLegacy = twoStoryShip && string.Equals(Plugin.Service.TwoStoryShipLayout, "Legacy", System.StringComparison.OrdinalIgnoreCase);

            if (Plugin.Service.IsBiggerShipInstalled)
            {
                Plugin.Log.LogInfo("Choosing layout: BiggerShip");
                return BiggerShip.Get();
            }

            if (twoStoryShip)
            {
                return GetTwoStoryLayout(twoStoryLegacy, widerShip, shipWindows, rightWindow, enableRightWindows, enableLeftWindows);
            }

            if (shipWindows && rightWindow && widerShip && widerShipSide == "Left")
            {
                Plugin.Log.LogInfo("Choosing layout: ShipWindows + WiderShip (Left)");
                return ShipWindows_WiderShip_Left.Get();
            }

            if (widerShip)
            {
                Plugin.Log.LogInfo($"Choosing layout: WiderShip - {widerShipSide}");
                switch (widerShipSide)
                {
                    case Constants.WiderShipSideBoth: return WiderShip_Both.Get();
                    case Constants.WiderShipSideRight: return WiderShip_Right.Get();
                    case Constants.WiderShipSideLeft: return WiderShip_Left.Get();
                }
            }

            if (shipWindows && rightWindow)
            {
                Plugin.Log.LogInfo("Choosing layout: ShipWindows");
                return ShipWindows.Get();
            }

            Plugin.Log.LogInfo("Choosing layout: Vanilla");
            return Vanilla.Get();
        }
        private static PosterData[] GetTwoStoryLayout(
            bool legacy, bool widerShip, bool shipWindows, bool rightWindow,
            bool enableRightWindows, bool enableLeftWindows)
        {
            if (legacy && widerShip)
            {
                Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (Legacy) + WiderShip");
                return PosterLayouts.Legacy.TwoStoryShip_WiderShip.Get();
            }

            if (widerShip)
            {
                Plugin.Log.LogInfo($"Choosing layout: 2 Story Ship + WiderShip{GetWiderShipDetail(shipWindows, rightWindow, enableLeftWindows)}");
                return TwoStoryShip_WiderShip.Get();
            }

            Plugin.Log.LogInfo($"Choosing layout: 2 Story Ship{GetTwoStorySuffix(shipWindows, enableRightWindows, enableLeftWindows)}");
            return TwoStoryShip.Get();
        }

        private static string GetWiderShipDetail(bool shipWindows, bool rightWindow, bool enableLeftWindows)
        {
            if (shipWindows && rightWindow) return " + ShipWindows";
            if (shipWindows) return " + ShipWindows (No right window)";
            if (!enableLeftWindows) return " (No left window)";
            return "";
        }

        private static string GetTwoStorySuffix(bool shipWindows, bool enableRightWindows, bool enableLeftWindows)
        {
            if (shipWindows) return " + ShipWindows";
            if (!enableRightWindows && !enableLeftWindows) return " (No both windows)";
            if (enableRightWindows && enableLeftWindows) return " (All windows)";
            if (!enableRightWindows) return " (No right window)";
            return " (No left window)";
        }
    }
}
