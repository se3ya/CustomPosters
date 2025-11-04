using UnityEngine;
using CustomPosters.Data;

namespace CustomPosters.Data.PosterLayouts.Legacy
{
    internal static class TwoStoryShip_WiderShip
    {
        public static PosterData[] Get()
        {
            bool usePoster5Vanilla = Plugin.ModConfig.UsePoster5VanillaModel;
            bool useTipsVanilla = Plugin.ModConfig.UseTipsVanillaModel;

            return new PosterData[]
            {
                new()
                {
                    Position = new Vector3(4.2012f, 2.9407f, -21.8535f),
                    Rotation = new Vector3(0, 200.9627f, 0),
                    Scale = new Vector3(0.6391f, 0.4882f, 2f),
                    Name = Constants.PosterNamePoster1
                },
                new()
                {
                    Position = new Vector3(5.9419f, 2.4776f, -6.5299f),
                    Rotation = new Vector3(0, 270f, 0),
                    Scale = new Vector3(0.7296f, 0.4896f, 1f),
                    Name = Constants.PosterNamePoster2
                },
                new()
                {
                    Position = new Vector3(10.1364f, 2.8591f, -22.4808f),
                    Rotation = new Vector3(0, 180f, 356.3345f),
                    Scale = new Vector3(0.7487f, 1.0539f, 1f),
                    Name = Constants.PosterNamePoster3
                },
                new()
                {
                    Position = new Vector3(5.9419f, 2.5963f, -8.2335f),
                    Rotation = new Vector3(0, 270f, 2.68f),
                    Scale = new Vector3(0.7289f, 0.9989f, 1f),
                    Name = Constants.PosterNamePoster4
                },

                usePoster5Vanilla
                    ? PosterHelper.Poster5Vanilla(
                        new Vector3(5.3554f, 2.5882f, -22.2798f),
                        new Vector3(359.4623f, 359.907f, 180f),
                        new Vector3(0.465f, 0.71f, 1f))
                    : PosterHelper.Poster5Quad(
                        new Vector3(5.3554f, 2.5882f, -22.2956f),
                        new Vector3(0, 200.9633f, 359.8f),
                        new Vector3(0.5516f, 0.769f, 1f)),

                useTipsVanilla
                    ? PosterHelper.TipsVanilla(
                        new Vector3(8.147f, 2.399426f, -21.929f),
                        new Vector3(-270.38f, -321.026f, 219.239f),
                        new Vector3(46.75954f, 100f, 70.89838f))
                    : PosterHelper.TipsQuad(
                        new Vector3(2.9911f, 2.8174f, -11.7254f),
                        new Vector3(0, 0, 358.6752f),
                        new Vector3(0.8596f, 1.2194f, 1f)),
            };
        }
    }
}
