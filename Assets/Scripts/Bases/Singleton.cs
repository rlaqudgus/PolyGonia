using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T _instance;
    public static T Instance
    {
        get
        {
            if (!_instance) Debug.Log($"{typeof(T)} is Empty");
            return _instance;
        }
    }

    protected virtual void Init()
    {

    }

    protected void CreateSingleton(T instance)
    {
        if (_instance == null)
        {
            _instance = instance;
            DontDestroyOnLoad(this);
            Init();
        }
        else Destroy(this);
    }

}