using UnityEngine;
using System.Collections;
using NG;
using NG.Easing;

public class MultiLerp_Test : MonoBehaviour
{
    public Vector3[] waypoints;
    [Range(0,1)]
    public float ratio = 0f;

    
    void Start()
    {
        /*
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
        */

        waypoints = Vector3Ext.MakeAngledPath(Vector3.zero, PathAnchor.TopCenter, new Vector3(10,1,0), PathAnchor.LeftMiddle);


    }

    Vector3 start = new Vector3(-8, 0, 0);
    Vector3 end = new Vector3(8, 0, 0);
    float timer = 0f;
    //bool runUpdate = true;
    void Update()
    {
        /*
        if (!runUpdate)
            return;

        if (waypoints != null && waypoints.Length > 1)
        {
            Vector3 position = Vector3Ext.MultiLerp(waypoints, ratio);
            gameObject.transform.position = position;

            Debug.DrawLine(waypoints[0], waypoints[1], Color.red, 1f);
            Debug.DrawLine(waypoints[1], waypoints[2], Color.red, 1f);
        }
        */

        //gameObject.transform.position = Vector3.Lerp(start, end, Easing.EaseInOut((timer / 10f), EasingType.Sine, EasingType.Sine));//  Sigmoid(50f, timer / 10f));
        //gameObject.transform.position = Vector3.Lerp(start, end, Easing.SigmoidEaseIn(10f, (timer / 10f)));
        if (timer < 10f)
            gameObject.transform.position = Vector3.Lerp(start, end, Easing.Elastic.In(timer / 10f));
        timer += Time.deltaTime;
        
    }
}
