using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : Singleton<DontDestroyObject>
{
    private void Awake()
    {
        CreateSingleton(this, true);
    }
}
