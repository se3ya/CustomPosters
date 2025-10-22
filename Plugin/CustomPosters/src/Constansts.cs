namespace CustomPosters
{
    internal static class Constants
    {
        // file extensions
        public static readonly string[] ValidImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp" };
        public static readonly string[] ValidVideoExtensions = { ".mp4" };
        public static readonly string[] AllValidExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" };

        // poster pack subdirectories
        public static readonly string[] PosterPackSubdirectories = 
        { 
            "*/posters",  // multi-pack
            "*/tips",     // multi-pack
            "CustomPosters/posters",  // single-pack
            "CustomPosters/tips",     // single-pack
            "*/CustomPosters/posters", // single-pack
            "*/CustomPosters/tips"     // single-pack
        };

        // video settings
        public const int DefaultVideoVolume = 10;
        public const float DefaultVideoMaxDistance = 3.5f;

        // mod folder exclusions
        public static readonly string[] ExcludedModFolderNames = 
        { 
            "CustomPosters", 
            "seechela-CustomPosters",
            "plugins"
        };

        // mod dependencies
        public const string BiggerShipGUID = "BiggerShip";
        public const string ShipWindowsGUID = "TestAccount666.ShipWindows";
        public const string WiderShipModGUID = "mborsh.WiderShipMod";
        public const string TwoStoryShipGUID = "MelanieMelicious.2StoryShip";
    }
}