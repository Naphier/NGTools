using UnityEngine;
using NG.HttpClient;


/// <summary>
/// This class and gameObject it is attached to
/// is automatically instantiated by HttpRequest.
/// It is not designed to be used on its own.
/// </summary>
public class WWWController : MonoBehaviour
{
    private IHttpRequest request = null;
    private bool onFinishedSet = false;


    public void SetRequest(IHttpRequest request)
    {
        this.request = request;
        Execute();
    }

    public void Execute()
    {
        if (request == null)
        {
            Debug.LogError("HttpRequest was null. Not allowed.");
            DestroyOnCompletion(null);
        }
        else
        {
            if (!onFinishedSet)
            {
                onFinishedSet = true;
                request.onFinished += DestroyOnCompletion;
            }
            StartCoroutine(request.ExecuteCoroutine());
        }
    }


    private void DestroyOnCompletion(HttpResult result)
    {
        if (!request.CanBeDestroyed())
            return;

        if (gameObject.GetComponents<Component>().Length <= 2)
            Destroy(gameObject);
        else
            Destroy(this);
    }

}
