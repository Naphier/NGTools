using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RbTester : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(RandomForce());
    }

    public float interval = 0.5f;
    public float force = 10f;
    IEnumerator RandomForce()
    {
        yield return new WaitForSeconds(interval);
        Vector3 vForce = Vector3.zero;
        vForce.x = (Random.value < 0.5f ? 1f : -1f) * force;
        vForce.y = (Random.value < 0.5f ? 1f : -1f) * force;

        rb.AddForce(vForce);

        StartCoroutine(RandomForce());
        yield break;
    }
}
