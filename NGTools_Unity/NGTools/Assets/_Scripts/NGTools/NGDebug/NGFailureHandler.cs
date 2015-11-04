#define NG_RIGIDBODY_EXTENSION //define this if using NGRigidbodyPauser.cs
using UnityEngine;

namespace NG.AssertDebug
{
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
                        Debug.Break();
#elif NG_RIGIDBODY_EXTENSION
                        RigidbodyPauser.PauseAll();
#endif
                        break;
                    case FailureReaction.kill:
                        Application.Quit();
#if UNITY_EDITOR
                        Debug.Break();
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

                switch (AssertDebugConfig.instance.debugErrorReaction)
                {
                    case FailureReaction.pause:
#if UNITY_EDITOR
                        Debug.Break();
#elif NG_RIGIDBODY_EXTENSION
                        RigidbodyPauser.PauseAll();
#endif
                        break;
                    case FailureReaction.kill:
                        Application.Quit();
#if UNITY_EDITOR
                        Debug.Break();
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
}
