using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues; //used for "Special Operations" fade group

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class CustomTransformComponent : Editor
{
    private Transform _transform;
    private GUILayoutOption layoutMaxWidth = null;
    public override void OnInspectorGUI()
    {
        if (layoutMaxWidth == null)
            layoutMaxWidth = GUILayout.MaxWidth(600);
            
        //We need this for all OnInspectorGUI sub methods
        _transform = (Transform)target;

        StandardTransformInspector();

        QuaternionInspector();
        
        ShowLocalAxisComponentToggle();

        SpecialOperations();
    }


    private void StandardTransformInspector()
    {
        bool didPositionChange = false;
        bool didRotationChange = false;
        bool didScaleChange = false;

        // Watch for changes. 
        //  1)  Float values are imprecise, so floating point error may cause changes 
        //      when you've not actually made a change.
        //  2)  This allows us to also record an undo point properly since we're only
        //      recording when something has changed.

        // Store current values for checking later
        Vector3 initialLocalPosition = _transform.localPosition;
        Vector3 initialLocalEuler = _transform.localEulerAngles;
        Vector3 initialLocalScale = _transform.localScale;

        EditorGUI.BeginChangeCheck();
        Vector3 localPosition = EditorGUILayout.Vector3Field("Position", _transform.localPosition, layoutMaxWidth);
        if (EditorGUI.EndChangeCheck())
            didPositionChange = true;

        EditorGUI.BeginChangeCheck();
        Vector3 localEulerAngles = EditorGUILayout.Vector3Field(
            "Euler Rotation",
            _transform.localEulerAngles, 
            layoutMaxWidth);

        if (EditorGUI.EndChangeCheck())
            didRotationChange = true;

        EditorGUI.BeginChangeCheck();
        Vector3 localScale = EditorGUILayout.Vector3Field("Scale", _transform.localScale, layoutMaxWidth);
        if (EditorGUI.EndChangeCheck())
            didScaleChange = true;

        // Apply changes with record undo
        if (didPositionChange || didRotationChange || didScaleChange)
        {
            Undo.RecordObject(_transform, _transform.name);

            if (didPositionChange)
                _transform.localPosition = localPosition;

            if (didRotationChange)
                _transform.localEulerAngles = localEulerAngles;

            if (didScaleChange)
                _transform.localScale = localScale;

        }

        // Since BeginChangeCheck only works on the selected object 
        // we need to manually apply transform changes to all selected objects.
        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length > 1)
        {
            foreach (var item in selectedTransforms)
            {
                if (didPositionChange || didRotationChange || didScaleChange)
                    Undo.RecordObject(item, item.name);

                if (didPositionChange)
                {
                    item.localPosition = ApplyChangesOnly(
                        item.localPosition, initialLocalPosition, _transform.localPosition);
                }

                if (didRotationChange)
                {
                    item.localEulerAngles = ApplyChangesOnly(
                        item.localEulerAngles, initialLocalEuler, _transform.localEulerAngles);
                }

                if (didScaleChange)
                {
                    item.localScale = ApplyChangesOnly(
                        item.localScale, initialLocalScale, _transform.localScale);
                }
                
            }
        }
    }

    private Vector3 ApplyChangesOnly(Vector3 toApply, Vector3 initial, Vector3 changed)
    {
        if (!Mathf.Approximately(initial.x, changed.x))
            toApply.x = _transform.localPosition.x;

        if (!Mathf.Approximately(initial.y, changed.y))
            toApply.y = _transform.localPosition.y;

        if (!Mathf.Approximately(initial.z, changed.z))
            toApply.z = _transform.localPosition.z;

        return toApply;
    }
    


    private static bool quaternionFoldout = false;
    private void QuaternionInspector()
    {
        
        //Additional element to also view the Quaternion rotation values
        quaternionFoldout = EditorGUILayout.Foldout(quaternionFoldout, "Quaternion Rotation:    " + _transform.localRotation.ToString("F3"));
        if (quaternionFoldout)
        {
            Vector4 q = QuaternionToVector4(_transform.localRotation);
            EditorGUI.BeginChangeCheck();
            //EditorGUILayout.Vector3Field("Be careful!", Vector3.one);
            GUILayout.Label("Be careful!");
            EditorGUILayout.BeginHorizontal(layoutMaxWidth);
            GUILayout.Label("X");
            q.x = EditorGUILayout.FloatField(q.x);
            GUILayout.Label("Y");
            q.y = EditorGUILayout.FloatField(q.y);
            GUILayout.Label("Z");
            q.z = EditorGUILayout.FloatField(q.z);
            GUILayout.Label("W");
            q.w = EditorGUILayout.FloatField(q.w);
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_transform, "modify quaternion rotation on " + _transform.name);
                _transform.localRotation = ConvertToQuaternion(q);
            }
        }
    }


    private Quaternion ConvertToQuaternion(Vector4 v4)
    {
        return new Quaternion(v4.x, v4.y, v4.z, v4.w);
    }


    private Vector4 QuaternionToVector4(Quaternion q)
    {
        return new Vector4(q.x, q.y, q.z, q.w);
    }


    private bool showLocalAxisToggle = false;
    private void ShowLocalAxisComponentToggle()
    {
        GUILayout.Space(10);
        ShowLocalAxis showLocalAxis = _transform.gameObject.GetComponent<ShowLocalAxis>();

        
        if (showLocalAxis == null)
            showLocalAxisToggle = false;
        else
            showLocalAxisToggle = true;
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show local rotation handles", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        showLocalAxisToggle = GUILayout.Toggle(showLocalAxisToggle, (showLocalAxisToggle ? "on" : "off" ));
        GUILayout.FlexibleSpace();
        if (EditorGUI.EndChangeCheck())
        {
            if (showLocalAxisToggle == true)
            {
                showLocalAxis = _transform.gameObject.AddComponent<ShowLocalAxis>();
                int componentCount = _transform.GetComponents<Component>().Length;
                for (int i = 1; i < componentCount; i++)
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(showLocalAxis);
                }
            }
            else
            {
                showLocalAxis.destroyWhenSafe = true;
            }
        }
        GUILayout.EndHorizontal();
    }


    private AnimBool m_showExtraFields;
    private static bool _showExtraFields;

    void OnEnable()
    {
        m_showExtraFields = new AnimBool(_showExtraFields);
        m_showExtraFields.valueChanged.AddListener(Repaint);
    }

    private void SpecialOperations()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        m_showExtraFields.target = EditorGUILayout.ToggleLeft("Special operations", m_showExtraFields.target);
        if (EditorGUILayout.BeginFadeGroup(m_showExtraFields.faded))
        {
            AlignmentInspector();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            RandomRotatationInspector();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            RandomScaleInspector();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            RandomPositionInspector();
        }
        _showExtraFields = m_showExtraFields.value;
        EditorGUILayout.EndFadeGroup();
    }

    
    private GUILayoutOption[] buttonOptions = new GUILayoutOption[1] { GUILayout.Width(200f) };
    private bool Button(string label)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool value = GUILayout.Button(label, buttonOptions);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        return value;
    }

    public enum AlignToType { lastSelected, firstSelected }
    public enum AxisFlag { X = 1, Y = 2, Z = 4 }

    public AlignToType alignTo = AlignToType.lastSelected;
    public AxisFlag alignmentAxis = AxisFlag.X;
    private void AlignmentInspector()
    {
        EditorGUILayout.LabelField("Alignment", EditorStyles.boldLabel, layoutMaxWidth);
        alignTo = (AlignToType)EditorGUILayout.EnumPopup("Align to", alignTo, layoutMaxWidth);
        alignmentAxis = (AxisFlag)EditorGUILayout.EnumMaskField("Axis", alignmentAxis, layoutMaxWidth);

        string buttonLabel = "Select another object to align to";
        bool enableButton = false;
        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length > 1)
        {
            if (alignTo == AlignToType.lastSelected)
            {
                buttonLabel = "Align to " + selectedTransforms[selectedTransforms.Length - 1].name;
            }
            else
            {
                buttonLabel = "Align to " + selectedTransforms[0].name;
            }
            enableButton = true;
        }

        GUI.enabled = enableButton;
        if (Button(buttonLabel))
        {
            AlignTo(alignTo, alignmentAxis);
        }
        GUI.enabled = true;
    }


    private void AlignTo(AlignToType to , AxisFlag axis)
    {
        Transform[] selectedTransforms = Selection.transforms;

        int targetIndex = 0;
        if (to == AlignToType.lastSelected)
            targetIndex = selectedTransforms.Length - 1;

        for (int i = 0; i < selectedTransforms.Length; i++)
        {
            if (i == targetIndex)
                continue;

            Vector3 temp = selectedTransforms[i].position;

            if ((axis & AxisFlag.X) == AxisFlag.X)
                temp.x = selectedTransforms[targetIndex].position.x;

            if ((axis & AxisFlag.Y) == AxisFlag.Y)
                temp.y = selectedTransforms[targetIndex].position.y;

            if ((axis & AxisFlag.Z) == AxisFlag.Z)
                temp.z = selectedTransforms[targetIndex].position.z;

            Undo.RecordObject(selectedTransforms[i], selectedTransforms[i].name +  " aligned to " + selectedTransforms[targetIndex].name);
            selectedTransforms[i].position = temp;
        }
    }



    public AxisFlag rotationAxisFlag;
    private void RandomRotatationInspector()
    {
        EditorGUILayout.LabelField("Random Rotation", EditorStyles.boldLabel, layoutMaxWidth);
        rotationAxisFlag = (AxisFlag)EditorGUILayout.EnumMaskField("Rotation Axis", rotationAxisFlag, layoutMaxWidth);

        Transform[] selectedTransforms = Selection.transforms;

        string label = "Rotate " + _transform.name;
        if (selectedTransforms.Length > 1)
            label = "Rotate selected";

        if (Button(label))
        {
            RandomRotate(rotationAxisFlag , selectedTransforms);
        }
    }

    private void RandomRotate(AxisFlag axis , Transform[] selected)
    {
        for (int i = 0; i < selected.Length; i++)
        {
            Vector3 temp = selected[i].localEulerAngles;

            if ((axis & AxisFlag.X) == AxisFlag.X)
                temp.x = RdmDeg();

            if ((axis & AxisFlag.Y) == AxisFlag.Y)
                temp.y = RdmDeg();

            if ((axis & AxisFlag.Z) == AxisFlag.Z)
                temp.z = RdmDeg();

            Undo.RecordObject(_transform, "random rotate " + selected[i].name);
            selected[i].localEulerAngles = temp;
        }
    }

    private float RdmDeg()
    {
        return Random.Range(0f, 360f);
    }


    private AxisFlag scaleAxisFlag;
    private float minScale, maxScale;
    private bool scaleSame = true;
    private void RandomScaleInspector()
    {
        EditorGUILayout.LabelField("Random Scale (local)", EditorStyles.boldLabel, layoutMaxWidth);

        scaleAxisFlag = (AxisFlag)EditorGUILayout.EnumMaskField("Scale Axis", scaleAxisFlag, layoutMaxWidth);
        scaleSame = EditorGUILayout.ToggleLeft("Scale same", scaleSame, layoutMaxWidth);

        minScale = EditorGUILayout.FloatField("Min:", minScale, layoutMaxWidth);
        maxScale = EditorGUILayout.FloatField("Max", maxScale, layoutMaxWidth);

        Transform[] selectedTransforms = Selection.transforms;
        string btnLabel = "Scale " + _transform.name;
        if (selectedTransforms.Length > 1)
            btnLabel = "Scale selection";

        if (Button(btnLabel))
        {
            RandomScale(scaleAxisFlag, selectedTransforms, scaleSame);
        }
    }

    private void RandomScale(AxisFlag axis, Transform[] selected , bool scaleSame)
    {
        for (int i = 0; i < selected.Length; i++)
        {
            Vector3 temp = selected[i].localScale;
            Vector3 random = Vector3.zero;
            if (scaleSame)
            {
                float rdm = Random.Range(minScale, maxScale);
                random.x = rdm;
                random.y = rdm;
                random.z = rdm;
            }
            else
            {
                random.x = Random.Range(minScale, maxScale);
                random.y = Random.Range(minScale, maxScale);
                random.z = Random.Range(minScale, maxScale);
            }

            if ((axis & AxisFlag.X) == AxisFlag.X)
                temp.x = random.x;

            if ((axis & AxisFlag.Y) == AxisFlag.Y)
                temp.y = random.y;

            if ((axis & AxisFlag.Z) == AxisFlag.Z)
                temp.z = random.z;

            Undo.RecordObject(_transform, "random scale " + selected[i].name);
            selected[i].localScale = temp;
        }
    }


    private Vector3 minPosition, maxPosition;
    private void RandomPositionInspector()
    {
        EditorGUILayout.LabelField("Random Position", EditorStyles.boldLabel, layoutMaxWidth);
        minPosition = EditorGUILayout.Vector3Field("Min", minPosition, layoutMaxWidth);
        maxPosition = EditorGUILayout.Vector3Field("Max", maxPosition, layoutMaxWidth);

        Transform[] selectedTransforms = Selection.transforms;
        string btnLabel = "Move " + _transform.name;
        if (selectedTransforms.Length > 1)
            btnLabel = "Move selection";

        if (Button(btnLabel))
        {
            RandomPosition(minPosition, maxPosition, selectedTransforms);
        }
    }

    

    private void RandomPosition(Vector3 min , Vector3 max, Transform[] t)
    {
        for (int i = 0; i < t.Length; i++)
        {
            Vector3 temp = t[i].position;
            if (!Mathf.Approximately(min.x, max.x))
                temp.x = Random.Range(min.x, max.x);

            if (!Mathf.Approximately(min.y, max.y))
                temp.y = Random.Range(min.y, max.y);

            if (!Mathf.Approximately(min.z, max.z))
                temp.z = Random.Range(min.z, max.z);

            Undo.RecordObject(t[i], "Random position " + t[i].name);
            t[i].position = temp;
        }
    }
}
