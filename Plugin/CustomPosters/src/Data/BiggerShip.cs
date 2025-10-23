using UnityEngine;
using CustomPosters.Data;

namespace CustomPosters.Data.PosterLayouts
{
    internal static class BiggerShip
    {
        public static PosterData[] Get()
        {
            bool usePoster5Vanilla = Plugin.ModConfig.UsePoster5VanillaModel;
            bool useTipsVanilla = Plugin.ModConfig.UseTipsVanillaModel;

            return new PosterData[]
            {
                new()
                {
                    Position = new Vector3(4.1886f, 2.8918f, -17.8606f),
                    Rotation = new Vector3(0, 169.6929f, 0),
                    Scale = new Vector3(0.6391f, 0.4882f, 2f),
                    Name = Constants.PosterNamePoster1
                },
                new()
                {
                    Position = new Vector3(6.4202f, 2.4776f, -10.9064f),
                    Rotation = new Vector3(0, 0, 0),
                    Scale = new Vector3(0.7296f, 0.4896f, 1f),
                    Name = Constants.PosterNamePoster2
                },
                new()
                {
                    Position = new Vector3(9.9186f, 2.8591f, -17.5587f),
                    Rotation = new Vector3(0, 180f, 356.3345f),
                    Scale = new Vector3(0.7487f, 1.0539f, 1f),
                    Name = Constants.PosterNamePoster3
                },
                new()
                {
                    Position = new Vector3(5.2187f, 2.5963f, -10.782f),
                    Rotation = new Vector3(0, 10.7715f, 2.68f),
                    Scale = new Vector3(0.7289f, 0.9989f, 1f),
                    Name = Constants.PosterNamePoster4
                },

                usePoster5Vanilla
                    ? PosterHelper.Poster5Vanilla(
                        new Vector3(5.5286f, 2.5882f, -17.6077f),
                        new Vector3(1.3609f, 329.1297f, 182.4321f),
                        new Vector3(0.465f, 0.71f, 1f))
                    : PosterHelper.Poster5Quad(
                        new Vector3(5.5286f, 2.5882f, -17.6184f),
                        new Vector3(0, 169.7645f, 359.8f),
                        new Vector3(0.5516f, 0.769f, 1f)),

                useTipsVanilla
                    ? PosterHelper.TipsVanilla(
                        new Vector3(8.147f, 2.399426f, -20.6869f),
                        new Vector3(-270.38f, -321.026f, 219.239f),
                        new Vector3(46.75954f, 100f, 70.89838f))
                    : PosterHelper.TipsQuad(
                        new Vector3(3.0647f, 2.8174f, -10.4842f),
                        new Vector3(0, 0, 0),
                        new Vector3(0.8596f, 1.2194f, 1f))
            };
        }
    }
}