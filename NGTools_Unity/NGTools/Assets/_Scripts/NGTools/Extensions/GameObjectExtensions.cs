namespace UnityEngine
{
    public static class GameObjectExtensions
    {
        public static T GetOrCreateComponent<T>(this GameObject gameObject) where T : Component
        {
            Component component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component as T;
        }

    }
}
