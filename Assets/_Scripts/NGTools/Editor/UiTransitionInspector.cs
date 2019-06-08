using UnityEngine;
using UnityEditor;

namespace NG.UI
{
    [CustomEditor(typeof(UiTransition))]
    public class UiTransitionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UiTransition uiTransitions = (UiTransition)target;
            GUI.enabled = EditorApplication.isPlaying;
            
            if (GUILayout.Button("Transition"))
            {
                uiTransitions.Toggle();
            }
            GUI.enabled = true;
        }
    }
}