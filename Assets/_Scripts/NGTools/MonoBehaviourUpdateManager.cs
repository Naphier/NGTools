using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace NG.MonoBehaviourUpdate
{
    /// <summary>
    /// Observer pattern for subscribing to MonoBehaviour.Update.
    /// Includes lightweight diagnostic timers to dive deep into the performance 
    /// of the Observer methods that are notified of MonoBehaviour.Update.
    /// 
    /// Be aware that the use of stopWatchMicro will cause overhead of approximately
    /// 0.0005 milliseconds per observer. While this is a small number it can accumulate
    /// quickly and have minor implications on performance when managing many observers.
    /// 
    /// !IMPORTANT! 
    /// Make sure to unsubcribe your observers when your game objects are destroyed. 
    /// Otherwise the observer will continue to exist and be called.
    /// 
    /// Todo - ensure that correct delegate is removed with corresponding go.
    /// 
    /// </summary>
    public class MonoBehaviourUpdateManager : SingletonMonoBehaviour<MonoBehaviourUpdateManager>
    {
        protected MonoBehaviourUpdateManager() { }
        public delegate void ObserverDelegate();

        public bool displayOnGui = true;
        public bool stopWatchMacroActive = true;
        public bool stopWatchMicroActive = true;

        private List<UpdateObserver> updateObservers = new List<UpdateObserver>();
        public ReadOnlyCollection<UpdateObserver> UpdateObservers;

        private NanoStopWatch stopWatchMacro = new NanoStopWatch();
        private NanoStopWatch stopWatchMicro = new NanoStopWatch();
        public double ms { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            UpdateObservers = updateObservers.AsReadOnly();
        }


        void Update()
        {
            if (stopWatchMacroActive)
                stopWatchMacro.Start();

            if (updateObservers != null)
            {
                for (int i = 0; i < updateObservers.Count; i++)
                {
                    UpdateObserver item = updateObservers[i];
                    if (item != null && item.Observer != null && item.active)
                    {
                        if (stopWatchMicroActive)
                            stopWatchMicro.Start();

                        item.Observer();

                        if (stopWatchMicroActive)
                        {
                            item.ms = stopWatchMicro.ElapsedMilliseconds;
                            stopWatchMicro.Stop();
                        }
                    } 
                }
            }

            if (stopWatchMacroActive)
            {
                ms = stopWatchMacro.ElapsedMilliseconds;
                stopWatchMacro.Stop();
            }
        }


        /// <summary>
        /// Subscribe a UNIQUE observer method to the Update event.<para />
        /// Use this if your method should only ever be subscribed once at
        /// a time in the queue. <para />
        /// You may supply an optional name to assist in keeping track of 
        /// this subscriber.
        /// </summary>
        /// <param name="Observer">A unique Observer delegate.</param>
        /// <param name="name">Optional name of the subscriber</param>
        public void Subscribe(ObserverDelegate Observer, string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = Observer.Method.Name;

            string className = Observer.Method.DeclaringType.ToString();

            UpdateObserver test = updateObservers.Find(a => a != null && (a.name == name || a.Observer == Observer));
            if (test != null)
                return;
            
            UpdateObserver newUpdateObserver = new UpdateObserver(Observer, name, className);
            updateObservers.Add(newUpdateObserver);
            AddToClassNames(className);
        }

        /// <summary>
        /// Subscribe a non-unique observer method to the Update event.<para />
        /// Use this for things like enemy behaviour where many enemies may need 
        /// to be notified of Unity's update event.
        /// </summary>
        /// <param name="Observer">A non-unique Observer delegate</param>
        public void SubscribeNonUnique(ObserverDelegate Observer)
        {
            string name = Observer.Method.Name;
            string className = Observer.Method.DeclaringType.ToString();

            int existingCount = updateObservers.FindAll(a => a != null && (a.name == name || a.Observer == Observer)).Count;

            UpdateObserver newUpdateObserver = new UpdateObserver(Observer, name, className);

            if (existingCount > 0)
                newUpdateObserver.nameExtra = "_" + existingCount.ToString();

            updateObservers.Add(newUpdateObserver);
            AddToClassNames(className);
        }


        /// <summary>
        /// Unsubscribes the first Observer found with the same method.<para />
        /// Optionally use the name parameter to supercede the observer method.
        /// </summary>
        /// <param name="Observer">The unique observer to unsubscribe.</param>
        /// <param name="name">Optional name of the unique observer.</param>
        public void Unsubscribe(ObserverDelegate Observer, string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = Observer.Method.Name;

            UpdateObserver test = updateObservers.Find(a => a != null && (a.name == name || a.Observer == Observer));
            if (test != null)
            {
                updateObservers.Remove(test);
                CleanClassNamesList();
            }
        }


        /// <summary>
        /// Unsubscribes all observers of the same method.
        /// </summary>
        /// <param name="Observer">Observer method to unsubscribe.</param>
        public void UnsubscribeNonUnique(ObserverDelegate Observer)
        {
            bool didRemove = false;
            for (int i = updateObservers.Count - 1; i >= 0; i--)
            {
                if (updateObservers[i].Observer == Observer)
                {
                    updateObservers.RemoveAt(i);
                    didRemove = true;
                }
            }

            if (didRemove)
                CleanClassNamesList();
        }

        /// <summary>
        /// Unsubscribes all observers of the same method by their name.
        /// </summary>
        /// <param name="name">Non-unique name to unsubscribe.</param>
        public void UnsubscribeNonUniqueByName(string name)
        {
            bool didRemove = false;
            for (int i = updateObservers.Count - 1; i >= 0; i--)
            {
                if (updateObservers[i].name == name)
                {
                    updateObservers.RemoveAt(i);
                    didRemove = true;
                }
            }

            if (didRemove)
                CleanClassNamesList();
        }


        public double GetTotalMillisecondsByClassName(string className)
        {
            double ms = 0;
            foreach (var item in updateObservers)
            {
                if (item.className == className && item.active)
                    ms += item.ms;
            }

            return ms;
        }


        private List<string> classNames = new List<string>();
        private void AddToClassNames(string className)
        {
            if (!classNames.Contains(className))
                classNames.Add(className);
        }


        private void CleanClassNamesList()
        {
            for (int i = classNames.Count - 1; i >= 0; i--)
            {
                bool found = false;
                foreach (var item in updateObservers)
                {
                    if (item.className == classNames[i])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    classNames.RemoveAt(i);
            }
        }


        public List<string> GetClassNames()
        {
            return classNames;
        }

        public bool GetAnyActiveByClassName(string className)
        {
            foreach (var item in updateObservers)
            {
                if (item.className == className && item.active)
                    return true;
            }

            return false;
        }

        public void SetAllActiveByClassName(string className, bool active)
        {
            for (int i = 0; i < updateObservers.Count; i++)
            {
                if (updateObservers[i].className == className)
                    updateObservers[i].active = active;
            }
        }


        public void SetObserverActive(ObserverDelegate observer, bool active, string name = null)
        {
            for (int i = 0; i < updateObservers.Count; i++)
            {
                if (updateObservers[i].Observer == observer || 
                    (!string.IsNullOrEmpty(name) && updateObservers[i].name == name))
                    updateObservers[i].active = active;
            }
        }

        #region OnGUI display

        private static Rect fullScreenRect = new Rect(0, 0, Screen.width, Screen.height);

        void OnGUI()
        {
            if (!displayOnGui)
                return;

            GUI.Label(fullScreenRect, GetGuiDisplay());
        }
        
        private string GetGuiDisplay()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Macro timer: {0:N3}ms\n", ms);
            string methodDetails = "";
            foreach (var className in classNames)
            {
                sb.AppendFormat("{0} {1:N3}ms\n", 
                    className, 
                    GetTotalMillisecondsByClassName(className));

                foreach (var updateObserver in updateObservers)
                {
                    if (updateObserver.className == className)
                    {
                        if (updateObserver.active)
                            methodDetails = string.Format(" {0:N3}ms", updateObserver.ms);
                        else
                            methodDetails = " inactive";

                        sb.AppendFormat("\t{0}{1} {2}",
                            updateObserver.name,
                            updateObserver.nameExtra,
                            methodDetails);
                    }
                }
            }

            return sb.ToString();
        }

        #endregion

        #region helper classes
        public class UpdateObserver
        {
            public ObserverDelegate Observer { get; private set; }
            public Guid id { get; private set; }
            public string name { get; private set; }
            public string nameExtra;
            public string className { get; private set; }
            public static readonly Guid guid_dummy = new Guid();
            private bool _active = true;
            public bool active
            {
                get
                {
                    return _active;
                }

                set
                {
                    _active = value;
                    if (!active)
                        ms = 0;
                }

            }
            public double ms = 0;

            public UpdateObserver(ObserverDelegate Observer, string name, string className)
            {
                this.Observer = Observer;
                this.name = name;
                this.className = className;
                id = Guid.NewGuid();
            }
        }

        public class NanoStopWatch
        {
            private long nanosecPerTick;
            private System.Diagnostics.Stopwatch stopwatch;
            public double ElapsedMilliseconds
            {
                get
                {
                    if (stopwatch == null)
                        return 0;
                    else
                        return (stopwatch.ElapsedTicks * nanosecPerTick) / 1000000.0;
                }
            }

            public NanoStopWatch()
            {
                stopwatch = new System.Diagnostics.Stopwatch();
                nanosecPerTick = (1000L * 1000L * 1000L) / System.Diagnostics.Stopwatch.Frequency;
            }

            public void Start() { stopwatch.Reset(); stopwatch.Start(); }
            public void Stop() { stopwatch.Stop(); }
        }
        #endregion
    }
}
