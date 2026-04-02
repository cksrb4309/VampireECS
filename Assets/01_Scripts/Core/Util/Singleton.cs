using System;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance = null;

    static Singleton()
    {
        SingletonRuntimeRegistry.Register(ResetInstance);
    }

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }

            return instance;
        }
    }

    private static void SetupInstance()
    {
        instance = FindFirstObjectByType<T>();

        if (instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;

            T type = gameObj.AddComponent<T>();

            instance = type;

            DontDestroyOnLoad(gameObj);
        }
        else
        {
            if (instance.transform.parent != null)
            {
                instance.transform.SetParent(null);
            }

            DontDestroyOnLoad(instance);
        }
    }

    private static void ResetInstance()
    {
        instance = null;
    }

    public virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;

            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}

internal static class SingletonRuntimeRegistry
{
    private static readonly List<Action> resetActions = new();

    internal static void Register(Action resetAction)
    {
        if (resetAction == null)
        {
            return;
        }

        resetActions.Add(resetAction);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        for (int i = 0; i < resetActions.Count; i++)
        {
            resetActions[i]?.Invoke();
        }
    }
}
