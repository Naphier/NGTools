using UnityEngine;
using System.Collections;

public class TestLogMessageReceivedCallback : MonoBehaviour
{
    GameObject go;

    void Update()
    {
        go = gameObject;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NG.Debug.Log("yo!");
            NG.Debug.LogError("error test");
            NG.Assert.IsNotNull(go , null);
            NG.Assert.AreEqual(1, 2);
        }
    }
}
