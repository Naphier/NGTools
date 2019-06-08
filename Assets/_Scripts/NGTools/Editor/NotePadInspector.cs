using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NotePad))]
public class NotePadInspector : Editor
{
    private Vector2 scroll;


    void OnEnable()
    {
        NotePad notePad = (NotePad)target;
        HandleTransformVisibility(notePad);
    }

    public override void OnInspectorGUI()
    {
        NotePad notePad = (NotePad)target;

        EditorGUI.BeginChangeCheck();
        notePad.showTransform = EditorGUILayout.ToggleLeft("Show transform", notePad.showTransform);
        if (EditorGUI.EndChangeCheck())
        {
            HandleTransformVisibility(notePad);
            
        }

        EditorGUILayout.LabelField("Notes", EditorStyles.boldLabel);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        notePad.text = EditorGUILayout.TextArea(notePad.text , GUILayout.Height(Screen.height));
        EditorGUILayout.EndScrollView();

        
    }

    private void HandleTransformVisibility(NotePad notePad)
    {
        if (notePad.showTransform)
            notePad.transform.hideFlags = HideFlags.None;
        else
            notePad.transform.hideFlags = HideFlags.HideInInspector;

        
    }
}