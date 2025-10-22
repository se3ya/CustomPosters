using UnityEngine;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class TwoStoryShip_WiderShip
    {
        public static PosterData[] Get()
        {
            bool usePoster5Vanilla = Plugin.ModConfig.UsePoster5VanillaModel.Value;
            bool useTipsVanilla = Plugin.ModConfig.UseTipsVanillaModel.Value;

            return new PosterData[]
            {
                new()
                {
                    Position = new Vector3(5.4012f, 2.9407f, -18.9355f),
                    Rotation = new Vector3(0, 231.1166f, 0),
                    Scale = new Vector3(0.6391f, 0.4882f, 2f),
                    Name = "Poster1"
                },
                new()
                {
                    Position = new Vector3(5.9419f, 2.4776f, -6.5299f),
                    Rotation = new Vector3(0, 270f, 0),
                    Scale = new Vector3(0.7296f, 0.4896f, 1f),
                    Name = "Poster2"
                },
                new()
                {
                    Position = new Vector3(10.1364f, 2.8591f, -22.4808f),
                    Rotation = new Vector3(0, 180f, 356.3345f),
                    Scale = new Vector3(0.7487f, 1.0539f, 1f),
                    Name = "Poster3"
                },
                new()
                {
                    Position = new Vector3(5.9419f, 2.5963f, -8.2335f),
                    Rotation = new Vector3(0, 270f, 2.68f),
                    Scale = new Vector3(0.7289f, 0.9989f, 1f),
                    Name = "Poster4"
                },

                usePoster5Vanilla
                    ? new()
                    {
                        // Vanilla model
                        Position = new Vector3(5.8554f, 2.5882f, -19.9712f),
                        Rotation = new Vector3(359.4623f, 68.8888f, 180f),
                        Scale = new Vector3(0.465f, 0.71f, 1f),
                        Name = "Poster5"
                    }
                    : new()
                    {
                        // Quad
                        Position = new Vector3(5.8425f, 2.5882f, -19.9712f),
                        Rotation = new Vector3(0, 270f, 359.8f),
                        Scale = new Vector3(0.5516f, 0.769f, 1f),
                        Name = "Poster5"
                    },

                useTipsVanilla
                    ? new()
                    {
                        // Vanilla model
                        Position = new Vector3(7.8417f, 2.5171f, -21.971f),
                        Rotation = new Vector3(89.9031f, 180f, 0),
                        Scale = new Vector3(46.75954f, 100f, 70.89838f),
                        Name = "CustomTips"
                    }
                    : new()
                    {
                        // Quad
                        Position = new Vector3(2.9911f, 2.8174f, -11.7254f),
                        Rotation = new Vector3(0, 0, 358.6752f),
                        Scale = new Vector3(0.8596f, 1.2194f, 1f),
                        Name = "CustomTips"
                    },
            };
        }
    }
}