using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SimpleMovement : MonoBehaviour
{
    int loadedLevel = 0;
    void Start()
    {
        loadedLevel = 0;
        DontDestroyOnLoad(gameObject);
    }

    public float speed = 10f;
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Mathf.Abs(h) > 0.1f)
            gameObject.transform.position += h * Vector3.right * speed * Time.deltaTime;

        if (Mathf.Abs(v) > 0.1f)
            gameObject.transform.position += v * Vector3.up * speed * Time.deltaTime;

        HandleLevelLoading();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "level_trigger")
            StartLoadLevelSequence();
    }

    bool isLoading = false;
    List<Transform> destroyQueue;
    AsyncOperation loadLevel;
    void StartLoadLevelSequence()
    {
        if (isLoading)
        {
            Debug.Log("already loading");
            return;
        }

        if (loadedLevel == 0)
            loadedLevel = 1;
        else
            loadedLevel = 0;

        destroyQueue = GameObject.FindObjectsOfType<Transform>().ToList();

        isLoading = true;
        StartCoroutine(LoadNextLevelCo());
    }

    IEnumerator LoadNextLevelCo()
    {
        isLoading = true;
        loadLevel = Application.LoadLevelAdditiveAsync(loadedLevel);
        yield return loadLevel;
    }

    void HandleLevelLoading()
    {
        if (isLoading && loadLevel != null)
        {
            if (loadLevel.isDone)
            {
                Debug.Log("Level loading is done");
                isLoading = false;
                for (int i = 0; i < destroyQueue.Count; i++)
                {
                    if (destroyQueue[i].name != "Main Camera")
                    {
                        Debug.Log("destroying: " + destroyQueue[i].gameObject.name);
                        Destroy(destroyQueue[i].gameObject);
                    }
                }

                GameObject[] duplicateCam = GameObject.FindGameObjectsWithTag("MainCamera");
                for (int i = 0; i < duplicateCam.Length; i++)
                {
                    if (duplicateCam[i].GetComponent<SimpleMovement>() != this)
                        Destroy(duplicateCam[i]);
                }
            }
        }
    }


    void OnGUI()
    {
        string extra = "null";
        if (loadLevel != null)
            extra = (loadLevel.isDone ? "is done" : "not done");

        GUI.Label(new Rect(0, 0, 500, 500), "isLoading: " + isLoading.ToString() + "\n" + extra);
    }
}


public class CoroutineWithData
{
    public Coroutine coroutine { get; private set; }
    public object result;
    private IEnumerator target;
    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext())
        {
            result = target.Current;
            yield return result;
        }
    }
}