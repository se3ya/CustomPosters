namespace CustomPosters
{
    internal static class Constants
    {
        // file extensions
        public static readonly string[] ValidImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp" };
        public static readonly string[] ValidVideoExtensions = { ".mp4" };
        public static readonly string[] AllValidExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" };

        // common folder names
        public const string PluginsFolderName = "plugins";

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

        // poster names
        public const string PosterNamePoster1 = "Poster1";
        public const string PosterNamePoster2 = "Poster2";
        public const string PosterNamePoster3 = "Poster3";
        public const string PosterNamePoster4 = "Poster4";
        public const string PosterNamePoster5 = "Poster5";
        public const string PosterNameCustomTips = "CustomTips";

        // wider ship side values
        public const string WiderShipSideLeft = "Left";
        public const string WiderShipSideRight = "Right";
        public const string WiderShipSideBoth = "Both";

        // single-pack folder name
        public const string SinglePackFolderName = "CustomPosters";

        // networking message identifiers
        public const string PackSyncIdentifier = "CustomPosters_SyncPack";
        public const string VideoRequestIdentifier = "CustomPosters_RequestVideoTime";
        public const string VideoSyncIdentifier = "CustomPosters_SyncVideoTime";

        // ES3 keys
        public const string Es3SelectedPackKey = "CustomPosters_SelectedPack";

        // rendering defaults
        public const int PosterRenderTextureWidth = 512;
        public const int PosterRenderTextureHeight = 512;
        public const int PosterRenderTextureDepth = 16;
    }
}