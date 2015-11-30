using UnityEngine;

public class TestSimpleCSVDebug : MonoBehaviour
{
    // Construct an instance of the SimpleCSVDebug class to use.
    private SimpleCSVDebug simpleCSVDebug = new SimpleCSVDebug();


    void Start()
    {
        // Write the heading to the file so we have named columns in the CSV.
        simpleCSVDebug.AddLineToOutput("Time,lerpTime,newPositionX,newPositionY");
    }

    private float lerpTime = 0f;
    private Vector2 start = Vector2.zero;
    private Vector2 end = new Vector2(10f,10f);
    private float lerpDuration = 2f;
    void Update()
    {
        Vector2 newPosition = Vector2.Lerp(start, end, lerpTime / lerpDuration);
        simpleCSVDebug.AddLineToOutput(simpleCSVDebug.DelimitedFormat(',', "F6", Time.time, lerpTime, newPosition.x, newPosition.y));

        lerpTime += Time.deltaTime;

        // We've successfully lerped so we can disable the script.
        // OnDisable will write the output to the file.
        if (newPosition == end)
        {
            Debug.Log("success!");
            enabled = false;
        }
    }

    // We're using enabled=false to signal success,
    // yet we may still want to see our results so we should
    // write to the file at that time.
    void OnDisable() { simpleCSVDebug.FinalWrite(); }

    // We may also want to stop the application and automatically write to file
    // if we don't ever meet the success condition to disable the script.
    void OnApplicationQuit() { simpleCSVDebug.FinalWrite(); }


    void OnGUI()
    {
        // Let's monitor our stringbuilder's size.
        GUI.Label(new Rect(0, 0, 500, 500), "ouput's bytes: " + simpleCSVDebug.OuputSizeInBytes().ToString("N"));
    }


}

