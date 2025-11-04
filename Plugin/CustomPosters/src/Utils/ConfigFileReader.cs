using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomPosters.Utils
{
    internal static class ConfigFileReader
    {
        public static bool ReadBoolValue(string configPath, string key, bool defaultValue = true)
        {
            if (!File.Exists(configPath))
            {
                return defaultValue;
            }

            try
            {
                var lines = File.ReadAllLines(configPath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = trimmed.Substring(key.Length).Trim();
                        if (bool.TryParse(value, out bool result))
                        {
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error reading config file {configPath}: {ex.Message}");
            }

            return defaultValue;
        }

        public static string ReadStringValue(string configPath, string key, string defaultValue = "")
        {
            if (!File.Exists(configPath))
            {
                return defaultValue;
            }

            try
            {
                var lines = File.ReadAllLines(configPath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return trimmed.Substring(key.Length).Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error reading config file {configPath}: {ex.Message}");
            }

            return defaultValue;
        }

        public static bool ReadBoolFromSection(string configPath, string sectionHeader, string key, bool defaultValue = true)
        {
            if (!File.Exists(configPath))
            {
                return defaultValue;
            }

            try
            {
                var lines = File.ReadAllLines(configPath);
                bool inSection = false;

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();

                    if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        inSection = trimmed.Equals($"[{sectionHeader}]", StringComparison.OrdinalIgnoreCase);
                        continue;
                    }

                    if (inSection && trimmed.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = trimmed.Substring(key.Length).Trim();
                        if (bool.TryParse(value, out bool result))
                        {
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error reading config file {configPath}: {ex.Message}");
            }

            return defaultValue;
        }

        public static string ReadStringFromSection(string configPath, string sectionHeader, string key, string defaultValue = "")
        {
            if (!File.Exists(configPath))
            {
                return defaultValue;
            }

            try
            {
                bool inSection = false;
                foreach (var rawLine in File.ReadAllLines(configPath))
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        var current = line.Substring(1, line.Length - 2).Trim();
                        inSection = string.Equals(current, sectionHeader, StringComparison.OrdinalIgnoreCase);
                        continue;
                    }

                    if (!inSection) continue;

                    if (line.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    {
                        var idx = line.IndexOf('=');
                        if (idx >= 0 && idx + 1 < line.Length)
                        {
                            var value = line.Substring(idx + 1).Trim();
                            return value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try { CustomPosters.Plugin.Log.LogDebug($"ReadStringFromSection error for '{sectionHeader}:{key}': {ex.Message}"); } catch { }
            }

            return defaultValue;
        }

        public static Dictionary<string, string> ReadMultipleValues(string configPath, params string[] keys)
        {
            var results = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(configPath))
            {
                return results;
            }

            try
            {
                var lines = File.ReadAllLines(configPath);
                var keysToFind = new HashSet<string>(keys, StringComparer.OrdinalIgnoreCase);

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    foreach (var key in keysToFind)
                    {
                        if (trimmed.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                        {
                            var value = trimmed.Substring(key.Length).Trim();
                            results[key] = value;

                            if (results.Count == keys.Length)
                            {
                                return results;
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error reading config file {configPath}: {ex.Message}");
            }

            return results;
        }
    }
}
