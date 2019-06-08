using UnityEngine;
using System.Collections;
using NG.MonoBehaviourUpdate;
public class TestMonoBehaviourUpdateManager : MonoBehaviour
{
    void Start()
    {
        /*
        for (int i = 0; i < 10; i++)
        {
            MonoBehaviourUpdateManager.Instance.SubscribeNonUnique(DoMe, "DoMe");
        }
        */
    
        for (int i = 0; i < 10; i++)
        {
            MonoBehaviourUpdateManager.Instance.Subscribe(DoMe);
        }
        /*
        if (MonoBehaviourUpdateManager.Instance.UpdateObservers.Count != 1)
            Debug.LogError("Subscribe unique fails. Multiple update observers added. Count: " +
                MonoBehaviourUpdateManager.Instance.UpdateObservers.Count.ToString());

        for (int i = 0; i < 9; i++)
        {
            MonoBehaviourUpdateManager.Instance.SubscribeNonUnique(DoMe);
        }

        if (MonoBehaviourUpdateManager.Instance.UpdateObservers.Count != 10)
            Debug.LogError("Subscribe unique fails. Incorrect observer count. Expected 10, actual Count: " +
                MonoBehaviourUpdateManager.Instance.UpdateObservers.Count.ToString());
        */
        /*
        MonoBehaviourUpdateManager.Instance.Unsubscribe(DoMe, "DoMe");
        MonoBehaviourUpdateManager.Instance.Unsubscribe(DoMe, "DoMe");
        MonoBehaviourUpdateManager.Instance.Unsubscribe(DoMe, "DoMe");

        MonoBehaviourUpdateManager.Instance.UnsubscribeNonUnique(DoMe);

        for (int i = 0; i < 9; i++)
        {
            MonoBehaviourUpdateManager.Instance.SubscribeNonUnique(DoMe, "DoMe");
        }

        MonoBehaviourUpdateManager.Instance.UnsubscribeNonUniqueByName("DoMe");
        */

        /*
        NonMonoSubscriber nonMonoSubscriber = new NonMonoSubscriber();

        for (int i = 0; i < 50; i++)
        {
            MonoBehaviourUpdateManager.Instance.SubscribeNonUnique(nonMonoSubscriber.HeyYo);
        }

        for (int i = 0; i < 50; i++)
        {
            MonoBehaviourUpdateManager.Instance.SubscribeNonUnique(DoMe);
        }
        */

        new GameObject("MBUpdateMonoUnitTest", typeof(MBUpdateMonoUnitTest));
    }

    void DoMe()
    {

    }

    int hey = 0;
    void Update()
    {
        hey += 1;
    }
}
