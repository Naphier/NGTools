#define NG_RIGIDBODY_EXTENSION
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
    /// all, assertOnly, and debugOnly will always write to file after each message.
    /// allFailureOnly, assertFailureOnly, and debugErrorOnly will only write to 
    ///     file on failure and stores messages in a list before writing.
    /// Log file is tab seperated, so don't use tabs in your log messages!
    /// </summary>
    public enum LogToFileCondition
    { all, assertOnly, debugOnly, allFailureOnly, assertFailureOnly, debugErrorOnly, none }

    public class AssertDebugConfig
    {
        public static bool masterDisable = false;
        public static DebugWarningLevel debugLogWarningLevel = DebugWarningLevel.all;
        public static bool assertDisable = false;
        public static FailureReaction assertFailureReaction = FailureReaction.pause;
        public static FailureReaction debugReaction = FailureReaction.pause;
        public static bool shouldAssertDoFailure = true;
        public static bool shouldLogErrorDoFailure = true;
        public static LogToFileCondition logToFileCondition = LogToFileCondition.all;
        
        public static string logFileName = "debug";
        public static string logFileExt = ".txt";
        public static bool takeScreenShot = false; //todo
        public static string screenShotFileName = "error_screen";
        public static KeyCode debugConsoleKey = KeyCode.BackQuote;

        // show gui in builds only
        public static bool ShowGuiLogConsoleOnError()
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
        public static string GetLogFilePath()
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

        public static string GetDebugFileName()
        {
            return GetLogFilePath() + "/" + logFileName + "_" + AssertDateStamp.FileDate() + logFileExt;
        }

        public static string GetScreenShotFileName()
        {
            int extNum = 0;
            while (File.Exists(ScreenShotFileName(extNum)))
                extNum++;

            return ScreenShotFileName(extNum);
        }

        private static string ScreenShotFileName(int extNum)
        {
            return GetLogFilePath() + "/" + screenShotFileName + "_" + AssertDateStamp.FileDate() + extNum.ToString() + ".png";
        }

        public static string DebugFileHeader()
        {
            return "Time\tError Type\tMessage\tStack Trace";
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

    // let's take this and extend it fully
    //http://wiki.unity3d.com/index.php/DebugConsole
    //http://zaxisgames.blogspot.com/2012/03/let-me-debug-in-your-gui.html
    //

    /// <summary>
    /// Replacement class that "extends" UnityEngine.Assertions
    /// Is not executed if not running in the editor and not a debug build
    /// </summary>
    public class Assert
    {
        private static bool ShouldExecute()
        {
#pragma warning disable 0162
            if (AssertDebugConfig.masterDisable || AssertDebugConfig.assertDisable)
                return false;

#if UNITY_EDITOR
            RegisterLogCallBack();
            return true;
#endif

            if (!UnityEngine.Debug.isDebugBuild)
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
        /// Asserts that two values are equal and disables the referenced MonoBehaviour
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
    /// Replacement for Unity's debug so we can just shut it off for production
    /// </summary>
        public class Debug
    {
        private static DebugWarningLevel warningLevel = AssertDebugConfig.debugLogWarningLevel;
        private static bool ShouldExecute()
        {
#pragma warning disable 0162
            if (AssertDebugConfig.masterDisable || 
                AssertDebugConfig.debugLogWarningLevel == DebugWarningLevel.none)
                return false;

#if UNITY_EDITOR
            RegisterLogCallBack();
            return true;
#endif

            if (!UnityEngine.Debug.isDebugBuild)
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
                if (!AssertDebugConfig.shouldAssertDoFailure)
                    return;

                if (AssertDebugConfig.logToFileCondition == LogToFileCondition.assertFailureOnly ||
                    AssertDebugConfig.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.WriteToLog(AssertDebugConfig.GetDebugFileName());
                }
                

                switch (AssertDebugConfig.assertFailureReaction)
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
                if (!AssertDebugConfig.shouldLogErrorDoFailure)
                    return;

                if (AssertDebugConfig.logToFileCondition == LogToFileCondition.debugErrorOnly ||
                    AssertDebugConfig.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.WriteToLog(AssertDebugConfig.GetDebugFileName());
                }

                switch (AssertDebugConfig.debugReaction)
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
#pragma warning restore

            Application.CaptureScreenshot(AssertDebugConfig.GetScreenShotFileName());
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
                    sw.WriteLine(AssertDebugConfig.DebugFileHeader());

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
                    sw.WriteLine(AssertDebugConfig.DebugFileHeader());

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
            if (AssertDebugConfig.logToFileCondition == LogToFileCondition.none)
                return;

            string line = AssertDateStamp.LogEntryTime() + "\t" +
                            type.ToString() + "\t" +
                            DebugToFile.CleanLine(logString) + "\t" +
                            DebugToFile.CleanLine(stackTrace);

            if (type == LogType.Assert)
            {
                if (AssertDebugConfig.logToFileCondition == LogToFileCondition.all ||
                    AssertDebugConfig.logToFileCondition == LogToFileCondition.assertOnly)
                {
                    DebugToFile.WriteLineToLogAssertDebug(AssertDebugConfig.GetDebugFileName(), line);
                }
                else if (AssertDebugConfig.logToFileCondition == LogToFileCondition.assertFailureOnly ||
                         AssertDebugConfig.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.dOutDebugAssert.Add(line);
                }
            }
            else
            {
                if (AssertDebugConfig.logToFileCondition == LogToFileCondition.all ||
                    AssertDebugConfig.logToFileCondition == LogToFileCondition.assertOnly)
                {
                    DebugToFile.WriteLineToLogAssertDebug(AssertDebugConfig.GetDebugFileName(), line);
                }
                else if (AssertDebugConfig.logToFileCondition == LogToFileCondition.debugErrorOnly ||
                         AssertDebugConfig.logToFileCondition == LogToFileCondition.allFailureOnly)
                {
                    DebugToFile.dOutDebugAssert.Add(line);
                }
            }
#pragma warning restore
        }
    }

}


