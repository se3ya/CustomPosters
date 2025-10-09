using UnityEngine;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class TwoStoryShip_ShipWindows
    {
        public static PosterData[] Get() => new PosterData[]
        {
            new() { Position = new Vector3(6.5923f, 2.9318f, -17.4766f), Rotation = new Vector3(0, 179.2201f, 0), Scale = new Vector3(0.6391f, 0.4882f, 2f), Name = "Poster1" },
            new() { Position = new Vector3(9.0884f, 2.4776f, -8.8229f), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(0.7296f, 0.4896f, 1f), Name = "Poster2" },
            new() { Position = new Vector3(9.9186f, 2.8591f, -17.4716f), Rotation = new Vector3(0, 180f, 356.3345f), Scale = new Vector3(0.7487f, 1.0539f, 1f), Name = "Poster3" },
            new() { Position = new Vector3(5.3599f, 2.5963f, -9.455f), Rotation = new Vector3(0, 307.2657f, 2.68f), Scale = new Vector3(0.7289f, 0.9989f, 1f), Name = "Poster4" },
            new() { Position = new Vector3(10.2813f, 2.7482f, -8.8271f), Rotation = new Vector3(0, 0.9014f, 359.8f), Scale = new Vector3(0.5516f, 0.769f, 1f), Name = "Poster5" },
            new() { Position = new Vector3(2.5679f, 2.6763f, -11.7341f), Rotation = new Vector3(0, 0, 358.6752f), Scale = new Vector3(0.8596f, 1.2194f, 1f), Name = "CustomTips" }
        };
    }
}