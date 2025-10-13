using System;

namespace CustomPosters.Utils
{
    internal static class PathUtils
    {
        public static string GetPrettyPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;

            int pluginsIndex = fullPath.IndexOf("plugins", StringComparison.OrdinalIgnoreCase);
            if (pluginsIndex != -1)
            {
                return fullPath.Substring(pluginsIndex + "plugins".Length + 1);
            }
            
            return fullPath;
        }
        
        public static string GetPackName(string fullPackName)
        {
            if (string.IsNullOrEmpty(fullPackName))
                return string.Empty;

            int dashIndex = fullPackName.IndexOf('-');
            if (dashIndex > 0 && dashIndex < fullPackName.Length - 1)
            {
                return fullPackName.Substring(dashIndex + 1);
            }
    
            return fullPackName;
        }
    }
}