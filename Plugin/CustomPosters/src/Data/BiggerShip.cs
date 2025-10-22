using UnityEngine;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class BiggerShip
    {
        public static PosterData[] Get()
        {
            bool usePoster5Vanilla = Plugin.ModConfig.UsePoster5VanillaModel.Value;
            bool useTipsVanilla = Plugin.ModConfig.UseTipsVanillaModel.Value;

            return new PosterData[]
            {
                new()
                {
                    Position = new Vector3(4.1886f, 2.8918f, -17.8606f),
                    Rotation = new Vector3(0, 169.6929f, 0),
                    Scale = new Vector3(0.6391f, 0.4882f, 2f),
                    Name = "Poster1"
                },
                new()
                {
                    Position = new Vector3(6.4202f, 2.4776f, -10.9064f),
                    Rotation = new Vector3(0, 0, 0),
                    Scale = new Vector3(0.7296f, 0.4896f, 1f),
                    Name = "Poster2"
                },
                new()
                {
                    Position = new Vector3(9.9186f, 2.8591f, -17.5587f),
                    Rotation = new Vector3(0, 180f, 356.3345f),
                    Scale = new Vector3(0.7487f, 1.0539f, 1f),
                    Name = "Poster3"
                },
                new()
                {
                    Position = new Vector3(5.2187f, 2.5963f, -10.782f),
                    Rotation = new Vector3(0, 10.7715f, 2.68f),
                    Scale = new Vector3(0.7289f, 0.9989f, 1f),
                    Name = "Poster4"
                },

                usePoster5Vanilla
                    ? new()
                    {
                         // Vanilla model
                        Position = new Vector3(5.5286f, 2.5882f, -17.6077f),
                        Rotation = new Vector3(1.3609f, 329.1297f, 182.4321f),
                        Scale = new Vector3(0.465f, 0.71f, 1f),
                        Name = "Poster5"
                    }
                    : new()
                    {
                        // Quad
                        Position = new Vector3(5.5286f, 2.5882f, -17.6184f),
                        Rotation = new Vector3(0, 169.7645f, 359.8f),
                        Scale = new Vector3(0.5516f, 0.769f, 1f),
                        Name = "Poster5"
                    },

                useTipsVanilla
                    ? new()
                    {
                        // Vanilla model
                        Position = new Vector3(8.1417f, 2.3991f, -20.696f),
                        Rotation = new Vector3(89.6078f, 134.6205f, 314.8865f),
                        Scale = new Vector3(46.75954f, 100f, 70.89838f),
                        Name = "CustomTips"
                    }
                    : new()
                    {
                        // Quad
                        Position = new Vector3(3.0647f, 2.8174f, -10.4842f),
                        Rotation = new Vector3(0, 0, 0),
                        Scale = new Vector3(0.8596f, 1.2194f, 1f),
                        Name = "CustomTips"
                    }
            };
        }
    }
}