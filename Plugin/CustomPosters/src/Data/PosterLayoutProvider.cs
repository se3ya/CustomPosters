using CustomPosters.Data.PosterLayouts;

namespace CustomPosters.Data
{
    internal static class PosterLayoutProvider
    {
        public static PosterData[] GetLayout()
        {
            bool shipWindows = Plugin.Service.IsShipWindowsInstalled;
            bool window2 = Plugin.Service.IsWindow2Enabled;
            bool widerShip = Plugin.Service.IsWiderShipModInstalled;
            string? widerShipSide = Plugin.Service.WiderShipExtendedSide;
            bool twoStoryShip = Plugin.Service.Is2StoryShipModInstalled;

            if (Plugin.Service.IsBiggerShipInstalled)
            {
                Plugin.Log.LogInfo("Choosing layout: BiggerShip");
                return BiggerShip.Get();
            }

            if (twoStoryShip)
            {
                if (shipWindows && widerShip && !Plugin.Service.IsWindow2Enabled)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + ShipWindows + WiderShip  (No left window)");
                    return TwoStoryShip_ShipWindows_WiderShip_NoLeftWindow.Get();
                }

                if (shipWindows && widerShip)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + ShipWindows + WiderShip");
                    return TwoStoryShip_ShipWindows_WiderShip.Get();
                }

                if (widerShip && !Plugin.Service.EnableLeftWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + WiderShip (No left window)");
                    return TwoStoryShip_WiderShip_NoLeftWindow.Get();
                }

                if (shipWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + ShipWindows");
                    return TwoStoryShip_ShipWindows.Get();
                }

                if (widerShip)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship + WiderShip");
                    return TwoStoryShip_WiderShip.Get();
                }

                if (!Plugin.Service.EnableRightWindows && !Plugin.Service.EnableLeftWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (No both windows)");
                    return TwoStoryShip_NoBothWindows.Get();
                }

                if (Plugin.Service.EnableRightWindows && Plugin.Service.EnableLeftWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (All windows)");
                    return TwoStoryShip_AllWindows.Get();
                }
                if (!Plugin.Service.EnableRightWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (No right window)");
                    return TwoStoryShip_NoRightWindow.Get();
                }
                if (!Plugin.Service.EnableLeftWindows)
                {
                    Plugin.Log.LogInfo("Choosing layout: 2 Story Ship (No left window)");
                    return TwoStoryShip_NoLeftWindow.Get();
                }
            }

            if (shipWindows && window2 && widerShip && widerShipSide == "Left")
            {
                Plugin.Log.LogInfo("Choosing layout: ShipWindows + WiderShip (Left)");
                return ShipWindows_WiderShip_Left.Get();
            }

            if (widerShip)
            {
                Plugin.Log.LogInfo($"Choosing layout: WiderShip - {widerShipSide}");
                switch (widerShipSide)
                {
                    case "Both": return WiderShip_Both.Get();
                    case "Right": return WiderShip_Right.Get();
                    case "Left": return WiderShip_Left.Get();
                }
            }

            if (shipWindows && window2)
            {
                Plugin.Log.LogInfo("Choosing layout: ShipWindows");
                return ShipWindows.Get();
            }

            Plugin.Log.LogInfo("Choosing layout: Vanilla");
            return Vanilla.Get();
        }
    }
}