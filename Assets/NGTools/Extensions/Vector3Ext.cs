using System;
using UnityEngine;

namespace NG
{
    public enum PathAnchor { TopLeft, TopCenter, TopRight, RightUpper, RightMiddle, RightLower,
    BottomRight, BottomCenter, BottomLeft, LeftLower, LeftMiddle, LeftUpper}


    public static class Vector3Ext
    {
        #region Multilerp
        // This is not very optimized. There are at least n + 1 and at most 2n Vector3.Distance
        // calls (where n is the number of waypoints). 
        public static Vector3 MultiLerp(Vector3[] waypoints, float ratio)
        {
            Vector3 position = Vector3.zero;
            float totalDistance = waypoints.MultiDistance();
            float distanceTravelled = totalDistance * ratio;

            int indexLow = GetVectorIndexFromDistanceTravelled(waypoints, distanceTravelled);
            int indexHigh = indexLow + 1;

            // we're done
            if (indexHigh > waypoints.Length - 1)
                return waypoints[waypoints.Length - 1];

            
            // calculate the distance along this waypoint to the next
            Vector3[] completedWaypoints = new Vector3[indexLow + 1];

            for (int i = 0; i < indexLow + 1; i++)
            {
                completedWaypoints[i] = waypoints[i];
            }

            float distanceCoveredByPreviousWaypoints = completedWaypoints.MultiDistance();
            float distanceTravelledThisSegment = distanceTravelled - distanceCoveredByPreviousWaypoints;
            float distanceThisSegment = Vector3.Distance(waypoints[indexLow], waypoints[indexHigh]);

            float currentRatio = distanceTravelledThisSegment / distanceThisSegment;
            position = Vector3.Lerp(waypoints[indexLow], waypoints[indexHigh], currentRatio);

            return position;
        }

        public static float MultiDistance(this Vector3[] waypoints)
        {
            float distance = 0f;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (i + 1 > waypoints.Length - 1)
                    break;

                distance += Vector3.Distance(waypoints[i], waypoints[i + 1]);
            }

            return distance;
        }

        public static int GetVectorIndexFromDistanceTravelled(Vector3[] waypoints, float distanceTravelled)
        {
            float distance = 0f;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (i + 1 > waypoints.Length - 1)
                    return waypoints.Length - 1;

                float segmentDistance = Vector3.Distance(waypoints[i], waypoints[i + 1]);

                if (segmentDistance + distance > distanceTravelled)
                {
                    return i;
                }

                distance += segmentDistance;
            }

            return waypoints.Length - 1;
        }
        #endregion


        public static Vector3[] MakeAngledPath(
            Vector3 start, PathAnchor startAnchor, Vector3 end, PathAnchor endAnchor)
        {
            Vector3[] path = new Vector3[3];
            path[0] = start;
            path[1] = start;
            path[2] = end;

            int pathType = GetPathType(start, startAnchor, end, endAnchor);

            switch (pathType)
            {
                case 0:
                default:
                    Debug.LogErrorFormat(
                        "Invalid PathAnchor pairing - startAnchor: {0}  endAnchor: {1}   start: {2}   end: {3}", 
                        startAnchor, endAnchor, start, end);
                    return null;
                case 1:
                    path[1] = new Vector3(
                        start.x,
                        end.y,
                        (start.z + 0.5f * (end.z - start.z)));
                    break;
                case 2:
                    path[1] = new Vector3(
                        end.x,
                        start.y,
                        (start.z + 0.5f * (end.z - start.z)));
                    break;
            }

            return path;
        }


        public static int GetPathType(Vector3 start, PathAnchor startAnchor, Vector3 end, PathAnchor endAnchor)
        {
            int pathType = 0;

            switch (startAnchor)
            {
                case PathAnchor.TopLeft:
                case PathAnchor.TopCenter:
                case PathAnchor.TopRight:
                    switch (endAnchor)
                    {
                        case PathAnchor.TopLeft:
                        case PathAnchor.TopCenter:
                        case PathAnchor.TopRight:
                            break;
                        case PathAnchor.RightUpper:
                        case PathAnchor.RightMiddle:
                        case PathAnchor.RightLower:
                            if (end.x > start.x)
                                break;
                            else
                                pathType = 1;
                            break;
                        case PathAnchor.BottomRight:
                        case PathAnchor.BottomCenter:
                        case PathAnchor.BottomLeft:
                            if (!Mathf.Approximately(end.x, start.x))
                                break;
                            else
                                pathType = 1;
                            break;
                        case PathAnchor.LeftLower:
                        case PathAnchor.LeftMiddle:
                        case PathAnchor.LeftUpper:
                            if (end.x < start.x)
                                break;
                            else
                                pathType = 1;
                            break;
                    }
                    break;

                case PathAnchor.RightUpper:
                case PathAnchor.RightMiddle:
                case PathAnchor.RightLower:
                    switch (endAnchor)
                    {
                        case PathAnchor.TopLeft:
                        case PathAnchor.TopCenter:
                        case PathAnchor.TopRight:
                            if (end.y > start.y)
                                break;
                            else
                                pathType = 2;
                            break;
                        case PathAnchor.RightUpper:
                        case PathAnchor.RightMiddle:
                        case PathAnchor.RightLower:
                            break;
                        case PathAnchor.BottomRight:
                        case PathAnchor.BottomCenter:
                        case PathAnchor.BottomLeft:
                            if (end.y < start.y)
                                break;
                            else
                                pathType = 2;

                            break;
                        case PathAnchor.LeftLower:
                        case PathAnchor.LeftMiddle:
                        case PathAnchor.LeftUpper:
                            if (!Mathf.Approximately(start.y, end.y))
                                break;
                            else
                                pathType = 2;
                            break;
                    }
                    break;

                case PathAnchor.BottomRight:
                case PathAnchor.BottomCenter:
                case PathAnchor.BottomLeft:
                    switch (endAnchor)
                    {
                        case PathAnchor.TopLeft:
                        case PathAnchor.TopCenter:
                        case PathAnchor.TopRight:
                            if (!Mathf.Approximately(start.y, end.y))
                                break;
                            else
                                pathType = 1;
                            break;
                        case PathAnchor.RightUpper:
                        case PathAnchor.RightMiddle:
                        case PathAnchor.RightLower:
                            if (end.x > start.x)
                                break;
                            else
                                pathType = 1;
                            break;
                        case PathAnchor.BottomRight:
                        case PathAnchor.BottomCenter:
                        case PathAnchor.BottomLeft:
                            break;
                        case PathAnchor.LeftLower:
                        case PathAnchor.LeftMiddle:
                        case PathAnchor.LeftUpper:
                            if (end.x < start.x)
                                break;
                            else
                                pathType = 1;
                            break;
                    }
                    break;

                case PathAnchor.LeftLower:
                case PathAnchor.LeftMiddle:
                case PathAnchor.LeftUpper:
                    switch (endAnchor)
                    {
                        case PathAnchor.TopLeft:
                        case PathAnchor.TopCenter:
                        case PathAnchor.TopRight:
                            if (end.x > start.x)
                                break;
                            else
                                pathType = 2;
                            break;
                        case PathAnchor.RightUpper:
                        case PathAnchor.RightMiddle:
                        case PathAnchor.RightLower:
                            if (!Mathf.Approximately(start.y, end.y))
                                break;
                            else
                                pathType = 2;
                            break;
                        case PathAnchor.BottomRight:
                        case PathAnchor.BottomCenter:
                        case PathAnchor.BottomLeft:
                            if (end.y < start.y)
                                break;
                            else
                                pathType = 4;
                            break;
                        case PathAnchor.LeftLower:
                        case PathAnchor.LeftMiddle:
                        case PathAnchor.LeftUpper:
                            break;

                    }
                    break;
            }

            return pathType;
        }


        public static PathAnchor[] GetPathAnchors(Vector3 start, Vector3 end)
        {

            if (Mathf.Approximately(start.x, end.x))
            {
                if (start.y > end.y)
                    return new PathAnchor[] { PathAnchor.BottomCenter, PathAnchor.TopCenter };
                else
                    return new PathAnchor[] { PathAnchor.TopCenter, PathAnchor.BottomCenter };
            }
            else if (Mathf.Approximately(start.y, end.y))
            {
                if (start.x > end.x)
                    return new PathAnchor[] { PathAnchor.LeftMiddle, PathAnchor.RightMiddle };
                else
                    return new PathAnchor[] { PathAnchor.RightMiddle, PathAnchor.LeftMiddle };
            }
            else if (start.x < end.x)
            {
                if (start.y < end.y)
                    return new PathAnchor[] { PathAnchor.RightUpper, PathAnchor.BottomLeft };
                else
                    return new PathAnchor[] { PathAnchor.RightLower, PathAnchor.TopLeft };
            }
            else if (start.x > end.x)
            {
                if (start.y < end.y)
                    return new PathAnchor[] { PathAnchor.LeftUpper, PathAnchor.BottomRight };
                else
                    return new PathAnchor[] { PathAnchor.LeftLower, PathAnchor.TopRight };
            }


            Debug.LogErrorFormat("Could not determine PathAnchors for start: {0}  end: {1}", start, end);
            return null;
        }
    }

}
