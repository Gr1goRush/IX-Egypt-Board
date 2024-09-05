using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Singleton<T>
{
    public static T Instance { get; private set; }

    public bool dontDestroyOnLoad = false;

    protected virtual void Awake()
    {
        if (!dontDestroyOnLoad)
        {
            Instance = (T)this;
            return;
        }

        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
        DontDestroyOnLoad(gameObject);
    }
}