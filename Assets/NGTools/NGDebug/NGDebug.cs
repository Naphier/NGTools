using UnityEngine;

namespace NG.AssertDebug
{
    /// <summary>
    /// Replacement for Unity's debug so we can just shut it off for production.
    /// Is not executed if not running in the editor and not a debug build.
    /// </summary>
    public class NGDebug
    {
        private static DebugWarningLevel warningLevel = AssertDebugConfig.instance.debugLogWarningLevel;
        private static bool ShouldExecute()
        {
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
                Debug.Log(message.ToString());
        }

        public static void Log(object message, Object context)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all)
                Debug.Log(message.ToString(), context);
        }

        public static void LogWarning(object message)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all || warningLevel == DebugWarningLevel.warning)
                Debug.LogWarning(message.ToString());
        }

        public static void LogWarning(object message, Object context)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all || warningLevel == DebugWarningLevel.warning)
                Debug.LogWarning(message.ToString(), context);
        }

        public static void LogError(object message)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all || warningLevel == DebugWarningLevel.error)
                Debug.LogError(message.ToString());

            FailureHandler.HandleFailure(typeof(NGDebug));
        }

        public static void LogError(object message, Object context)
        {
            if (!ShouldExecute()) return;

            if (warningLevel == DebugWarningLevel.all || warningLevel == DebugWarningLevel.error)
                Debug.LogError(message.ToString(), context);

            FailureHandler.HandleFailure(typeof(NGDebug));
        }
    }
}
