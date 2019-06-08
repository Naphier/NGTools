using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NG.MonoBehaviourUpdate
{
    public class UpdateObserverManagerInspector : Editor
    {
        [CustomEditor(typeof(MonoBehaviourUpdateManager))]
        public class MonoBehaviourUpdateManager2Inspector : Editor
        {
            private Dictionary<string, bool> classFoldouts = new Dictionary<string, bool>();

            private MonoBehaviourUpdateManager _target;
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                _target = (MonoBehaviourUpdateManager)target;

                string macroDetail = "inactive";
                if (_target.stopWatchMacroActive)
                    macroDetail = string.Format("{0:N3}ms", _target.ms);

                    EditorGUILayout.LabelField("Macro Timer: " + macroDetail);

                List<string> classNames = _target.GetClassNames();
                foreach (var item in classNames)
                {
                    if (!classFoldouts.ContainsKey(item))
                        classFoldouts.Add(item, false);
                }

                string classTimerLabel = "";
                string methodLabel = "";
                string methodTimerLabel = "";

                bool groupActive = false;
                foreach (var item in classNames)
                {
                    if (classFoldouts.ContainsKey(item))
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUI.BeginChangeCheck();

                        groupActive = EditorGUILayout.ToggleLeft("", 
                            _target.GetAnyActiveByClassName(item), 
                            GUILayout.Width(40));
                        
                        if (EditorGUI.EndChangeCheck())
                        {
                            _target.SetAllActiveByClassName(item, groupActive);
                        }

                        if (_target.stopWatchMicroActive)
                            classTimerLabel = string.Format(
                                "{0}  {1:N3}ms",
                                item,
                                _target.GetTotalMillisecondsByClassName(item));
                        else
                            classTimerLabel = item;

                        classFoldouts[item] = EditorGUILayout.Foldout(
                            classFoldouts[item], 
                            classTimerLabel
                            );

                        EditorGUILayout.EndHorizontal();

                        if (classFoldouts[item])
                        {
                            EditorGUI.indentLevel++;
                            foreach (var observer in _target.UpdateObservers)
                            {
                                if (observer.className == item)
                                {
                                    methodLabel = observer.name + observer.nameExtra;
                                    if (_target.stopWatchMicroActive)
                                        methodTimerLabel = string.Format("  {0:N3}ms", observer.ms);
                                    else
                                        methodTimerLabel = "";

                                    observer.active = EditorGUILayout.ToggleLeft(
                                        methodLabel + methodTimerLabel, observer.active);
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                }

                EditorUtility.SetDirty(_target);
            }

            void MakeSpaces(int space)
            {
                for (int i = 0; i < space; i++)
                {
                    EditorGUILayout.Space();
                }
            }
        }
    }
}