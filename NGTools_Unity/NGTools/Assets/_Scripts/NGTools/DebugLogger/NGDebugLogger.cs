using System;
using UnityEngine;

namespace NG
{
    public enum LogDestinationType { editorConsole = 1, file = 2, guiConsole = 4}
    public enum DisableContext { onError = 1, onException = 2, onFailedAssertion = 4}

    public enum NGLogType { Assert = 1, Error = 2, Exception = 4, Log = 8, Warning = 16}

    public static class Debug
    {
        public static NGLogType noFilter
        {
            get
            {
                return NGLogType.Assert | NGLogType.Error | NGLogType.Exception | NGLogType.Log | NGLogType.Warning;
            }
        }

        public static LogDestinationType logDestinations =
            LogDestinationType.editorConsole | LogDestinationType.file | LogDestinationType.guiConsole;

        public static DisableContext disableContext = 
            DisableContext.onError | DisableContext.onException | DisableContext.onFailedAssertion;

        private static ILogger _consoleLogger = null;
        public static ILogger consoleLogger
        {
            get
            {
                Init();
                if (_consoleLogger == null)
                {
                    _consoleLogger = UnityEngine.Debug.logger;
                    _consoleLogger.logEnabled = consoleLogEnabled;
                    _consoleLogger.filterLogType = (LogType.Assert | LogType.Error | LogType.Exception | LogType.Log | LogType.Warning);
                }
                return _consoleLogger;
            }
        }

        private static FileLogger _fileLogger = null;
        public static FileLogger fileLogger
        {
            get
            {
                Init();

                if (_fileLogger == null)
                {
                    _fileLogger = new FileLogger(fileLogEnabled, fileLogFilter);
                }

                return _fileLogger;
            }
            set
            {
                _fileLogger = value;
            }
        }

        private static GuiLogger _guiLogger = null;
        public static GuiLogger guiLogger
        {
            get
            {
                Init();

                if (_guiLogger == null)
                {
                    _guiLogger = new GuiLogger(guiLogEnabled, guiLogFilter);
                }

                return _guiLogger;
            }
            set
            {
                _guiLogger = value;
            }
        }

        private static NGLogType _consoleLogFilter = noFilter;
        public static NGLogType consoleLogFilter
        {
            get
            {
                Init();
                return _consoleLogFilter;
            }
            set
            {
                _consoleLogFilter = value;
            }
        }

        private static NGLogType _fileLogFilter = noFilter;
        public static NGLogType fileLogFilter
        {
            get
            {
                Init();
                return _fileLogFilter;
            }
            set
            {
                _fileLogFilter = value;
                fileLogger.ngFilterLogType = value;
            }
        }

        private static NGLogType _guiLogFilter = noFilter;
        public static NGLogType guiLogFilter
        {
            get
            {
                Init();
                return _guiLogFilter;
            }
            set
            {
                _guiLogFilter = value;
                guiLogger.ngFilterLogType = value;
            }
        }

        public static bool _consoleLogEnabled = true;
        public static bool consoleLogEnabled
        {
            get
            {
                Init();
                return _consoleLogEnabled;
            }
            set
            {
                consoleLogger.logEnabled = value;
                _consoleLogEnabled = value;
            }
        }

        private static bool _fileLogEnabled = true;
        public static bool fileLogEnabled
        {
            get
            {
                Init();
                return _fileLogEnabled;
            }
            set
            {
                fileLogger.logEnabled = value;
                _fileLogEnabled = value;
            }
        }

        private static bool _guiLogEnabled = true;
        public static bool guiLogEnabled
        {
            get
            {
                Init();
                return _guiLogEnabled;
            }
            set
            {
                _guiLogEnabled = value;
                guiLogger.logEnabled = value;
            }
        }

        public static bool ShouldAttemptDisableContext(DisableContext currentContext)
        {
            return ((disableContext & currentContext) == currentContext);
        }

        public static void AttemptDisableContext(DisableContext currentContext, UnityEngine.Object context)
        {
            if (!ShouldAttemptDisableContext(currentContext))
                return;

            if (context == null)
                return;

            try
            {
                MonoBehaviour monobehaviour = (MonoBehaviour)context;
                if (monobehaviour != null)
                {
                    monobehaviour.enabled = false;
                    return;
                }
            }
            catch(Exception){ }

            try
            {
                GameObject gameObject = (GameObject)context;
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }
            catch(Exception){ }
        }

        // TODO
        /// <summary>
        /// Use this later for loading settings
        /// </summary>
        private static bool didInit = false;
        private static void Init()
        {
            if (didInit)
                return;
            didInit = true;
            NGDebugLoggerSettingsLoader.LoadSettings();
        }

        public static bool ShouldLogToConsole(LogType logType)
        {
            if (!consoleLogEnabled)
                return false;

            if ((logDestinations & LogDestinationType.editorConsole) != LogDestinationType.editorConsole)
                return false;

            NGLogType ngLogType = logType.ToNGLogType();

            if ((consoleLogFilter & ngLogType) != ngLogType)
                return false;
            
            return true;
        }

        public static bool shouldLogToFile
        {
            get
            {
                return 
                    fileLogEnabled &&
                    ((logDestinations & LogDestinationType.file) == LogDestinationType.file);
            }
        }


        public static bool ShouldLogToGuiConsole(LogType logType)
        {
            if (!guiLogEnabled)
                return false;

            if ((logDestinations & LogDestinationType.guiConsole) != LogDestinationType.guiConsole)
                return false;

            NGLogType ngLogType = logType.ToNGLogType();

            if ((guiLogFilter & ngLogType) != ngLogType)
                return false;

            return true;
        }

        public static string TimeStamp
        {
            get
            {
                return "[" + DateTime.Now.ToString() + "]";
            }
        }

        public static string FormatTagConsole(string tag)
        {
            string compose = TimeStamp;
            if (!string.IsNullOrEmpty(tag))
                compose += " [" + tag + "]";
            return compose; 
        }

        #region LOG
        public static void Log(string tag, object message, UnityEngine.Object context)
        {
            string formattedTag = FormatTagConsole(tag);
            if (ShouldLogToConsole(LogType.Log))
            {
                consoleLogger.Log(formattedTag, message, context);
            }

            if (shouldLogToFile)
                fileLogger.Log(tag, message, context);

            if (ShouldLogToGuiConsole(LogType.Log))
                guiLogger.Log(formattedTag, message, context);
        }

        public static void Log(object message) { Log(null, message, null); }

        public static void Log(object message, UnityEngine.Object context)
        { Log(null, message, context); }

        public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (ShouldLogToConsole(LogType.Log))
                consoleLogger.LogFormat(LogType.Log, context, TimeStamp + ": " + format, args);

            if (shouldLogToFile)
                fileLogger.LogFormat(LogType.Log, context, format, args);

            if (ShouldLogToGuiConsole(LogType.Log))
                guiLogger.LogFormat(LogType.Log, context, TimeStamp + ": " + format, args);
        }

        public static void LogFormat(string format, params object[] args)
        { LogFormat(null, format, args); }
        #endregion


        #region WARNING
        public static void LogWarning(string tag, object message, UnityEngine.Object context)
        {
            string formattedTag = FormatTagConsole(tag);
            if (ShouldLogToConsole(LogType.Warning))         
                consoleLogger.LogWarning(formattedTag, message, context);

            if (shouldLogToFile)
                fileLogger.LogWarning(tag, message, context);

            if (ShouldLogToGuiConsole(LogType.Warning))
                guiLogger.LogWarning(formattedTag, message, context);
        }

        public static void LogWarning(object message) { LogWarning(null, message, null); }

        public static void LogWarning(object message, UnityEngine.Object context)
        { LogWarning(null, message, context); }

        public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (ShouldLogToConsole(LogType.Warning))
                consoleLogger.LogFormat(LogType.Warning, context, TimeStamp + ": " + format, args);

            if (shouldLogToFile)
                fileLogger.LogFormat(LogType.Warning, context, format, args);

            if (ShouldLogToGuiConsole(LogType.Warning))
                guiLogger.LogFormat(LogType.Warning, context, TimeStamp + ": " + format, args);
        }

        public static void LogWarningFormat(string format, params object[] args)
        { LogWarningFormat(null, format, args); }
        #endregion


        #region ERROR
        public static void LogError(string tag, object message, UnityEngine.Object context)
        {
            string formattedTag = FormatTagConsole(tag);
            if (ShouldLogToConsole(LogType.Error))
                consoleLogger.LogError(formattedTag, message, context);

            if (shouldLogToFile)
                fileLogger.LogError(tag, message, context);

            if (ShouldLogToGuiConsole(LogType.Error))
                guiLogger.LogError(formattedTag, message, context);

            AttemptDisableContext(DisableContext.onError, context);
        }

        public static void LogError(object message) { LogError(null, message, null); }

        public static void LogError(object message, UnityEngine.Object context)
        { LogError(null, message, context); }

        public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (ShouldLogToConsole(LogType.Error))
                consoleLogger.LogFormat(LogType.Error, context, TimeStamp + ": " + format, args); ;

            if (shouldLogToFile)
                fileLogger.LogError(null, string.Format(format, args), context);

            if (ShouldLogToGuiConsole(LogType.Error))
                guiLogger.LogFormat(LogType.Error, context, TimeStamp + ": " + format, args);

            AttemptDisableContext(DisableContext.onError, context);
        }

        public static void LogErrorFormat(string format, params object[] args)
        { LogErrorFormat(null, format, args); }
        #endregion


        #region EXCEPTION
        public static void LogException(Exception exception, UnityEngine.Object context)
        {
            if (ShouldLogToConsole(LogType.Exception))
                consoleLogger.LogException(exception, context);

            if (shouldLogToFile)
                fileLogger.LogException(exception, context);

            if (ShouldLogToGuiConsole(LogType.Error))
                guiLogger.LogException(exception, context);

            AttemptDisableContext(DisableContext.onException, context);
        }

        public static void LogException(Exception exception) { LogException(exception, null); }
        #endregion

        // I think these might be better in a custom assertion class
        /*
        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertion(object message) { }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertion(object message, UnityEngine.Object context) { }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertionFormat(string format, params object[] args) { }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args) { }
        */
    }

    public static class NGLogTypeExt
    {
        public static NGLogType All = NGLogType.Assert | NGLogType.Error | NGLogType.Exception | NGLogType.Log | NGLogType.Warning;

        public static NGLogType ToNGLogType(this LogType unityLogType)
        {
            switch (unityLogType)
            {
                case LogType.Error:
                    return NGLogType.Error;
                case LogType.Assert:
                    return NGLogType.Assert;
                case LogType.Warning:
                    return NGLogType.Warning;
                case LogType.Log:
                    return NGLogType.Log;
                case LogType.Exception:
                    return NGLogType.Exception;
                default:
                    return 0;
            }
        }
    }
}
