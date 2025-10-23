using UnityEngine;
using CustomPosters.Data;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class WiderShip_Both
    {
        public static PosterData[] Get()
        {
            bool usePoster5Vanilla = Plugin.ModConfig.UsePoster5VanillaModel;
            bool useTipsVanilla = Plugin.ModConfig.UseTipsVanillaModel;

            return new PosterData[]
            {
                new()
                {
                    Position = new Vector3(4.6877f, 2.9407f, -19.62f),
                    Rotation = new Vector3(0, 118.2274f, 0),
                    Scale = new Vector3(0.6391f, 0.4882f, 2f),
                    Name = Constants.PosterNamePoster1
                },
                new()
                {
                    Position = new Vector3(6.4202f, 2.4776f, -10.8218f),
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
                    Position = new Vector3(5.5699f, 2.5963f, -10.3268f),
                    Rotation = new Vector3(0, 62.0324f, 2.6799f),
                    Scale = new Vector3(0.7289f, 0.9989f, 1f),
                    Name = Constants.PosterNamePoster4
                },

                usePoster5Vanilla
                    ? PosterHelper.Poster5Vanilla(
                        new Vector3(5.3602f, 2.5882f, -18.3492f),
                        new Vector3(357.7245f, 277.6282f, 180f),
                        new Vector3(0.465f, 0.71f, 1f))
                    : PosterHelper.Poster5Quad(
                        new Vector3(5.3602f, 2.5882f, -18.3793f),
                        new Vector3(0, 118.0114f, 359.8f),
                        new Vector3(0.5516f, 0.769f, 1f)),

                useTipsVanilla
                    ? PosterHelper.TipsVanilla(
                        new Vector3(7.7417f, 2.4171f, -16.9514f),
                        new Vector3(90f, 180f, 0),
                        new Vector3(46.75954f, 100f, 70.89838f))
                    : PosterHelper.TipsQuad(
                        new Vector3(3.0947f, 2.8174f, -6.7252f),
                        new Vector3(0, 0, 358.6752f),
                        new Vector3(0.8596f, 1.2194f, 1f))
            };
        }
    }
}