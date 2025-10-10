using UnityEngine;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class TwoStoryShip_WiderShip
    {
        public static PosterData[] Get() => new PosterData[]
        {
            new() { Position = new Vector3(6.3172f, 2.9407f, -22.4766f), Rotation = new Vector3(0, 180f, 0), Scale = new Vector3(0.6391f, 0.4882f, 2f), Name = "Poster1" },
            new() { Position = new Vector3(9.5975f, 2.5063f, -5.8245f), Rotation = new Vector3(0, 0, 0), Scale = new Vector3(0.7296f, 0.4896f, 1f), Name = "Poster2" },
            new() { Position = new Vector3(10.1364f, 2.8591f, -22.4788f), Rotation = new Vector3(0, 180f, 356.3345f), Scale = new Vector3(0.7487f, 1.0539f, 1f), Name = "Poster3" },
            new() { Position = new Vector3(5.3599f, 2.5963f, -9.455f), Rotation = new Vector3(0, 307.2657f, 2.68f), Scale = new Vector3(0.7289f, 0.9989f, 1f), Name = "Poster4" },
            new() { Position = new Vector3(7.5475f, 2.5882f, -22.4803f), Rotation = new Vector3(0, 180f, 359.8f), Scale = new Vector3(0.5516f, 0.769f, 1f), Name = "Poster5" },
            new() { Position = new Vector3(-5.8111f, 2.541f, -17.577f), Rotation = new Vector3(0, 270.0942f, 358.6752f), Scale = new Vector3(0.8596f, 1.2194f, 1f), Name = "CustomTips" }
        };
    }
}