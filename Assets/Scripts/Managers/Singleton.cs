using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;

    private static readonly object instanceLock = new object();

    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting) return null;

            lock(instanceLock)
            {
                if (!applicationIsQuitting )
                {
                    var all = FindObjectsOfType<T>();
                    instance = all != null && all.Length > 0 ? all[0] : null;

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = "Singleton: " + typeof(T).ToString();
                    }
                }
                return instance;
            }
        }
    }

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
