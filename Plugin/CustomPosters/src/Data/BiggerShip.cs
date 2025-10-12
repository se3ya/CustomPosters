using UnityEngine;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class BiggerShip
    {
        public static PosterData[] Get() => new PosterData[]
        {
            new() { Position = new Vector3(4.1886f, 2.8918f, -17.8606f), Rotation = new Vector3(0, 169.6929f, 0), Scale = new Vector3(0.6391f, 0.4882f, 2f), Name = "Poster1" },
            new() { Position = new Vector3(7.1953f, 2.4776f, -10.9097f), Rotation = Vector3.zero, Scale = new Vector3(0.7296f, 0.4896f, 1f), Name = "Poster2" },
            new() { Position = new Vector3(9.9186f, 2.8591f, -17.5587f), Rotation = new Vector3(0, 180f, 00), Scale = new Vector3(0.7487f, 1.0539f, 1f), Name = "Poster3" },
            new() { Position = new Vector3(6.3833f, 2.5963f, -10.9072f), Rotation = new Vector3(0, 0, 2.68f), Scale = new Vector3(0.7289f, 0.9989f, 1f), Name = "Poster4" },
            new() { Position = new Vector3(5.5286f, 2.5882f, -17.6184f), Rotation = new Vector3(0, 169.7645f, 359.8f), Scale = new Vector3(0.5516f, 0.769f, 1f), Name = "Poster5" },
            new() { Position = new Vector3(2.7781f, 2.8174f, -10.4842f), Rotation = Vector3.zero, Scale = new Vector3(0.8596f, 1.2194f, 1f), Name = "CustomTips" }
        };
    }
}