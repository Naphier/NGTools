using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NG.HttpClient;

public class TestHttpRequest : MonoBehaviour
{
    HttpRequest request;
    List<HttpRequest> testRequests = new List<HttpRequest>();
    int currentTestIndex = 0;
    int failingTestStartIndex = -1;
    float progress = 0;
    string response = "";
    GameObject textureHolder = null;
    

    void Start()
    {
        textureHolder = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Set up tests
        testRequests.Add(Http.Request("www.naplandgames.com", null));

        HttpRequest httpSecGet = Http.Request(HttpMethod.GET, "www.wordspionagedb.com", "stats/dbstats.php", "napland", "pIUWg8CNd9yO");
        testRequests.Add(httpSecGet);

        HttpRequest httpsSecGet = Http.Request(HttpMethod.GET, "www.wordspionagedb.com", "stats/dbstats.php", "napland", "pIUWg8CNd9yO");
        httpsSecGet.useHttps = true;
        testRequests.Add(httpsSecGet);

        HttpRequest httpSecPost = Http.Request(HttpMethod.POST, "www.wordspionagedb.com", "stats/dbstats.php", "napland", "pIUWg8CNd9yO");
        testRequests.Add(httpSecPost);

        HttpRequest httpsSecPost = Http.Request(HttpMethod.POST, "www.wordspionagedb.com", "stats/dbstats.php", "napland", "pIUWg8CNd9yO");
        httpsSecPost.useHttps = true;
        testRequests.Add(httpsSecPost);

        WWWForm form = new WWWForm();
        form.AddField("value", 100);
        HttpRequest postField = Http.Request("www.naplandgames.com", "testing/testpost.php", form);
        testRequests.Add(postField);

        HttpRequest imageGetRequest = Http.Request("www.naplandgames.com", "gravitone/large-icon-512_with-title.png");
        imageGetRequest.onSuccess += HandleImage;
        imageGetRequest.resultType = ResultData.texture;
        testRequests.Add(imageGetRequest);

        HttpRequest imageGetRequest2 = Http.Request("www.naplandgames.com", "images/tile_sis.png");
        imageGetRequest2.onSuccess += HandleImage;
        imageGetRequest2.resultType = ResultData.textureNonReadable;
        testRequests.Add(imageGetRequest2);

        HttpRequest audioGetRequest = Http.Request("www.naplandgames.com", "testing/luigi.wav");
        audioGetRequest.onSuccess += HandleAudio;
        audioGetRequest.resultType = ResultData.audioClip;
        testRequests.Add(audioGetRequest);

        //Failed tests begin
        testRequests.Add(Http.Request("baddomain", null));
        failingTestStartIndex = testRequests.Count - 1;
        NextTest(null);
    }

    void NextTest(HttpResult result)
    {
        if (currentTestIndex < testRequests.Count)
        {
            string message = "Passing test #: ";
            if (failingTestStartIndex > 0 && currentTestIndex >= failingTestStartIndex)
                message = "Failing test #: ";

            message += currentTestIndex.ToString();
            Debug.LogWarning(message);

            
            request = testRequests[currentTestIndex];
            request.onSuccess += OnSuccess;
            request.onError += OnFailure;
            request.timeOut = 2f;
            request.retryAttempts = 2;
            request.onFinished += NextTest;
            request.Execute();
            currentTestIndex++;
        }
        else
            Debug.LogWarning("TESTING COMPLETE");
    }


    void OnSuccess(HttpResult result)
    {
        response = result.response;
        Debug.Log("SUCCESS: \n" + result.ToString());
    }


    void HandleImage(HttpResult result)
    {
        Texture2D tex = result.texture2d;
        if (tex == null)
        {
            Debug.LogError("Something went wrong!");
            return;
        }

        Vector3 scale = new Vector3(result.texture2d.width / 100f, result.texture2d.height / 100, 1f);
        textureHolder.transform.localScale = scale;
        Material mat = textureHolder.GetComponent<Renderer>().material;
        mat.mainTexture = tex;
        mat.shader = Shader.Find("Unlit/Texture");
    }

    public AudioSource audioSource;
    void HandleAudio(HttpResult result)
    {
        AudioClip clip = result.audioClip;
        if (clip == null)
        {
            Debug.LogError("Something went wrong!");
            return;
        }

        if (audioSource == null)
        {
            throw new MissingReferenceException();
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

    

    void OnFailure(HttpResult result)
    {
        response = result.response;
        Debug.LogError("FAILURE: \n" + result.ToString());
    }


    void OnGUI()
    {
        if (request != null)
        {
            progress = request.GetProgress();
        }

        string label = string.Format("Test#: {0} -  Progress: {1}\nResponse: {2}", currentTestIndex, progress, response);
        GUI.Label(new Rect(0, 0, 1000, 1000), StringExtension.Truncate(label , 1000));
    }
}
