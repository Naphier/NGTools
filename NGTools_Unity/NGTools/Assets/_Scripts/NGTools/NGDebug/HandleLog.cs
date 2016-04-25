using UnityEngine;
using NG.AssertDebug;

using LogType = UnityEngine.LogType;
/// <summary>
/// Handle log messages callbacks for NG.Assert and NG.Debug
/// Use this script as the bridge between NGAssertDebug and NGDebugConsole
/// Make any custom settings and assign callbacks here.
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
        // NG.AssertDebugConfig to not take screenshots -- should be done in AssertDebugConfig and saved to personal settings
        AssertDebugConfig.instance.takeScreenShot = false;

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

        if (AssertDebugConfig.instance.ShowGuiLogConsoleOnError())
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
        if (Input.GetKeyDown(AssertDebugConfig.instance.debugConsoleKey))
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
