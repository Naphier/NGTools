using UnityEngine;

public class ShowLocalAxis : MonoBehaviour
{
    [HideInInspector]
    public bool destroyWhenSafe = false;

    [Range(1f,100f)]
    public float handleLength = 10f;

    public void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, Vector3.forward * handleLength);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, Vector3.right * handleLength);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Vector3.zero, Vector3.up* handleLength);
    }
}