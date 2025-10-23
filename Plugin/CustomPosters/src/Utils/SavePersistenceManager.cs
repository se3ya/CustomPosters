using System;
using System.IO;
using System.Reflection;
using BepInEx;

namespace CustomPosters.Utils
{
    internal static class SavePersistenceManager
    {
    private const string Es3Key = Constants.Es3SelectedPackKey;

        public static string? TryGetCurrentSaveId()
        {
            try
            {
                var gnm = GameNetworkManager.Instance;
                if (gnm != null)
                {
                    var type = gnm.GetType();
                    var field = type.GetField("currentSaveFileName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var prop = type.GetProperty("currentSaveFileName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var value = field?.GetValue(gnm) as string ?? prop?.GetValue(gnm) as string;
                    if (!string.IsNullOrEmpty(value)) return value!;
                }
            }
            catch
            {
            }
            return null;
        }

        public static string? TryLoadSelectedPack(string saveId)
        {
            try
            {
                if (string.IsNullOrEmpty(saveId)) return null;
                if (ES3.KeyExists(Es3Key, saveId))
                {
                    var val = ES3.Load<string>(Es3Key, saveId, "");
                    return string.IsNullOrEmpty(val) ? null : val;
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogDebug($"Failed to load per save selection from ES3: {ex.Message}");
            }
            return null;
        }

        public static void SaveSelectedPack(string saveId, string packPath)
        {
            try
            {
                if (string.IsNullOrEmpty(saveId)) return;
                ES3.Save<string>(Es3Key, packPath, saveId);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogDebug($"Failed to save per save selection to ES3: {ex.Message}");
            }
        }
    }
}
