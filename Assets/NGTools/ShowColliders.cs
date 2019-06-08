using UnityEngine;

public class ShowColliders : MonoBehaviour
{
    public enum collidertype
    { All, TriggersOnly, CollidersOnly, None}

    public collidertype show = collidertype.All;
    public Color colliderColor = Color.yellow;
    public Color triggerColor = Color.blue;

    void OnDrawGizmos()
    {
        if (show == collidertype.None)
            return;

        GameObject[] all = GameObject.FindObjectsOfType<GameObject>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].activeInHierarchy)
            {
                switch (show)
                {
                    case collidertype.All:
                        DrawCollider(all[i]);
                        DrawTrigger(all[i]);
                        break;
                    case collidertype.TriggersOnly:
                        DrawTrigger(all[i]);
                        break;
                    case collidertype.CollidersOnly:
                        DrawCollider(all[i]);
                        break;
                    case collidertype.None:
                        break;
                    default:
                        break;
                }
                
            }
        }
    }

    void DrawCollider(GameObject go)
    {
        Collider _collider = go.GetComponent<Collider>();

        if (_collider == null)
            return;

        if (_collider.isTrigger)
            return;

        Gizmos.color = colliderColor;
        Gizmos.DrawWireCube(_collider.bounds.center, _collider.bounds.size);
    }


    void DrawTrigger(GameObject go)
    {
        Collider _collider = go.GetComponent<Collider>();

        if (_collider == null)
            return;

        if (!_collider.isTrigger)
            return;

        Gizmos.color = triggerColor;
        Gizmos.DrawWireCube(_collider.bounds.center, _collider.bounds.size);
    }
}