using UnityEngine;
using System.Collections;
using NG.MonoBehaviourUpdate;

public class MBUpdateMonoUnitTest : MonoBehaviour
{
    private NonMonoSubscriber subscriber = new NonMonoSubscriber();
    void Start()
    {
        MonoBehaviourUpdateManager.Instance.SubscribeNonUnique(subscriber.HeyYo);
        StartCoroutine(KillInFive());
    }

    IEnumerator KillInFive()
    {
        yield return new WaitForSeconds(5);
        MonoBehaviourUpdateManager.Instance.SetObserverActive(subscriber.HeyYo, false);

        yield return new WaitForSeconds(10);
        MonoBehaviourUpdateManager.Instance.SetObserverActive(subscriber.HeyYo, true);

        yield return new WaitForSeconds(15);
        Destroy(gameObject);
    }

    void Update()
    {

    }

    
    void OnDestroy()
    {
        MonoBehaviourUpdateManager.Instance.Unsubscribe(subscriber.HeyYo);
    }
    
}
