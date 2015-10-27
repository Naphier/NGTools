#define NG_RIGIDBODY_EXTENSION //define this if using NGRigidbodyPauser.cs
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace NG
{
    public enum DebugWarningLevel
    { all, warning, error, none }

    public enum FailureReaction
    { pause, kill, none}

    /// <summary>
    /// Options:
    /// <para />all, assertOnly, and debugOnly will always write to file after each message.
    /// <para />allFailureOnly, assertFailureOnly, and debugErrorOnly will only write to
    /// <para />file on failure and stores messages in a list before writing.
    /// <para />Log file is tab seperated, so don't use tabs in your log messages!
    /// </summary>
    public enum LogToFileCondition
    { all, assertOnly, debugOnly, allFailureOnly, assertFailureOnly, debugErrorOnly, none }

    // TODO
    //need to convert all public statics to public and access via instance
    // convert all to serializable, add saving and loading methods

    public class AssertDebugConfig
    {
        private static AssertDebugConfig m_instance;
        public static AssertDebugConfig instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new AssertDebugConfig();
                return m_instance;
            }
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
        public FailureReaction debugReaction = FailureReaction.pause;
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
        public bool ShowGuiLogConsoleOnError()
        {
#if !UNITY_EDITOR
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
        public bool personalSettingsLoaded = false;
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



    /// <summary>
    /// Replacement class that "extends" UnityEngine.Assertions.
    /// Is not executed if not running in the editor and not a debug build.
    /// </summary>
    public class Assert
    {
        private static bool ShouldExecute()
        {
            AssertDebugConfigSerialized.LoadSettings();

#pragma warning disable 0162
            if (AssertDebugConfig.instance.masterDisable || AssertDebugConfig.instance.assertDisable)
                return false;

#if UNITY_EDITOR
            RegisterLogCallBack();
            return true;
#endif

            if (!UnityEngine.Debug.isDebugBuild && AssertDebugConfig.instance.runInDebugBuildOnly)
                return false;

            RegisterLogCallBack();
            return true;
#pragma warning restore
        }

        private static bool isCallbackRegistered = false;
        private static void RegisterLogCallBack()
        {
            if (isCallbackRegistered)
                return;

            isCallbackRegistered = true;

            HandleLog handleLog = GameObject.FindObjectOfType<HandleLog>();
            if (handleLog == null)
            {
                GameObject go = new GameObject("HandleLog");
                Object.DontDestroyOnLoad(go);
                handleLog = go.AddComponent<HandleLog>();
            }
        }


        /// <summary>
        /// Asserts that two values are equal and disables the referenced MonoBehaviour.
        /// </summary>
        /// <param name="monoBehaviour">The class instance that will be disabled on failure</param>
        /// <returns>true if values are equal</returns>
        public static bool AreEqual<T>(T expected, T actual, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (!Equals(expected, actual))
            {
                UnityEngine.Assertions.Assert.AreEqual(expected, actual);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool AreEqual<T>(T expected, T actual)
        {
            return AreEqual(expected, actual, null);
        }


        public static bool AreApproximatelyEqual(float expected, float actual, float? tolerance, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            bool pass = true;
            if (tolerance != null)
            {
                tolerance = Mathf.Abs((float)tolerance);
                float difference = Mathf.Abs(expected - actual);
                if (difference > tolerance)
                {
                    UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected, actual, (float)tolerance);
                    pass = false;
                }
            }
            else
            {
                if (!Mathf.Approximately(expected, actual))
                {
                    UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected, actual);
                    pass = false;
                }
            }

            if (!pass)
            {
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return pass;
        }

        public static bool AreApproximatelyEqual(float expected, float actual, float tolerance)
        {
            return AreApproximatelyEqual(expected, actual, tolerance, null);
        }

        public static bool AreApproximatelyEqual(float expected, float actual, MonoBehaviour monoBehaviour)
        {
            return AreApproximatelyEqual(expected, actual, null, monoBehaviour);
        }

        public static bool AreApproximatelyEqual(float expected, float actual)
        {
            return AreApproximatelyEqual(expected, actual, null, null);
        }


        public static bool AreNotApproximatelyEqual(float expected, float actual, float? tolerance, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            bool pass = true;
            if (tolerance != null)
            {
                tolerance = Mathf.Abs((float)tolerance);
                float difference = Mathf.Abs(expected - actual);
                if (difference < tolerance)
                {
                    UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual, (float)tolerance);
                    pass = false;
                }
            }
            else
            {
                if (Mathf.Approximately(expected, actual))
                {
                    UnityEngine.Assertions.Assert.AreNotApproximatelyEqual(expected, actual);
                    pass = false;
                }
            }

            if (!pass)
            {
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return pass;
        }

        public static bool AreNotApproximatelyEqual(float expected, float actual, float tolerance)
        {
            return AreNotApproximatelyEqual(expected, actual, tolerance, null);
        }

        public static bool AreNotApproximatelyEqual(float expected, float actual, MonoBehaviour monoBehaviour)
        {
            return AreNotApproximatelyEqual(expected, actual, null, monoBehaviour);
        }

        public static bool AreNotApproximatelyEqual(float expected, float actual)
        {
            return AreNotApproximatelyEqual(expected, actual, null, null);
        }


        public static bool AreNotEqual<T>(T expected, T actual, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (Equals(expected, actual))
            {
                UnityEngine.Assertions.Assert.AreNotEqual(expected, actual);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool AreNotEqual<T>(T expected, T actual)
        {
            return AreNotEqual(expected, actual, null);
        }


        public static bool IsFalse(bool condition, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (condition)
            {
                UnityEngine.Assertions.Assert.IsFalse(condition);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool IsFalse(bool condition)
        {
            return IsFalse(condition, null);
        }


        public static bool IsTrue(bool condition, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (!condition)
            {
                UnityEngine.Assertions.Assert.IsTrue(condition);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool IsTrue(bool condition)
        {
            return IsTrue(condition, null);
        }

        public static bool IsNotNull(object value, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (value == null)
            {
                UnityEngine.Assertions.Assert.IsNotNull(value);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool IsNotNull(object value)
        {
            return IsNotNull(value, null);
        }


        public static bool IsNull(object value, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (value != null)
            {
                UnityEngine.Assertions.Assert.IsNull(value);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool IsNull(object value)
        {
            return IsNull(value, null);
        }
    }


    /// <summary>
    /// Replacement for Unity's debug so we can just shut it off for production.
    /// Is not executed if not running in the editor and not a debug build.
    /// </summary>
    public class Debug
    {
        private static DebugWarningLevel warningLevel = AssertDebugConfig.instance.debugLogWarningLevel;
        private static bool ShouldExecute()
        {
            AssertDebugConfigSerialized.LoadSettings();
#pragma warning disable 0162
            if (AssertDebugConfig.instance.masterDisable || 
                AssertDebugConfig.instance.debugLogWarningLevel == DebugWarningLevel.none)
                return false;

#if UNITY_EDITOR
            RegisterLogCallBack();
            return true;
#endif

            if (!UnityEngine.Debug.isDebugBuild && AssertDebugConfig.instance.runInDebugBuildOnly)
                return false;

            RegisterLogCallBack();
            return true;
#pragma warning restore
        }

        private static bool isCallbackRegistered = false;
        private static void RegisterLogCallBack()
        {
            if (isCallbackRegistered)
                return;

            isCallbackRegistered = true;
            HandleLog handleLog = GameObject.FindObjectOfType<HandleLog>();
            if (handleLog == null)
            {
                GameObject go = new GameObject("HandleLog");
                Object.DontDestroyOnLoad(go);
                handleLog = go.AddComponent<HandleLog>();
            }
        }


        public static void Log(object message)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all)
                UnityEngine.Debug.Log(message.ToString());
        }

        public static void Log(object message, UnityEngine.Object context)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all)
                UnityEngine.Debug.Log(message.ToString(), context);
        }

        public static void LogWarning(object message)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all || warningLevel == DebugWarningLevel.warning)
                UnityEngine.Debug.LogWarning(message.ToString());
        }

        public static void LogWarning(object message, UnityEngine.Object context)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all || warningLevel == DebugWarningLevel.warning)
                UnityEngine.Debug.LogWarning(message.ToString(), context);
        }

        public static void LogError(object message)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all || warningLevel == DebugWarningLevel.error)
                UnityEngine.Debug.LogError(message.ToString());
        }

        public static void LogError(object message, UnityEngine.Object context)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all || warningLevel == DebugWarningLevel.error)
                UnityEngine.Debug.LogError(message.ToString(), context);
        }
    }



    public class FailureHandler
    {
        public static void HandleFailure<T>(T classCaller)
        {
           
#pragma warning disable 0162
            if (classCaller.GetType() == typeof(Assert).GetType())
            {
                if (!AssertDebugConfig.instance.shouldAssertDoFailure)
                    return;

                if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.assertFailureOnly ||
                    AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.WriteToLog(AssertDebugConfig.instance.GetDebugFileName());
                }
                

                switch (AssertDebugConfig.instance.assertFailureReaction)
                {
                    case FailureReaction.pause:
#if UNITY_EDITOR
                        UnityEngine.Debug.Break();
#elif NG_RIGIDBODY_EXTENSION
                        RigidbodyPauser.PauseAll();
#endif
                        break;
                    case FailureReaction.kill:
                        Application.Quit();
#if UNITY_EDITOR
                        UnityEngine.Debug.Break();
#endif
                        break;
                    case FailureReaction.none:
                        return;
                    default:
                        return;
                }
            }

            if (typeof(Debug).GetType() == classCaller.GetType())
            {
                if (!AssertDebugConfig.instance.shouldLogErrorDoFailure)
                    return;

                if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.debugErrorOnly ||
                    AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.WriteToLog(AssertDebugConfig.instance.GetDebugFileName());
                }

                switch (AssertDebugConfig.instance.debugReaction)
                {
                    case FailureReaction.pause:
#if UNITY_EDITOR
                        UnityEngine.Debug.Break();
#elif NG_RIGIDBODY_EXTENSION
                        RigidbodyPauser.PauseAll();
#endif
                        break;
                    case FailureReaction.kill:
                        Application.Quit();
#if UNITY_EDITOR
                        UnityEngine.Debug.Break();
#endif
                        break;
                    case FailureReaction.none:
                        return;
                    default:
                        return;
                }
            }

            if (AssertDebugConfig.instance.takeScreenShot)
                Application.CaptureScreenshot(AssertDebugConfig.instance.GetScreenShotFileName());
#pragma warning restore


        }
    }


    public class DebugToFile
    {
        /// <summary>
        /// Add strings to this to dump all into a file at a later time
        /// </summary>
        public static List<string> dOut = new List<string>();

        /// <summary>
        /// Add strings to this to dump all into a file at a later time
        /// Only for use by NG.Assert and NG.Debug
        /// </summary>
        public static List<string> dOutDebugAssert = new List<string>();

        /// <summary>
        /// Writes all dOut strings to file
        /// </summary>
        public static void WriteToLog(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (string s in dOut)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
            }

            dOut = new List<string>(); 
        }


        /// <summary>
        /// For use only in NG.Assert and NG.Debug
        /// </summary>
        public static void WriteToLogAssertDebug(string fileName)
        {
            bool writeHeader = false;
            if (!File.Exists(fileName))
                writeHeader = true;

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                if (writeHeader)
                    sw.WriteLine(AssertDebugConfig.instance.debugFileHeader);

                foreach (string s in dOutDebugAssert)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
            }

            dOutDebugAssert = new List<string>();
        }

        /// <summary>
        /// Writes to output.csv - must add strings to dOut list
        /// </summary>
        public static void WriteToLog()
        {
            WriteToLog(Application.dataPath + "/output.csv");
        }

        /// <summary>
        /// Writes a single line to log file - for use only with NG.Assert and NG.Debug
        /// </summary>
        public static void WriteLineToLogAssertDebug(string fileName, string line)
        {
            bool writeHeader = false;
            if (!File.Exists(fileName))
                writeHeader = true;

            using (StreamWriter sw = new StreamWriter(fileName , true))
            {
                if (writeHeader)
                    sw.WriteLine(AssertDebugConfig.instance.debugFileHeader);

                sw.WriteLine(line);
                sw.Close();
            }
        }

        /// <summary>
        /// Writes a single line to log file
        /// </summary>
        public static void WriteLineToLog(string fileName, string line)
        {
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(line);
                sw.Close();
            }
        }

        public static string CleanLine(string line)
        {
            line = line.Replace("\n", " ");
            line = line.Replace("\r", " ");
            line = line.Replace("\t", " ");
            return line;
        }

        public static void HandleLogCallback(string logString, string stackTrace, LogType type)
        {
#pragma warning disable 0162
#pragma warning disable 0429
            if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.none)
                return;

            string line = AssertDateStamp.LogEntryTime() + "\t" +
                            type.ToString() + "\t" +
                            DebugToFile.CleanLine(logString) + "\t" +
                            DebugToFile.CleanLine(stackTrace);

            if (type == LogType.Assert)
            {
                if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.all ||
                    AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.assertOnly)
                {
                    DebugToFile.WriteLineToLogAssertDebug(AssertDebugConfig.instance.GetDebugFileName(), line);
                }
                else if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.assertFailureOnly ||
                         AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.dOutDebugAssert.Add(line);
                }
            }
            else
            {
                if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.all ||
                    AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.assertOnly)
                {
                    DebugToFile.WriteLineToLogAssertDebug(AssertDebugConfig.instance.GetDebugFileName(), line);
                }
                else if (AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.debugErrorOnly ||
                         AssertDebugConfig.instance.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.dOutDebugAssert.Add(line);
                }
            }
#pragma warning restore
        }
    }

}


