using UnityEngine;

public class NonMonoSubscriber
{
    private float lastTimeCalled = float.NaN;
    public void HeyYo()
    {
        if (lastTimeCalled != float.NaN && Time.time - lastTimeCalled < 5f)
            return;

        lastTimeCalled = Time.time;
        Debug.Log("HeyYo at " + lastTimeCalled);
    }
}

