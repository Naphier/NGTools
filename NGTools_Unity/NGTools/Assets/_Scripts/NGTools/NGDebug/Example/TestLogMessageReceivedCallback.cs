using UnityEngine;
using NG.AssertDebug;

public class TestLogMessageReceivedCallback : MonoBehaviour
{
    GameObject go;

    void Update()
    {
        go = gameObject;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NGDebug.Log("yo!");
            NGDebug.LogError("error test");
            Assert.IsNotNull(go , null);
            Assert.AreEqual(1, 2);
        }
    }
}
