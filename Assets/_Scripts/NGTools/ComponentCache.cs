using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Component reference caching system.
/// Instantiate a new ComponentCache object in your MonoBehaviour class with
/// the class's gameObject as the parameter.
/// The first call to ComponentCache.GetComponent will use the normal
/// gameObject.GetComponent method and will add the component to the cache list.
/// Subsequent calls will just grab the component from the cache list.
/// To ensure a component is removed from the cache use DestroyComponent.
/// </summary>
public class ComponentCache
{ 
    public enum LogMessageLevel { all, error, none}
    private LogMessageLevel logMessageLevel = LogMessageLevel.error;
    private GameObject _gameObject;
    private List<Component> _components = new List<Component>();


    /// <summary>
    /// Create a new ComponentCache
    /// </summary>
    /// <param name="gameObject">The gameObject which the cache applies to.</param>
    public ComponentCache(GameObject gameObject)
    {
        _gameObject = gameObject;
    }


    /// <summary>
    /// Create a new ComponentCache
    /// </summary>
    /// <param name="gameObject">The gameObject which the cache applies to.</param>
    /// <param name="messageLevel">Use this to control what messages are shown by this class.</param>
    public ComponentCache(GameObject gameObject , LogMessageLevel messageLevel)
    {
        _gameObject = gameObject;
        logMessageLevel = messageLevel;
    }


    /// <summary>
    /// Gets the specified component from the cache or adds it to the 
    /// cache if found on the gameObject.
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    /// <returns>The component found in the cache.</returns>
    public T GetComponent<T>() where T : Component
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].GetType() == typeof(T))
            {
                Log(typeof(T).Name + " found in cache.");
                return _components[i] as T;
            }
        }

        Component c = _gameObject.GetComponent<T>();
        if (c != null)
        {
            Log(typeof(T).Name + " added to cache.");
            _components.Add(c);
            return c as T;
        }

        LogError(typeof(T).Name + " not found.");
        return null;
    }


    /// <summary>
    /// Destroys the components of type T on the gameObject and 
    /// removes them from the cache. Returns true on success, false on failure.
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    /// <returns>True on success, false on failure.</returns>
    public bool DestroyComponent<T>() where T : Component
    {
        bool destroyed = false;
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].GetType() == typeof(T))
            {
                Log(typeof(T).Name + " removed from cache and destroyed.");
                Object.DestroyImmediate(_components[i]);
                _components.RemoveAt(i);
                destroyed = true;
            }
        }

        if (!destroyed)
            LogError(typeof(T).Name + " not found in cache.");

        return destroyed;
    }


    /// <summary>
    /// Destroys the specific component in the cache if found and  
    /// removes it from the gameObject. Returns true if removed from cache, 
    /// false if it was not found. If not found it will still be removed from the
    /// game object.
    /// </summary>
    /// <param name="component">Component to remove.</param>
    /// <returns>True if removed from cache, false if it was not found.</returns>
    public bool DestroyComponent(Component component)
    {
        bool destroyed = false;
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] == component)
            {
                Log(component.GetType().Name + " removed from cache and destroyed.");
                Object.DestroyImmediate(_components[i]);
                _components.RemoveAt(i);
                destroyed = true;
            }
        }

        if (!destroyed)
        {
            LogError(component.GetType().Name + " not found in cache. Destroying if it exists on the gameObject.");
            Object.DestroyImmediate(component);
        }

        return destroyed;
    }

    #region Class-specific message logging
    void Log(string message)
    {
        if (logMessageLevel == LogMessageLevel.all)
            Debug.Log(message);
    }

    void LogError(string message)
    {
        if (logMessageLevel != LogMessageLevel.none)
            Debug.LogError(message);
    }
    #endregion
}
