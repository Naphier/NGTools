using UnityEngine;
using UnityEditor;

namespace NG
{

    [CustomEditor(typeof(NGDebugLoggerSettings))]
    public class NGDebugLoggerSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            NGDebugLoggerSettings settings = (NGDebugLoggerSettings)target;

            MakSpaces(1);
            settings.platforms = (Platform)EditorGUILayout.EnumMaskField("Platforms: ", settings.platforms);

            settings.logDestinations = (LogDestinationType)EditorGUILayout.EnumMaskField("Log Destinations: ", settings.logDestinations);

            settings.disableContext = (DisableContext)EditorGUILayout.EnumMaskField("Disable context on: ", settings.disableContext);

            MakSpaces(2);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            settings.consoleLogEnabled = EditorGUILayout.ToggleLeft("Console Log Enabled", settings.consoleLogEnabled);
            if (settings.consoleLogEnabled)
            {
                settings.consoleLogFilter = (NGLogType)EditorGUILayout.EnumMaskField("Filters: ", settings.consoleLogFilter);
            }

            MakSpaces(2);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            settings.fileLogEnabled = EditorGUILayout.ToggleLeft("File Log Enabled", settings.fileLogEnabled);
            if (settings.fileLogEnabled)
            {
                settings.fileLogFilter = (NGLogType)EditorGUILayout.EnumMaskField("Filters: ", settings.fileLogFilter);

                settings.fileLogPathBaseType = (FileLogPathType)EditorGUILayout.EnumPopup("Save path: ", settings.fileLogPathBaseType);

                if (settings.fileLogPathBaseType == FileLogPathType.Custom)
                    settings.custompath = EditorGUILayout.TextField("Custom path: ", settings.custompath);

                settings.fileLogName = EditorGUILayout.TextField("File name: ", settings.fileLogName);

                EditorGUILayout.LabelField(string.Format("Example path: {0}", settings.fileLogPathBase + "/" + settings.fileLogName), EditorStyles.helpBox);

                settings.stackTrace = (StackTraceType)EditorGUILayout.EnumPopup("Stack Tracing: ", settings.stackTrace);
            }

            MakSpaces(2);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            settings.guiLogEnabled = EditorGUILayout.ToggleLeft("GUI Log Enabled", settings.guiLogEnabled);
            if (settings.guiLogEnabled)
            {
                settings.guiLogFilter = (NGLogType)EditorGUILayout.EnumMaskField("Filters: ", settings.guiLogFilter);
            }
        }

        public void MakSpaces(int spaces)
        {
            for (int i = 1; i <= spaces; i++)
            {
                EditorGUILayout.Space();
            }
        }
    }
}