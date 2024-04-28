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
            if (_instance == null) Debug.Log($"{typeof(T)} is Empty");
            return _instance;
        }
    }

    protected void CreateSingleton(T instance) => _instance = instance;
}