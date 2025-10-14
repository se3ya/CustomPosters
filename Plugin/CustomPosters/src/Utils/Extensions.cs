using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomPosters.Utils
{
    internal static class Extensions
    {
        public static bool IsImage(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return Constants.ValidImageExtensions.Contains(ext);
        }

        public static bool IsVideo(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return Constants.ValidVideoExtensions.Contains(ext);
        }

        public static bool IsValidPosterFile(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return Constants.AllValidExtensions.Contains(ext);
        }

        public static string NormalizePath(this string path)
        {
            return path?.Replace('\\', '/') ?? string.Empty;
        }

        public static string GetPosterName(this string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath)?.ToLower() ?? string.Empty;
        }

        public static T GetComponentSafe<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component
        {
            return gameObject != null ? gameObject.GetComponent<T>() : null!;
        }

        public static void AddRangeDistinct<T>(this List<T> list, IEnumerable<T> items, IEqualityComparer<T> comparer = null!)
        {
            comparer ??= EqualityComparer<T>.Default;
            var existingSet = new HashSet<T>(list, comparer);
            
            foreach (var item in items)
            {
                if (existingSet.Add(item))
                {
                    list.Add(item);
                }
            }
        }
    }
}