using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;

namespace NG
{
    public class FileLogger : ILogger
    {
        public FileLogger(bool enabled, NGLogType filterLogType, string path = null, 
            string filename = null, StackTraceType stackTraceType = StackTraceType.none)
        {
            logEnabled = enabled;
            this.ngFilterLogType = filterLogType;
            _logHandler = new FileLogHandler(path, filename, stackTraceType: stackTraceType);
        }

        public NGLogType ngFilterLogType = NGLogTypeExt.All;
        public LogType filterLogType { get; set; }

        private bool _logEnabled = true;
        public bool logEnabled { get {return _logEnabled; } set { _logEnabled = value; } }

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

            logHandler.LogFormat(logType, context, FileLogHandler.DELIMITER + format, args);
        }

        public void Log(LogType logType, string tag, object message, UnityEngine.Object context)
        {
            if (!IsLogTypeAllowed(logType))
                return;

            string composed = tag + FileLogHandler.DELIMITER + message;

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


    public enum StackTraceType { none, simple, verbose}
    public class FileLogHandler : ILogHandler
    {
        public const string DELIMITER = "|";

        public bool showConsoleMessages = false;

        public StackTraceType stackTraceType = StackTraceType.none;
        private string GetStackTrace()
        {
            StackTrace stackTrace;
            string output = "";
            switch (stackTraceType)
            {
                case StackTraceType.simple:
                    stackTrace = new StackTrace();
                    output = stackTrace.ToString().Replace("\t", "");
                    output = output.Replace("\r\n", "");
                    output = DELIMITER + output.Replace("   ", " ");
                    break;
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

        public static string DefaultPath
        {
            get
            {
                return Application.dataPath;
            }
        }

        public const string DefaultFileName = "debug.log";

        public string path { get; private set; }
        public string filename { get; private set; }

        string fullpath;
        public FileLogHandler(string path = null, string filename = null, bool showConsoleMessages = false, StackTraceType stackTraceType = StackTraceType.none)
        {
            this.showConsoleMessages = showConsoleMessages;
            this.stackTraceType = stackTraceType;
            if (string.IsNullOrEmpty(path))
                path = DefaultPath;
            this.path = path;

            if (string.IsNullOrEmpty(filename))
                filename = DefaultFileName;
            this.filename = DefaultFileName;

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                fullpath = Path.Combine(path, filename);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        public string FileHeader
        {
            get
            {
                string header = string.Format(
                    "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}",
                    DELIMITER, 
                    "DATE",
                    "TIME",
                    "LEVEL",
                    "TAG",
                    "MESSAGE",
                    "CONTEXT"
                    );

                if (stackTraceType == StackTraceType.simple)
                    header += DELIMITER + "TRACE";

                return header;
            }
        }

        bool creationMessageOnce = false;
        bool checkShouldWriteHeader = true;
        public void WriteToFile(string message)
        {
            try
            {
                bool shouldWriteHeader = false;
                if (checkShouldWriteHeader)
                {
                    checkShouldWriteHeader = false;
                    if (!File.Exists(fullpath))
                        shouldWriteHeader = true;
                }

                using (StreamWriter streamWriter = new StreamWriter(fullpath, true))
                {
                    if (!creationMessageOnce)
                    {
                        creationMessageOnce = true;
                        UnityEngine.Debug.LogFormat("Created log at: {0}", fullpath);
                    }

                    if (shouldWriteHeader)
                    {
                        streamWriter.WriteLine(FileHeader);
                    }

                    streamWriter.WriteLine(message);
                }
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
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

            WriteToFile(output);

            
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

            WriteToFile(output);
        }
    }
}
