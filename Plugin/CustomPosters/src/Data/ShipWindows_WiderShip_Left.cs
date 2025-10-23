using UnityEngine;
using CustomPosters.Data;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class ShipWindows_WiderShip_Left
    {
        public static PosterData[] Get()
        {
            bool usePoster5Vanilla = Plugin.ModConfig.UsePoster5VanillaModel;
            bool useTipsVanilla = Plugin.ModConfig.UseTipsVanillaModel;

            return new PosterData[]
            {
                new()
                {
                    Position = new Vector3(4.6777f, 2.9007f, -19.63f),
                    Rotation = new Vector3(0, 118.2274f, 0),
                    Scale = new Vector3(0.6391f, 0.4882f, 2f),
                    Name = Constants.PosterNamePoster1
                },
                new()
                {
                    Position = new Vector3(6.4202f, 2.2577f, -10.8226f),
                    Rotation = new Vector3(0, 0, 0),
                    Scale = new Vector3(0.7296f, 0.4896f, 1f),
                    Name = Constants.PosterNamePoster2
                },
                new()
                {
                    Position = new Vector3(9.9186f, 2.8591f, -17.4716f),
                    Rotation = new Vector3(0, 180f, 356.3345f),
                    Scale = new Vector3(0.7487f, 1.0539f, 1f),
                    Name = Constants.PosterNamePoster3
                },
                new()
                {
                    Position = new Vector3(6.4449f, 3.0961f, -10.8221f),
                    Rotation = new Vector3(0, 0, 2.68f),
                    Scale = new Vector3(0.7289f, 0.9989f, 1f),
                    Name = Constants.PosterNamePoster4
                },

                usePoster5Vanilla
                    ? PosterHelper.Poster5Vanilla(
                        new Vector3(5.3602f, 2.5882f, -18.3492f),
                        new Vector3(357.7245f, 277.6282f, 180f),
                        new Vector3(0.465f, 0.71f, 1f))
                    : PosterHelper.Poster5Quad(
                        new Vector3(5.3602f, 2.5482f, -18.3793f),
                        new Vector3(0, 118.0114f, 359.8f),
                        new Vector3(0.5516f, 0.769f, 1f)),

                useTipsVanilla
                    ? PosterHelper.TipsVanilla(
                        new Vector3(8.147f, 2.399426f, -21.929f),
                        new Vector3(-270.38f, -321.026f, 219.239f),
                        new Vector3(46.75954f, 100f, 70.89838f))
                    : PosterHelper.TipsQuad(
                        new Vector3(2.8647f, 2.7774f, -11.7341f),
                        new Vector3(0, 0, 358.6752f),
                        new Vector3(0.8596f, 1.2194f, 1f))
            };
        }
    }
}
