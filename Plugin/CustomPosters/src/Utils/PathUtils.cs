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

        public static string GetDisplayPackName(string packPath)
        {
            if (string.IsNullOrEmpty(packPath)) return string.Empty;

            var folderName = System.IO.Path.GetFileName(packPath);
            var parent = System.IO.Path.GetDirectoryName(packPath);
            if (!string.IsNullOrEmpty(parent))
            {
                var parentName = System.IO.Path.GetFileName(parent);
                if (string.Equals(folderName, "CustomPosters", StringComparison.OrdinalIgnoreCase))
                {
                    return GetPackName(parentName);
                }
                if (!string.IsNullOrEmpty(parentName))
                {
                    return folderName;
                }
            }

            return folderName;
        }
    }
}