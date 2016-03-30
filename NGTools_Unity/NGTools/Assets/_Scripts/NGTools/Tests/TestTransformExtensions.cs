using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class TestTransformExtensions : MonoBehaviour
{
    void Start()
    {
        #region Test Vector3 methods
        Vector3 toCompare = Vector3.zero;

        toCompare = toCompare.SetValues(1, 1, 1);
        Assert.AreEqual(Vector3.one, toCompare);
        Assert.AreNotEqual(Vector3.zero, toCompare);

        toCompare = toCompare.SetValues(x: 0, y: 0);
        Assert.AreEqual(Vector3.forward, toCompare);

        toCompare = toCompare.AddValues(1, 1);
        Assert.AreEqual(Vector3.one, toCompare);

        toCompare = toCompare.AddValues(-1, -1, -1);
        Assert.AreEqual(Vector3.zero, toCompare);
        Assert.AreNotEqual(Vector3.one, toCompare);

        toCompare = toCompare.AddValues(y: 1);
        Assert.AreEqual(Vector3.up, toCompare);
        #endregion


        #region Test Transform methods
        gameObject.transform.eulerAngles = Vector3.zero;
        gameObject.transform.SetEulerAngles(1, 1, 1);
        if (Vector3.one != gameObject.transform.eulerAngles)
            Debug.LogErrorFormat("Assertion failed. {0} != {1}", Vector3.one, gameObject.transform.eulerAngles);

        gameObject.transform.eulerAngles = Vector3.zero;
        gameObject.transform.SetEulerAngles(z: 1);
        if (Vector3.forward != gameObject.transform.eulerAngles)
            Debug.LogErrorFormat("Assertion failed. {0} != {1}", Vector3.forward, gameObject.transform.eulerAngles);

        gameObject.transform.AddEulerAngles(z: -1);
        Vector3Assert(Vector3.zero, gameObject.transform.eulerAngles);


        gameObject.transform.eulerAngles = Vector3.zero;
        gameObject.transform.SetLocalEulerAngles(x: 1);
        Vector3Assert(Vector3.right, gameObject.transform.localEulerAngles);

        gameObject.transform.AddToLocalEulerAngles(y: 1, z: 1);
        Vector3Assert(Vector3.one, gameObject.transform.localEulerAngles);
        gameObject.transform.localEulerAngles = Vector3.zero;


        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.SetLocalPosition(1, 1, 1);
        Vector3Assert(Vector3.one, gameObject.transform.localPosition);

        gameObject.transform.AddToLocalPosition(-1, -1, -1);
        Vector3Assert(Vector3.zero, gameObject.transform.localPosition);


        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.SetLocalScale(1, 1, 1);
        Vector3Assert(Vector3.one, gameObject.transform.localScale);

        gameObject.transform.AddToLocalScale(-1, -1, -1);
        Vector3Assert(Vector3.zero, gameObject.transform.localScale);


        gameObject.transform.position = Vector3.zero;
        gameObject.transform.SetPosition(1, 1, 1);
        Vector3Assert(Vector3.one, gameObject.transform.position);

        gameObject.transform.AddToPosition(-1, -1, -1);
        Vector3Assert(Vector3.zero, gameObject.transform.position);
        
        #endregion
    }

    void Vector3Assert(Vector3 expected, Vector3 actual)
    {
        if (expected != actual)
            Debug.LogErrorFormat("Assertion failed. Expected: {0} != Actual: {1}", expected, actual);
    }

    void Update()
    {

    }
}
