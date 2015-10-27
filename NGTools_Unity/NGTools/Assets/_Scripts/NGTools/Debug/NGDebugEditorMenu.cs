using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NG;

public class NGDebugEditorMenu : Editor
{
    [MenuItem("Edit/NGDebug/Save Settings")]
    static void SaveSettings()
    {
        string pathFile = AssertDebugConfig.instance.GetLogFilePath() + "/" + AssertDebugConfig.personalSettingsXmlFile;
        AssertDebugConfigSerialized.Save(pathFile);
        EditorUtility.DisplayDialog("NGDebug", "Personal settings saved to:\n" + pathFile + "\nDo not commit to repository.", "OK", "");
    }
}

[XmlRoot("AssertDebugConfig")]
public class AssertDebugConfigSerialized
{
    // Disables all logging
    [XmlAttribute("masterDisable")]
    public bool masterDisable = AssertDebugConfig.instance.masterDisable;

    // Set what debug messages to log
    [XmlAttribute("debugLogWarningLevel")]
    public DebugWarningLevel debugLogWarningLevel = DebugWarningLevel.all;

    // Disable assertion messages
    [XmlAttribute("assertDisable")]
    public bool assertDisable = false;

    // Run in debug build only
    [XmlAttribute("runInDebugBuildOnly")]
    public bool runInDebugBuildOnly = true;

    // What to do when we encounter a failure
    [XmlAttribute("assertFailureReaction")]
    public FailureReaction assertFailureReaction = FailureReaction.pause;
    [XmlAttribute("debugReaction")]
    public FailureReaction debugReaction = FailureReaction.pause;
    [XmlAttribute("shouldAssertDoFailure")]
    public bool shouldAssertDoFailure = true;
    [XmlAttribute("shouldLogErrorDoFailure")]
    public bool shouldLogErrorDoFailure = true;

    // When to log to file - see enum definition for details
    [XmlAttribute("logToFileCondition")]
    public LogToFileCondition logToFileCondition = LogToFileCondition.all;

    [XmlAttribute("logFileName")]
    public string logFileName = "debug";
    [XmlAttribute("logFileExt")]
    public string logFileExt = ".txt";
    [XmlAttribute("debugFileHeader")]
    public string debugFileHeader = "Time\tError Type\tMessage\tStack Trace";

    // Set to automatically take screenshot on failure
    [XmlAttribute("takeScreenShot")]
    public bool takeScreenShot = false;
    [XmlAttribute("screenShotFileName")]
    public string screenShotFileName = "error_screen";

    // Set the key that brings up the in-game debug console
    [XmlAttribute("debugConsoleKey")]
    public KeyCode debugConsoleKey = KeyCode.BackQuote;

    public static void Save(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;
        AssertDebugConfigSerialized asd = new AssertDebugConfigSerialized();
        var serializer = new XmlSerializer(typeof(AssertDebugConfigSerialized));
        
        using (var stream = new FileStream(fileName, FileMode.Create))
            serializer.Serialize(stream, asd);        
    }

    public static void LoadSettings()
    {
        if (!AssertDebugConfig.instance.personalSettingsLoaded)
        {
            AssertDebugConfig.instance.personalSettingsLoaded = true;
            string pathFile = AssertDebugConfig.instance.GetLogFilePath() + "/" + AssertDebugConfig.personalSettingsXmlFile;
            if (File.Exists(pathFile))
            {
                var serializer = new XmlSerializer(typeof(AssertDebugConfigSerialized));
                using (var stream = new FileStream(pathFile, FileMode.Open))
                {
                    AssertDebugConfigSerialized asds = serializer.Deserialize(stream) as AssertDebugConfigSerialized;
                    AssertDebugConfig.instance.masterDisable = asds.masterDisable;
                    UnityEngine.Debug.Log("settings loaded AssertDebugConfig.masterDisable: " + AssertDebugConfig.instance.masterDisable.ToString());
                }
            }
        }
    }
}