using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxCollider))]
public class CustomColliderComponent : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("OnInspectorGUI base");
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("DrawDefaultInspector");
        DrawDefaultInspector();
    }
}