using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShowLocalAxis))]
public class ShowLocalAxisInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        base.DrawDefaultInspector();

        ShowLocalAxis showRotated = (ShowLocalAxis)target;
        if (showRotated.destroyWhenSafe && Event.current.type == EventType.Repaint)
        {
            DestroyImmediate(showRotated);
            return;
        }
    }
}