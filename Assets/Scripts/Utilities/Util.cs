using System;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace Utilities
{
    public static class Util
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
            if (component == null) component = go.AddComponent<T>();

            return component;
        }

        public static T FindChild<T>(this GameObject obj, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (!recursive)
            {
                for (var i = 0; i < obj.transform.childCount; i++)
                {
                    var child = obj.transform.GetChild(i);
                    if (!string.IsNullOrEmpty(name) && child.name != name) continue;
                    var childT = child.GetComponent<T>();
                    if (childT != null) return childT;
                }
            }
            else
            {
                return obj.GetComponentsInChildren<T>().FirstOrDefault(childT => string.IsNullOrEmpty(name) || childT.name == name);
            }
            return null;
        }
    }
}