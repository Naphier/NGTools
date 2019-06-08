using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

public class TestSphereGeneration : MonoBehaviour
{

    Stopwatch stopwatch = new Stopwatch();

    List<Vector3> vertices = new List<Vector3>();
    void Start()
    {
        //vertices = Geometry.GetFibonacciSphereVertices(100, false);
        vertices = Geometry.GetSubdividedIcosahedronVertices(3);
        
        GameObject parent = new GameObject("parent");
        int vertexNum = 1;
        foreach (var item in vertices)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = vertexNum.ToString();
            vertexNum++;
            go.transform.position = item * 3f;
            //go.transform.localScale *= 0.1f;
            go.transform.SetParent(parent.transform);

        }



        StartCoroutine(TestSubdivision());
    }

    IEnumerator TestSubdivision()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 6; i++)
        {
            stopwatch.Reset();
            stopwatch.Start();
            vertices = Geometry.GetSubdividedIcosahedronVertices(i);

            string s = string.Format(
                "subdivisions:{0}  vertex count: {1}  calculation time: {2}",
                i, vertices.Count, stopwatch.ElapsedMilliseconds);
            Debug.Log(s);
            sb.AppendLine(s);
            yield return new WaitForEndOfFrame();
        }

        Debug.Log(sb);

    }

}
