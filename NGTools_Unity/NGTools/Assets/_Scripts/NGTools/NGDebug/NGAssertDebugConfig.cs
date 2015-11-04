using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NG.AssertDebug
{
    public enum DebugWarningLevel
    { all, warning, error, none }

    public enum FailureReaction
    { pause, kill, none }

    /// <summary>
    /// Options:
    /// <para />all, assertOnly, and debugOnly will always write to file after each message.
    /// <para />allFailureOnly, assertFailureOnly, and debugErrorOnly will only write to
    /// <para />file on failure and stores messages in a list before writing.
    /// <para />Log file is tab seperated, so don't use tabs in your log messages!
    /// </summary>
    public enum LogToFileCondition
    { all, assertOnly, debugOnly, allFailureOnly, assertFailureOnly, debugErrorOnly, none }

    public enum DebugConsoleDisplay
    { never, editorOnly, buildOnly, both}

    [XmlRoot("AssertDebugConfig")]
    public class AssertDebugConfig
    {
        private static AssertDebugConfig m_instance;
        public static AssertDebugConfig instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new AssertDebugConfig();
                    LoadSettings();
                }
                return m_instance;
            }

            private set { m_instance = value; }
        }

        // Disables all logging
        public bool masterDisable = false;

        // Set what debug messages to log
        public DebugWarningLevel debugLogWarningLevel = DebugWarningLevel.all;

        // Disable assertion messages
        public bool assertDisable = false;

        // Run in debug build only
        public bool runInDebugBuildOnly = true;

        // What to do when we encounter a failure
        public FailureReaction assertFailureReaction = FailureReaction.pause;
        public FailureReaction debugErrorReaction = FailureReaction.pause;
        public bool shouldAssertDoFailure = true;
        public bool shouldLogErrorDoFailure = true;

        // When to log to file - see enum definition for details
        public LogToFileCondition logToFileCondition = LogToFileCondition.all;
        public string logFileName = "debug";
        public string logFileExt = ".txt";
        public string debugFileHeader = "Time\tError Type\tMessage\tStack Trace";

        // Set to automatically take screenshot on failure
        public bool takeScreenShot = false;
        public string screenShotFileName = "error_screen";

        // Set the key that brings up the in-game debug console
        public KeyCode debugConsoleKey = KeyCode.BackQuote;

        // Set whether to show NGDebugConsole on error in editor, builds, or both.
        public DebugConsoleDisplay debugConsoleDisplay = DebugConsoleDisplay.both;
        public bool ShowGuiLogConsoleOnError()
        {
#if !UNITY_EDITOR
            if (debugConsoleDisplay == DebugConsoleDisplay.editorOnly ||
                debugConsoleDisplay == DebugConsoleDisplay.both)
                return true;
#else
            if (debugConsoleDisplay == DebugConsoleDisplay.buildOnly ||
                debugConsoleDisplay == DebugConsoleDisplay.both)
                return true;
#endif

            return false;
        }


        /// <summary>
        /// Returns the log file path. 
        /// When running from the editor this will be the "Unity project folder"/logs.
        /// When running a standalone build this will be MyDocuments/CompanyName/ApplicationName/logs
        /// </summary>
        /// <returns></returns>
        public string GetLogFilePath()
        {
            string path = Application.dataPath;
#if !UNITY_EDITOR
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + Application.companyName + "/" + Application.productName + "/logs";
#else
            DirectoryInfo parentDir = Directory.GetParent(Application.dataPath);
            path = parentDir.FullName + "/logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
#endif          
            return path;
        }

        public string GetDebugFileName()
        {
            return GetLogFilePath() + "/" + logFileName + "_" + AssertDateStamp.FileDate() + logFileExt;
        }

        public string GetScreenShotFileName()
        {
            int extNum = 0;
            while (File.Exists(ScreenShotFileName(extNum)))
                extNum++;

            return ScreenShotFileName(extNum);
        }

        private string ScreenShotFileName(int extNum)
        {
            return GetLogFilePath() + "/" + screenShotFileName + "_" + AssertDateStamp.FileDate() + extNum.ToString() + ".png";
        }

        public const string personalSettingsXmlFile = "personal_debug_settings.xml";
        [XmlIgnore]
        public bool personalSettingsLoaded = false;

        public static string Save()
        {
            string pathFile = instance.GetLogFilePath() + "/" + personalSettingsXmlFile;
            Save(pathFile);
            return pathFile;
        }

        private static void Save(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            var serializer = new XmlSerializer(typeof(AssertDebugConfig));

            using (var stream = new FileStream(fileName, FileMode.Create))
                serializer.Serialize(stream, instance);
        }


        //Don't reference instance here, use m_instance otherwise we'll possibly create an inf loop
        public static void LoadSettings()
        {
            if (!m_instance.personalSettingsLoaded)
            {
                string pathFile = instance.GetLogFilePath() + "/" + personalSettingsXmlFile;
                if (File.Exists(pathFile))
                {
                    var serializer = new XmlSerializer(typeof(AssertDebugConfig));
                    using (var stream = new FileStream(pathFile, FileMode.Open))
                    {
                        m_instance = serializer.Deserialize(stream) as AssertDebugConfig;
                        m_instance.personalSettingsLoaded = true;
                    }
                }
            }
        }

        public static void ReloadSettings()
        {
            m_instance.personalSettingsLoaded = false;
            LoadSettings();
        }

        public static bool RestoreDefaults()
        {
            string pathFile = instance.GetLogFilePath() + "/" + personalSettingsXmlFile;
            if (File.Exists(pathFile))
            {
                File.Delete(pathFile);
                m_instance = null;
                return true;
            }

            return false;
        }
    }



    public class AssertDateStamp
    {
        public static string FileDate()
        {
            System.DateTime time = System.DateTime.Now;
            string format = "yyyyMMdd-HH";
            return time.ToString(format);
        }

        public static string LogEntryTime()
        {
            System.DateTime time = System.DateTime.Now;
            string format = "HH:mm:ss.fff";
            return time.ToString(format);
        }
    }
}
