using UnityEngine;
using System.Collections;
using NG;

public class MultiLerp_Test : MonoBehaviour
{
    public Vector3[] waypoints;
    [Range(0,1)]
    public float ratio = 0f;

    /*
    void Start()
    {
        float totalDistance = waypoints.MultiDistance();
        Debug.LogFormat("totalDistance: {0}", totalDistance);

        for (float i = 0f; i < 1.1f; i += 0.1f)
        {
            float distanceTravelled = i * totalDistance;            
            int indexLow = Vector3Ext.GetVectorIndexFromDistanceTravelled(waypoints, distanceTravelled);
            int indexHigh = indexLow + 1;
            Debug.LogFormat("i: {0}    distanceTravelled: {1}" + 
                "    indexLow: {2}    indexHigh: {3}" , 
                i, distanceTravelled, 
                indexLow, indexHigh);
        }
    }
    */

    bool runUpdate = true;
    void Update()
    {
        if (!runUpdate)
            return;

        if (waypoints != null && waypoints.Length > 1)
        {
            Vector3 position = Vector3Ext.MultiLerp(waypoints, ratio);
            gameObject.transform.position = position;
        }
    }
}
