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
            "posters", 
            "tips", 
            "CustomPosters/posters", 
            "CustomPosters/tips" 
        };

        // GameObject paths
        public const string HangarShipPath = "Environment/HangarShip";
        public const string VanillaPosterPlanePath = "Environment/HangarShip/Plane.001";
        public const string VanillaPosterPlaneOldPath = "Environment/HangarShip/Plane.001 (Old)";
        public const string VanillaPlanePath = "Plane";
        public const string CustomPostersParentName = "CustomPosters";

        // shader names
        public const string HDRPLitShader = "HDRP/Lit";

        // video settings
        public const int DefaultVideoVolume = 20;
        public const float DefaultVideoMaxDistance = 4.0f;
        public const int VideoRenderTextureSize = 512;

        // performance settings
        public const int TextureLoadBatchSize = 5;
        public const float VideoSyncRequestDelay = 0.5f;

        // mod folder exclusions
        public static readonly string[] ExcludedModFolderNames = 
        { 
            "CustomPosters", 
            "seechela-CustomPosters",
            "plugins"
        };

        // network identifiers
        public const string PackSyncIdentifier = "CustomPosters_SyncPack";
        public const string VideoRequestIdentifier = "CustomPosters_RequestVideoTime";
        public const string VideoSyncIdentifier = "CustomPosters_SyncVideoTime";

        // mod dependencies
        public const string BiggerShipGUID = "BiggerShip";
        public const string ShipWindowsGUID = "TestAccount666.ShipWindows";
        public const string WiderShipModGUID = "mborsh.WiderShipMod";
        public const string TwoStoryShipGUID = "MelanieMelicious.2StoryShip";
    }
}