using System;
using System.Diagnostics;
using UnityEngine;

namespace NG.Logging
{
    public class GuiLogger : ILogger
    {
        public GuiLogger(bool enabled, NGLogType filterLogType, StackTraceType stackTraceType = StackTraceType.none)
        {
            logEnabled = enabled;
            this.ngFilterLogType = filterLogType;
            _logHandler = new GuiLogHandler(stackTraceType: stackTraceType);
        }

        public NGLogType ngFilterLogType = NGLogTypeExt.All;
        public LogType filterLogType { get; set; }

        private bool _logEnabled = true;
        public bool logEnabled { get { return _logEnabled; } set { _logEnabled = value; } }

        private ILogHandler _logHandler;
        public ILogHandler logHandler { get { return _logHandler; } set { /* Not allowed */} }

        public bool IsLogTypeAllowed(LogType logType)
        {
            return logEnabled && ((ngFilterLogType & logType.ToNGLogType()) == logType.ToNGLogType());
        }

        #region default methods
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (!IsLogTypeAllowed(logType))
                return;

            logHandler.LogFormat(logType, context, GuiLogHandler.DELIMITER + format, args);
        }

        public void Log(LogType logType, string tag, object message, UnityEngine.Object context)
        {
            if (!IsLogTypeAllowed(logType))
                return;

            string composed = tag + GuiLogHandler.DELIMITER + message;

            logHandler.LogFormat(logType, context, composed);
        }
        #endregion

        #region Only used internally
        public void Log(LogType logType, object message)
        {
            Log(logType, null, message, null);
        }

        public void Log(LogType logType, object message, UnityEngine.Object context)
        {
            Log(logType, null, message, context);
        }

        public void Log(LogType logType, string tag, object message)
        {
            Log(logType, tag, message, null);
        }

        public void LogFormat(LogType logType, string format, params object[] args)
        {
            LogFormat(logType, null, format, args);
        }
        #endregion

        public void Log(object message)
        {
            Log(LogType.Log, null, message, null);
        }

        public void Log(string tag, object message)
        {
            Log(LogType.Log, tag, message, null);
        }

        public void Log(string tag, object message, UnityEngine.Object context)
        {
            Log(LogType.Log, tag, message, context);
        }

        public void LogWarning(string tag, object message, UnityEngine.Object context)
        {
            Log(LogType.Warning, tag, message, context);
        }

        public void LogWarning(string tag, object message)
        {
            Log(LogType.Warning, tag, message);
        }

        public void LogError(string tag, object message)
        {
            Log(LogType.Error, tag, message);
        }

        public void LogError(string tag, object message, UnityEngine.Object context)
        {
            Log(LogType.Error, tag, message, context);
        }

        public void LogException(Exception exception)
        {
            LogException(exception, null);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            if (!IsLogTypeAllowed(LogType.Exception))
                return;

            logHandler.LogException(exception, context);
        }
    }


    public class GuiLogHandler : ILogHandler
    {
        public bool showConsoleMessages = false;
        public const string DELIMITER = "\t";

        public StackTraceType stackTraceType = StackTraceType.none;
        private string GetStackTrace()
        {
            StackTrace stackTrace;
            string output = "";
            switch (stackTraceType)
            {
                case StackTraceType.simple:
                case StackTraceType.verbose:
                    stackTrace = new StackTrace(true);
                    output = "\n" + stackTrace.ToString();
                    break;
                case StackTraceType.none:
                default:
                    break;
            }

            return output;
        }

        public GuiLogHandler(bool showConsoleMessages = false, StackTraceType stackTraceType = StackTraceType.none)
        {
            this.showConsoleMessages = showConsoleMessages;
            this.stackTraceType = stackTraceType;
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            string output = string.Format(
                "{1}{0}{2}{0}{3}{0}{0}{4}{0}",
                DELIMITER,
                DateTime.Now.ToString("yyyyMMdd"),
                DateTime.Now.ToString("HH:MM:ss"),
                LogType.Exception.ToString().ToUpper(),
                exception
                );

            if (context != null)
                output += context.name;

            output += GetStackTrace();

            if (showConsoleMessages)
                UnityEngine.Debug.LogError(output, context);

            GuiDebugDisplay.instance.AddMessage(output, LogType.Exception);

        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            string output = string.Format(
                "{1}{0}{2}{0}{3}{0}{4}{0}",
                DELIMITER,
                DateTime.Now.ToString("yyyyMMdd"),
                DateTime.Now.ToString("HH:MM:ss"),
                logType.ToString().ToUpper(),
                string.Format(format, args)
                );

            if (context != null)
                output += context.name;

            output += GetStackTrace();

            if (showConsoleMessages)
                UnityEngine.Debug.LogError(output, context);

            GuiDebugDisplay.instance.AddMessage(output, logType);
        }
    }
}
