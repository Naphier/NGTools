using UnityEngine;

namespace NG.AssertDebug
{
    /// <summary>
    /// Replacement class that "extends" UnityEngine.Assertions.
    /// Is not executed if not running in the editor and not a debug build.
    /// </summary>
    public class Assert
    {  
        private static bool ShouldExecute()
        {
#pragma warning disable 0162
            if (AssertDebugConfig.instance.masterDisable || AssertDebugConfig.instance.assertDisable)
                return false;

#if UNITY_EDITOR
            RegisterLogCallBack();
            return true;
#endif

            if (!Debug.isDebugBuild && AssertDebugConfig.instance.runInDebugBuildOnly)
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


        #region AreEqual
        public delegate void AreEqualDelegate();
        public static AreEqualDelegate AreEqualCallback;
        /// <summary>
        /// Asserts that two values are equal and disables the referenced MonoBehaviour.
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

                if (AreEqualCallback != null)
                    AreEqualCallback();

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool AreEqual<T>(T expected, T actual)
        {
            return AreEqual(expected, actual, null);
        }
        #endregion


        #region AreApproximatelyEqual
        public delegate void AreApproximatelyEqualDelegate();
        public static AreApproximatelyEqualDelegate AreApproximatelyEqualCallback;
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

                if (AreApproximatelyEqualCallback != null)
                    AreApproximatelyEqualCallback();

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
        #endregion


        #region AreNotApproximatelyEqual
        public delegate void AreNotApproximatelyEqualDelegate();
        public static AreNotApproximatelyEqualDelegate AreNotApproximatelyEqualCallback;
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

                if (AreNotApproximatelyEqualCallback != null)
                    AreNotApproximatelyEqualCallback();

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
        #endregion


        #region AreNotEqual
        public delegate void AreNotEqualDelegate();
        public static AreNotEqualDelegate AreNotEqualCallback;
        public static bool AreNotEqual<T>(T expected, T actual, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (Equals(expected, actual))
            {
                UnityEngine.Assertions.Assert.AreNotEqual(expected, actual);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                if (AreNotEqualCallback != null)
                    AreNotEqualCallback();

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool AreNotEqual<T>(T expected, T actual)
        {
            return AreNotEqual(expected, actual, null);
        }
        #endregion


        #region IsFalse
        public delegate void IsFalseDelegate();
        public static IsFalseDelegate IsFalseCallback;
        public static bool IsFalse(bool condition, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (condition)
            {
                UnityEngine.Assertions.Assert.IsFalse(condition);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                if (IsFalseCallback != null)
                    IsFalseCallback();

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool IsFalse(bool condition)
        {
            return IsFalse(condition, null);
        }
        #endregion


        #region IsTrue
        public delegate void IsTrueDelegate();
        public static IsTrueDelegate IsTrueCallback;
        public static bool IsTrue(bool condition, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (!condition)
            {
                UnityEngine.Assertions.Assert.IsTrue(condition);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                if (IsTrueCallback != null)
                    IsTrueCallback();

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool IsTrue(bool condition)
        {
            return IsTrue(condition, null);
        }
        #endregion


        #region IsNotNull
        public delegate void IsNotNullDelegate();
        public static IsNotNullDelegate IsNotNullCallback;
        public static bool IsNotNull(object value, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (value == null)
            {
                UnityEngine.Assertions.Assert.IsNotNull(value);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                if (IsNotNullCallback != null)
                    IsNotNullCallback();

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool IsNotNull(object value)
        {
            return IsNotNull(value, null);
        }
        #endregion


        #region IsNull
        public delegate void IsNullDelegate();
        public static IsNotNullDelegate IsNullCallback;
        public static bool IsNull(object value, MonoBehaviour monoBehaviour)
        {
            if (!ShouldExecute())
                return true;

            if (value != null)
            {
                UnityEngine.Assertions.Assert.IsNull(value);
                if (monoBehaviour != null)
                    monoBehaviour.enabled = false;

                if (IsNotNullCallback != null)
                    IsNotNullCallback();

                FailureHandler.HandleFailure(typeof(Assert));
                return false;
            }

            return true;
        }

        public static bool IsNull(object value)
        {
            return IsNull(value, null);
        }
        #endregion
    }
}
