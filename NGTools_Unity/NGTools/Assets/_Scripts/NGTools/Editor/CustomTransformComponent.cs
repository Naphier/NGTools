using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class MyTransformComponent : Editor
{
    private Transform _transform;
    private AnimBool m_showExtraFields;


    void OnEnable()
    {
        m_showExtraFields = new AnimBool(false);
        m_showExtraFields.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {
        //We need this for all OnInspectorGUI sub methods
        _transform = (Transform)target;

        StandardTransformInspector();

        QuaternionInspector();

        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        ShowLocalAxisComponentToggle();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        m_showExtraFields.target = EditorGUILayout.ToggleLeft("Special operations", m_showExtraFields.target);
        if (EditorGUILayout.BeginFadeGroup(m_showExtraFields.faded))
        {
            AlignmentButton();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            RandomRotateButton();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            RandomScaleButton();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            RandomPositionButton();
        }
        EditorGUILayout.EndFadeGroup();
    }


    private void StandardTransformInspector()
    {
        _transform.localPosition = EditorGUILayout.Vector3Field("Position", _transform.localPosition);
        _transform.localEulerAngles = EditorGUILayout.Vector3Field("Euler Rotation", _transform.localEulerAngles);
        _transform.localScale = EditorGUILayout.Vector3Field("Scale", _transform.localScale);
    }


    private bool quaternionFoldout = false;
    private void QuaternionInspector()
    {
        //Additional element to also view the Quaternion rotation values
        quaternionFoldout = EditorGUILayout.Foldout(quaternionFoldout, "Quaternion Rotation:    " + _transform.localRotation.ToString("F3"));
        if (quaternionFoldout)
        {
            EditorGUI.BeginChangeCheck();
            Vector4 qRotation = EditorGUILayout.Vector4Field("Be careful!", QuaternionToVector4(_transform.localRotation));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_transform, "modify quaternion rotation on " + _transform.name);
                _transform.localRotation = ConvertToQuaternion(qRotation);
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


    private bool showLoxalAxisToggle = false;
    private ShowLocalAxis showLocalAxis;
    private void ShowLocalAxisComponentToggle()
    {    
        showLocalAxis = _transform.gameObject.GetComponent<ShowLocalAxis>();

        
        if (showLocalAxis == null)
            showLoxalAxisToggle = false;
        else
            showLoxalAxisToggle = true;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show local rotation handles", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        showLoxalAxisToggle = EditorGUILayout.ToggleLeft((showLoxalAxisToggle ? "on" : "off" ), showLoxalAxisToggle);
        if (EditorGUI.EndChangeCheck())
        {
            if (showLoxalAxisToggle == true)
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
        EditorGUILayout.EndHorizontal();
    }



    public enum AlignToType { lastSelected, firstSelected }
    public AlignToType alignTo = AlignToType.lastSelected;

    
    public enum AxisFlag
    {
        X = 1,
        Y = 2,
        Z = 4
    }
    
    public AxisFlag alignAxis;

    private void AlignmentButton()
    {
        EditorGUILayout.LabelField("Alignment", EditorStyles.boldLabel);
        alignTo = (AlignToType)EditorGUILayout.EnumPopup("Align to", alignTo);
        alignAxis = (AxisFlag)EditorGUILayout.EnumMaskField("Axis", alignAxis);

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
        if (GUILayout.Button(buttonLabel))
        {
            AlignTo(alignTo, alignAxis);
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
                temp.y = selectedTransforms[targetIndex].position.z;

            Undo.RecordObject(selectedTransforms[i], selectedTransforms[i].name +  " aligned to " + selectedTransforms[targetIndex].name);
            selectedTransforms[i].position = temp;
        }
    }



    public AxisFlag axisFlag;
    private void RandomRotateButton()
    {
        EditorGUILayout.LabelField("Random Rotation", EditorStyles.boldLabel);
        axisFlag = (AxisFlag)EditorGUILayout.EnumMaskField("Rotation Axis", axisFlag);

        Transform[] selectedTransforms = Selection.transforms;

        string label = "Rotate " + _transform.name;
        if (selectedTransforms.Length > 1)
            label = "Rotate selected";

        if (GUILayout.Button(label))
        {
            RandomRotate(axisFlag , selectedTransforms);
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


    private AxisFlag scaleAxis;
    private float minScale, maxScale;
    private bool scaleSame = true;
    private void RandomScaleButton()
    {
        EditorGUILayout.LabelField("Random Scale (local)", EditorStyles.boldLabel);
        scaleAxis = (AxisFlag)EditorGUILayout.EnumMaskField("Scale Axis", scaleAxis);
        scaleSame = EditorGUILayout.ToggleLeft("Scale same", scaleSame);

        EditorGUILayout.BeginHorizontal();
        minScale = EditorGUILayout.FloatField("Min:", minScale);
        maxScale = EditorGUILayout.FloatField("Max", maxScale);
        EditorGUILayout.EndHorizontal();

        Transform[] selectedTransforms = Selection.transforms;
        string btnLabel = "Scale " + _transform.name;
        if (selectedTransforms.Length > 1)
            btnLabel = "Scale selection";

        if (GUILayout.Button(btnLabel))
        {
            RandomScale(scaleAxis, selectedTransforms, scaleSame);
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
    private void RandomPositionButton()
    {
        EditorGUILayout.LabelField("Random Position", EditorStyles.boldLabel);
        minPosition = EditorGUILayout.Vector3Field("Min", minPosition);
        maxPosition = EditorGUILayout.Vector3Field("Max", maxPosition);

        Transform[] selectedTransforms = Selection.transforms;
        string btnLabel = "Move " + _transform.name;
        if (selectedTransforms.Length > 1)
            btnLabel = "Move selection";

        if (GUILayout.Button(btnLabel))
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

    public void SetRepaint()
    {
        Repaint();
    }
}