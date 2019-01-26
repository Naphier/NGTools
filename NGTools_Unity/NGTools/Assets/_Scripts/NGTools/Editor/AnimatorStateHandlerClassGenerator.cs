using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System;

namespace NGTools
{
    public class AnimatorStateHandlerClassGenerator : Editor
    {
        [MenuItem("Assets/Generate Animator State Handler")]
        static void GenerateStateHandler(MenuCommand menuCommand)
        {
            var selected = Selection.activeObject;
            if (!selected)
                return;

            AnimatorController animatorController = null; // menuCommand as AnimatorController;
            try
            {
                animatorController = (AnimatorController)selected;
            }
            catch (Exception)
            {
                Debug.LogError("Selected object is not animation controller");
            }

            if (!animatorController)
                return;

            //Debug.Log("path: " + AssetDatabase.GetAssetPath(selected));

            // generate C# file
            string className = animatorController.name + "_AnimatorStateHandler";

            string contents = "using UnityEngine;\r\n\r\n" +
                "public class " + className + " : MonoBehaviour\r\n{\r\n\t" +
                "#pragma warning disable 0649\r\n\t" +
                "[SerializeField]\r\n\tAnimator animator;\r\n\t" +
                "#pragma warning restore\r\n\r\n\t";

            foreach (var item in animatorController.parameters)
            {
                contents += "public void Set_" + item.name + "(";
                var call = "animator.Set";
                switch (item.type)
                {
                    case AnimatorControllerParameterType.Float:
                        contents += "float value";
                        call += "Float(\"" + item.name + "\", value);\r\n\t";
                        break;
                    case AnimatorControllerParameterType.Int:
                        contents += "int value";
                        call += "Integer(\"" + item.name + "\", value);\r\n\t";
                        break;
                    case AnimatorControllerParameterType.Bool:
                        contents += "bool value";
                        call += "Bool(\"" + item.name + "\", value);\r\n\t";
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        call += "Trigger(\"" + item.name + "\");\r\n\t";
                        break;
                    default:
                        throw new NotImplementedException(item.type.ToString());
                }

                contents += ")\r\n\t{\r\n\t\t";
                contents += call + "}\r\n\t";
            }

            contents += "\r\n}";
            var assetPath = AssetDatabase.GetAssetPath(selected).Replace("Assets/", "");
            var split = assetPath.Split('/');
            var selectedFileName = split[split.Length - 1];
            assetPath = assetPath.Replace(selectedFileName, "");
            string filePath = Path.Combine(Application.dataPath, assetPath, className + ".cs");


            File.WriteAllText(filePath, contents);

            Debug.Log("Animator State Handler written to: " + filePath);
            AssetDatabase.Refresh();
        }
    }
}
