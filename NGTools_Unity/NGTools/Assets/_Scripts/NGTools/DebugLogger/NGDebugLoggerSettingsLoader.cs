using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace NG
{
    public class NGDebugLoggerSettingsLoader
    {
        public static void LoadSettings()
        {
            NGDebugLoggerSettings[] settingsAssets = Resources.LoadAll<NGDebugLoggerSettings>("NG Debug Logger");

            int countThisPlatform = 0;
            NGDebugLoggerSettings settingsToUse = null;
            foreach (var item in settingsAssets)
            {
                if (item.platforms.IsRuntimePlatform())
                {
                    countThisPlatform++;
                    if (settingsToUse == null)
                        settingsToUse = item;
                }
            }

            if (countThisPlatform > 1)
                Debug.LogErrorFormat("Found {0} settings files for Platform: {1}. Using first found.", countThisPlatform, Application.platform);

            if (settingsToUse != null)
            {
                Debug.logDestinations = settingsToUse.logDestinations;
                Debug.disableContext = settingsToUse.disableContext;
                Debug.consoleLogFilter = settingsToUse.consoleLogFilter;
                Debug.fileLogFilter = settingsToUse.fileLogFilter;
                Debug.consoleLogEnabled = settingsToUse.consoleLogEnabled;
                Debug.fileLogEnabled = settingsToUse.fileLogEnabled;
                Debug.guiLogEnabled = settingsToUse.guiLogEnabled;

                Debug.fileLogger = new FileLogger(
                    settingsToUse.fileLogEnabled,
                    settingsToUse.fileLogFilter,
                    settingsToUse.fileLogPathBase,
                    settingsToUse.fileLogName,
                    settingsToUse.stackTrace);

                // TODO do the same for GuiLogger
            }

#if UNITY_EDITOR
            if (settingsAssets == null || settingsAssets.Length <= 0)
            {
                CreateSettingsAsset();
            }
#endif
        }

#if UNITY_EDITOR
        public static void CreateSettingsAsset()
        {
            string defaultName = "DebugLoggerSettings";
            string ext = ".asset";
            string path = "Assets/Resources/NG Debug Logger";

            if (!AssetDatabase.IsValidFolder(path))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                    AssetDatabase.Refresh();
                }
                AssetDatabase.CreateFolder("Assets/Resources", "NG Debug Logger");
            }

            string[] searchPath = new string[] { path };
            int iter = 0;
            string searchFileName = defaultName;
            while (AssetDatabase.FindAssets(searchFileName, searchPath).Length > 0)
            {
                iter++;
                searchFileName = defaultName + iter.ToString();
                if (iter > 10)
                {
                    UnityEngine.Debug.LogError("Found more than 10 settings files already. New one not created.");
                    return;
                }
            }

            if (iter > 0)
                defaultName = defaultName + iter.ToString();


            NGDebugLoggerSettings asset = ScriptableObject.CreateInstance<NGDebugLoggerSettings>();

            AssetDatabase.CreateAsset(asset, path + "/" + defaultName + ext);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
#endif
    }
}
