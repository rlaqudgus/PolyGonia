using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebgugManager : MonoBehaviour
{
    public List<string> items;
    public static List<string> debugitems;
    private void OnEnable()
    {
        debugitems = items;
    }
}
