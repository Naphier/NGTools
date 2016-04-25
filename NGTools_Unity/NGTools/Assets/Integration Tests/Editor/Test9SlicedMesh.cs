using UnityEngine;
using UnityEditor;

public class Test9SlicedMesh : Editor
{
    [MenuItem("Edit/NGTools/Make 9-sliced mesh")]
    static void Make9SlicedMesh()
    {
        GameObject go = new GameObject("9sliced");
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();

        meshFilter.mesh = Geometry.Create9SlicedMesh();
    }
}