using UnityEngine;
using UnityEngine.Assertions;

public class TestCacheComponent : MonoBehaviour
{
    public ComponentCache cc;
    AudioSource asrc1;
    void Start()
    {
        cc = new ComponentCache(gameObject , ComponentCache.LogMessageLevel.all);

        Debug.LogWarning("Test Cache");
        cc.GetComponent<AudioSource>();
        cc.GetComponent<AudioSource>();
        cc.DestroyComponent<AudioSource>();
        

        Debug.LogWarning("Add component test");
        asrc1 = gameObject.AddComponent<AudioSource>();
        asrc1.mute = true;
        AudioSource testAs = cc.GetComponent<AudioSource>();
        if (testAs != asrc1)
            Debug.LogError("Retrieved component not the same?");
        
        testAs = cc.GetComponent<AudioSource>();
        if (testAs != asrc1)
            Debug.LogError("Retrieved component not the same?");
        
        
        Debug.LogWarning("Adding second component");
        AudioSource asrc2 = gameObject.AddComponent<AudioSource>();
        Debug.LogWarning("Destroying first component");
        cc.DestroyComponent(asrc1);
        cc.GetComponent<AudioSource>();
        cc.GetComponent<AudioSource>();

        Assert.IsNull(asrc1);
        Assert.IsNotNull(asrc2);
        

    }
    bool doOnce = true;
    void Update()
    {
        if (doOnce && asrc1 == null)
        {
            doOnce = false;
            Debug.LogWarning("AudioSource 1 is now null");
        }
    }
}
