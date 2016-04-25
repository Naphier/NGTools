using UnityEngine;
namespace NG
{
    public enum FileLogPathType { Custom, ApplicationDataPath, PersistentDataPath }

    public enum Platform { Editor = 1 , Windows = 2, OSX = 4, iOS = 8, Android = 16, Web = 32}

    public static class PlatformEnumExt
    {
        public static bool IsRuntimePlatform(this Platform platform)
        {
            if (((platform & Platform.Editor) == Platform.Editor) &&
                (Application.platform == RuntimePlatform.OSXEditor ||
                 Application.platform == RuntimePlatform.WindowsEditor))
            {
                return true;
            }

            if (((platform & Platform.Windows) == Platform.Windows) &&
                Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return true;
            }

            if (((platform & Platform.OSX) == Platform.OSX) &&
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                return true;
            }

            if (((platform & Platform.iOS) == Platform.iOS) &&
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return true;
            }

            if (((platform & Platform.Android) == Platform.Android) &&
                Application.platform == RuntimePlatform.Android)
            {
                return true;
            }

            if (((platform & Platform.Web) == Platform.Web) &&
                (Application.platform == RuntimePlatform.WebGLPlayer ||
                Application.platform == RuntimePlatform.OSXWebPlayer ||
                Application.platform == RuntimePlatform.WindowsWebPlayer))
            {
                return true;
            }

            return false;
        }
    }

    public class NGDebugLoggerSettings : ScriptableObject
    {
        public Platform platforms = Platform.Editor | Platform.Windows | Platform.OSX | Platform.iOS | Platform.Android | Platform.Web;

        public LogDestinationType logDestinations =
            LogDestinationType.editorConsole | LogDestinationType.file | LogDestinationType.guiConsole;

        public DisableContext disableContext = 0;

        public NGLogType consoleLogFilter = Debug.noFilter;
        public NGLogType fileLogFilter = Debug.noFilter;
        public NGLogType guiLogFilter = Debug.noFilter;

        public bool consoleLogEnabled = true;
        public bool fileLogEnabled = true;
        public bool guiLogEnabled = true;


        public FileLogPathType fileLogPathBaseType = FileLogPathType.ApplicationDataPath;
        public string fileLogPathBase
        {
            get
            {
                switch (fileLogPathBaseType)
                {
                    case FileLogPathType.Custom:
                        return custompath;
                    case FileLogPathType.ApplicationDataPath:
                        return Application.dataPath;
                    case FileLogPathType.PersistentDataPath:
                    default:
                        return Application.persistentDataPath;
                }
            }
        }

        public string custompath = "";

        public string fileLogName = "debug.log";

        public StackTraceType stackTrace = StackTraceType.none;
    }

    
    
}
