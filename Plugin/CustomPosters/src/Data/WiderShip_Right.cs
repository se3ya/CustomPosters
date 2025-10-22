using UnityEngine;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class WiderShip_Right
    {
        public static PosterData[] Get()
        {
            bool usePoster5Vanilla = Plugin.ModConfig.UsePoster5VanillaModel.Value;
            bool useTipsVanilla = Plugin.ModConfig.UseTipsVanillaModel.Value;

            return new PosterData[]
            {
                new()
                {
                    Position = new Vector3(4.1886f, 2.9318f, -16.8409f),
                    Rotation = new Vector3(0, 200.9872f, 0),
                    Scale = new Vector3(0.6391f, 0.4882f, 2f),
                    Name = "Poster1"
                },
                new()
                {
                    Position = new Vector3(6.4202f, 2.4776f, -10.8226f),
                    Rotation = new Vector3(0, 0, 0),
                    Scale = new Vector3(0.7296f, 0.4896f, 1f),
                    Name = "Poster2"
                },
                new()
                {
                    Position = new Vector3(9.9186f, 2.8591f, -17.4716f),
                    Rotation = new Vector3(0, 180f, 356.3345f),
                    Scale = new Vector3(0.7487f, 1.0539f, 1f),
                    Name = "Poster3"
                },
                new()
                {
                    Position = new Vector3(5.5699f, 2.5963f, -10.3268f),
                    Rotation = new Vector3(0, 62.0324f, 2.6799f),
                    Scale = new Vector3(0.7289f, 0.9989f, 1f),
                    Name = "Poster4"
                },

                usePoster5Vanilla
                    ? new()
                    {
                        // Vanilla model
                        Position = new Vector3(5.5286f, 2.5882f, -17.3421f),
                        Rotation = new Vector3(1.3609f, 0.2388f, 182.4321f),
                        Scale = new Vector3(0.465f, 0.71f, 1f),
                        Name = "Poster5"
                    }
                    : new()
                    {
                        // Quad
                        Position = new Vector3(5.5286f, 2.5882f, -17.3541f),
                        Rotation = new Vector3(0, 200.9099f, 359.8f),
                        Scale = new Vector3(0.5516f, 0.769f, 1f),
                        Name = "Poster5"
                    },

                useTipsVanilla
                    ? new()
                    {
                        // Vanilla model
                        Position = new Vector3(7.7417f, 2.4171f, -16.9514f),
                        Rotation = new Vector3(90f, 180f, 0),
                        Scale = new Vector3(46.75954f, 100f, 70.89838f),
                        Name = "CustomTips"
                    }
                    : new()
                    {
                        // Quad
                        Position = new Vector3(3.0947f, 2.8174f, -6.7252f),
                        Rotation = new Vector3(0, 0, 358.6752f),
                        Scale = new Vector3(0.8596f, 1.2194f, 1f),
                        Name = "CustomTips"
                    }
            };
        }
    }
}