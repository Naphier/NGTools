using UnityEngine;
using System.Collections;
using NG;
using Debug = NG.Debug;

public class NGDebugLogger_Tests : MonoBehaviour
{
    void Start()
    {
        UnityEngine.Debug.LogFormat(
            "(logDestinations & LogDestinationType.editorConsole): {0}\n" +
            "(logDestinations & LogDestinationType.file): {1}\n" +
            "(logDestinations & LogDestinationType.guiConsole): {2}\n",
            (Debug.logDestinations & LogDestinationType.editorConsole),
            (Debug.logDestinations & LogDestinationType.file),
            (Debug.logDestinations & LogDestinationType.guiConsole)
            );

        UnityEngine.Debug.LogFormat(
            "shouldLogToFile: {0}" , 
            Debug.shouldLogToFile);

        string consoleFilters = "ShouldLogToConsole:";
        for (int i = 0; i <= 4; i++)
        {
            LogType logType = (LogType)i;
            bool should = Debug.ShouldLogToConsole(logType);
            consoleFilters += string.Format("LogType: {0} -- {1}\n", logType, should);
        }

        UnityEngine.Debug.Log(consoleFilters);

        string guiFilters = "ShouldLogToGuiConsole:";
        for (int i = 0; i <= 4; i++)
        {
            LogType logType = (LogType)i;
            bool should = Debug.ShouldLogToGuiConsole(logType);
            guiFilters += string.Format("LogType: {0} -- {1}\n", logType, should);
        }

        UnityEngine.Debug.Log(guiFilters);

        UnityEngine.Debug.LogFormat(
            "FileLogger.logEnabled: {0}\n" +
            "FileLogger.IsLogTypeAllowed(logType.Log): {1}",
            Debug.fileLogger.logEnabled,
            Debug.fileLogger.IsLogTypeAllowed(LogType.Log)
            );


        GameObject contextObject = new GameObject("context object");

        Debug.Log("Message");
        Debug.Log("Message", contextObject);
        Debug.Log("TAG", "Message", contextObject);
        Debug.LogFormat("{0} {1}", "Log", "Format");
        Debug.LogFormat(contextObject, "{0} {1}", "Log", "Format");

        Debug.LogWarning("Warning");
        Debug.LogWarning("Warning", contextObject);
        Debug.LogWarning("TAG", "Warning", contextObject);
        Debug.LogWarningFormat("{0} {1}", "Warning", "Format");
        Debug.LogWarningFormat(contextObject, "{0} {1}", "Warning", "Format");

        Debug.LogError("Error");
        Debug.LogError("Error", new GameObject("error context object 1"));
        Debug.LogError("TAG", "Error", new GameObject("error context object 2"));
        Debug.LogErrorFormat("{0} {1}", "Error", "Format");
        Debug.LogErrorFormat(new GameObject("error context object 3"), "{0} {1}", "Error", "Format");

        try
        {
            Debug.LogException(new System.Exception());
        }
        catch (System.Exception){}

        try
        {
            Debug.LogException(new System.Exception(), new GameObject("exception context object"));
        }
        catch (System.Exception){}

        Debug.AttemptDisableContext(DisableContext.onError, this);

    }

    void Update()
    {

    }
}
