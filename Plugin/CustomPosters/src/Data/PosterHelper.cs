using UnityEngine;

namespace CustomPosters.Data
{
    internal static class PosterHelper
    {
        public static PosterData Poster5Vanilla(Vector3 pos, Vector3 rot, Vector3 scale) => new PosterData
        {
            Position = pos,
            Rotation = rot,
            Scale = scale,
            Name = Constants.PosterNamePoster5
        };

        public static PosterData Poster5Quad(Vector3 pos, Vector3 rot, Vector3 scale) => new PosterData
        {
            Position = pos,
            Rotation = rot,
            Scale = scale,
            Name = Constants.PosterNamePoster5
        };

        public static PosterData TipsVanilla(Vector3 pos, Vector3 rot, Vector3 scale) => new PosterData
        {
            Position = pos,
            Rotation = rot,
            Scale = scale,
            Name = Constants.PosterNameCustomTips
        };

        public static PosterData TipsQuad(Vector3 pos, Vector3 rot, Vector3 scale) => new PosterData
        {
            Position = pos,
            Rotation = rot,
            Scale = scale,
            Name = Constants.PosterNameCustomTips
        };
    }
}
