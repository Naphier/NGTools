using UnityEngine;
using UnityEditor;
using NG.AssertDebug;
public class NGDebugEditorWindow : EditorWindow
{
    private const string TITLE = "NG Debug/Assert";
    [MenuItem("Window/NGDebug Config")]
    static void Init()
    {
        NGDebugEditorWindow window = (NGDebugEditorWindow)EditorWindow.GetWindow(typeof(NGDebugEditorWindow));
        GUIContent titleContent = new GUIContent(TITLE);
        window.titleContent = titleContent;
        window.minSize = new Vector2(360, 510);
        window.Show();
    }

    
    void OnGUI()
    {
        GUILayout.Label(TITLE + " Configuration Settings", EditorStyles.boldLabel);
        
        AssertDebugConfig.instance.masterDisable = EditorGUILayout.ToggleLeft("Master disable", AssertDebugConfig.instance.masterDisable);
        AssertDebugConfig.instance.debugLogWarningLevel = (DebugWarningLevel)EditorGUILayout.EnumPopup("Debug warning level", AssertDebugConfig.instance.debugLogWarningLevel);
        AssertDebugConfig.instance.assertDisable = EditorGUILayout.ToggleLeft("Disable assertions", AssertDebugConfig.instance.assertDisable);
        AssertDebugConfig.instance.runInDebugBuildOnly = EditorGUILayout.ToggleLeft("Run in debug build only", AssertDebugConfig.instance.runInDebugBuildOnly);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("What to do when an error occurs:");
        AssertDebugConfig.instance.assertFailureReaction = (FailureReaction)EditorGUILayout.EnumPopup("Assert failure: ", AssertDebugConfig.instance.assertFailureReaction);
        AssertDebugConfig.instance.shouldAssertDoFailure = EditorGUILayout.ToggleLeft("Should assert do failure? ", AssertDebugConfig.instance.shouldAssertDoFailure);
        AssertDebugConfig.instance.debugErrorReaction = (FailureReaction)EditorGUILayout.EnumPopup("Debug error: ", AssertDebugConfig.instance.debugErrorReaction);
        AssertDebugConfig.instance.shouldLogErrorDoFailure = EditorGUILayout.ToggleLeft("Should log error do failure? ", AssertDebugConfig.instance.shouldLogErrorDoFailure);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("Log file settings:");
        AssertDebugConfig.instance.logToFileCondition = (LogToFileCondition)EditorGUILayout.EnumPopup("Assert failure: ", AssertDebugConfig.instance.logToFileCondition);
        AssertDebugConfig.instance.logFileName = EditorGUILayout.TextField("Log file base name: ", AssertDebugConfig.instance.logFileName);
        AssertDebugConfig.instance.logFileExt = EditorGUILayout.TextField("Extension: ", AssertDebugConfig.instance.logFileExt);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("Screenshot settings:");
        AssertDebugConfig.instance.takeScreenShot = EditorGUILayout.ToggleLeft("Take screenshot on failure? ", AssertDebugConfig.instance.takeScreenShot);
        AssertDebugConfig.instance.screenShotFileName = EditorGUILayout.TextField("Base file name: ", AssertDebugConfig.instance.screenShotFileName);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("In-game debug console:");
        AssertDebugConfig.instance.debugConsoleDisplay = (DebugConsoleDisplay)EditorGUILayout.EnumPopup("When to display: ", AssertDebugConfig.instance.debugConsoleDisplay);
        AssertDebugConfig.instance.debugConsoleKey = (KeyCode)EditorGUILayout.EnumPopup("Key to open/close: ", AssertDebugConfig.instance.debugConsoleKey);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Save"))
        {
            string pathFile = AssertDebugConfig.Save();
            EditorUtility.DisplayDialog(TITLE, "Personal settings saved to:\n" + pathFile + "\nDo not commit to repository.", "OK", "");
            AssertDebugConfig.ReloadSettings();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Restore defaults"))
        {
            string msg = "Default settings already used.";
            if (AssertDebugConfig.RestoreDefaults())
            {
                msg = "Default settings restored. Personal settings deleted.";
            }

            EditorUtility.DisplayDialog(TITLE, msg, "OK", "");
        }

        string label = (AssertDebugConfig.instance.personalSettingsLoaded ? "Using personal settings" : "Personal settings not loaded - using default");
        GUILayout.Label(label, EditorStyles.helpBox);
    }
}