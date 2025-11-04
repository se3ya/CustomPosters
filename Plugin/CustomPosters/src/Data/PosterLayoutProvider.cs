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
                if (twoStoryLegacy)
                {
                    if (widerShip)
                    {
                        Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (Legacy) + WiderShip");
                        return PosterLayouts.Legacy.TwoStoryShip_WiderShip.Get();
                    }
                }

                if (shipWindows && widerShip && !rightWindow)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + ShipWindows + WiderShip  (No right window)");
                    return TwoStoryShip_WiderShip.Get();
                }

                if (shipWindows && widerShip)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + ShipWindows + WiderShip");
                    return TwoStoryShip_WiderShip.Get();
                }

                if (widerShip && !enableLeftWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + WiderShip (No left window)");
                    return TwoStoryShip_WiderShip.Get();
                }

                if (shipWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + ShipWindows");
                    return TwoStoryShip.Get();
                }

                if (widerShip)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + WiderShip");
                    return TwoStoryShip_WiderShip.Get();
                }

                if (!enableRightWindows && !enableLeftWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (No both windows)");
                    return TwoStoryShip.Get();
                }

                if (enableRightWindows && enableLeftWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (All windows)");
                    return TwoStoryShip.Get();
                }
                if (!enableRightWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (No right window)");
                    return TwoStoryShip.Get();
                }
                if (!enableLeftWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (No left window)");
                    return TwoStoryShip.Get();
                }
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
    }
}
