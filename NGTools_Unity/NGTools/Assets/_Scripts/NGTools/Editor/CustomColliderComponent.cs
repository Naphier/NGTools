/*
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(BoxCollider))]
public class CustomColliderComponent : Editor
{
    public override void OnInspectorGUI()
    {
        BoxCollider bc = (BoxCollider)target;

        GUILayout.Label("OnInspectorGUI base");
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("DrawDefaultInspector");
        DrawDefaultInspector();

        
        //http://forum.unity3d.com/threads/custom-inspector-for-colliders.365769/#post-2369536
        
        //EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Collider", bc.bounds , (Editor)this);


    }
}
*/