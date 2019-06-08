using UnityEditor;

namespace NG.Logging
{
    public class NGDebugLoggerMenu : Editor
    {
        [MenuItem("Window/NG Debug Logger/Settings Manager")]
        public static void OpenSettingsManager()
        {
            // TODO
            NGDebugLoggerSettingsLoader.LoadSettings();
        }

        [MenuItem("Window/NG Debug Logger/Create Settings Asset")]
        public static void CreateSettingsAsset()
        {
            NGDebugLoggerSettingsLoader.CreateSettingsAsset();
        }
    }
}