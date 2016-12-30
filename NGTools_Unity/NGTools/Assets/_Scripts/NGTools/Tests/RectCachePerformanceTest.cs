using UnityEngine;
using System.Collections;
using System.Diagnostics;
public class RectCachePerformanceTest : MonoBehaviour
{
    Stopwatch newRectTimer = new Stopwatch();
    Stopwatch rectCacheTimer = new Stopwatch();
    int iterations = 1000000;
    void Start()
    {
        for (int j = 0; j < 10; j++)
        {


            newRectTimer.Start();
            for (int i = 0; i < iterations; i++)
            {
                new Rect(i, 0, Screen.width, Screen.height);
            }
            newRectTimer.Stop();

            rectCacheTimer.Start();
            for (int i = 0; i < iterations; i++)
            {
                RectExtensions.FullScreen(i);
            }
            rectCacheTimer.Stop();

            UnityEngine.Debug.LogFormat(
                "Rect Cache Performance Test\n" +
                "iterations: {0}\n" +
                "averages:\n" +
                "\tnew: {1:N10}ms\n" +
                "\tcache: {2:N10}ms\n" +
                "totals:\n" +
                "\tnew: {3}ms\n" +
                "\tcache: {4}ms\n",
                iterations,
                newRectTimer.ElapsedMilliseconds / iterations,
                rectCacheTimer.ElapsedMilliseconds / iterations,
                newRectTimer.ElapsedMilliseconds,
                rectCacheTimer.ElapsedMilliseconds
                );

            newRectTimer.Reset();
            rectCacheTimer.Reset();
        }
    }

    

    void Update()
    {

    }
}
