using UnityEngine;
using NG;
/// <summary>
/// Handle log messages callbacks for NG.Assert and NG.Debug
/// </summary>
public class HandleLog : MonoBehaviour
{
    void OnEnable()
    {
        Application.logMessageReceived += HandleLogCallback;

        // set DebugConsole options
        DebugConsole.instance.shouldSaveWindowLog = true;
        DebugConsole.instance.shouldTakeScreenShot = true;

        // if taking screenshot via DebugConsole manually then should set
        // NG.AssertDebugConfig to not take screenshots
        AssertDebugConfig.takeScreenShot = false;

        // set button delegates for debug console if needed
        //DebugConsole.instance.ScreenshotButtonCallback += null;
        //DebugConsole.instance.SaveLogButtonCallback += null;
        DebugConsole.instance.ClearLogButtonCallback += ClearAssertDebugLogMessageLists;
    }

    public void HandleLogCallback(string logString, string stackTrace, LogType type)
    {
        // NG Assert/Debug
        DebugToFile.HandleLogCallback(logString, stackTrace, type);

        // Debug Console
        DebugConsole.instance.AddMessage(logString + "\n" + stackTrace, type);

        if (AssertDebugConfig.ShowGuiLogConsoleOnError())
        {
            if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
            {
                DebugConsole.instance.isVisible = true;
            }
        }
    }


    void Update()
    {
        // toggle the debug console
        if (Input.GetKeyDown(AssertDebugConfig.debugConsoleKey))
        {
            DebugConsole.instance.isVisible = !DebugConsole.instance.isVisible;
        }
    }

    void ClearAssertDebugLogMessageLists()
    {
        DebugToFile.dOut.Clear();
        DebugToFile.dOutDebugAssert.Clear();
    }


}
