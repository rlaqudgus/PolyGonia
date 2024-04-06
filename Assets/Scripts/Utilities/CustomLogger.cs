using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class CustomLogger
    {
        //확장 메서드 사용
        //default - print Type & object name
        public static void Log(this Object obj)
        {
            Debug.Log($"[{obj.GetType()}] [{obj.name}]");
        }

        //overload 1 - log message print & custom color to the msg if u want to
        public static void Log(this Object obj, object print, string msgColor)
        {
            Debug.Log($"[{obj.GetType()}] [{obj.name}] : <color={msgColor}>{print}</color>\n", obj);
        }

        //overload 2 - colorbyType default - true
        public static void Log(this Object obj, object print, bool colorByType=true)
        {
            if(colorByType)
            {
                switch (obj)
                {
                    case PlayerController:
                        Debug.Log($"[<color=lightblue>{obj.GetType()}</color>] [{obj.name}] : <color=white>{print}</color>\n", obj);
                        break;
                    case Enemy:
                        Debug.Log($"[<color=red>{obj.GetType()}</color>] [{obj.name}] : <color=white>{print}</color>\n", obj);
                        break;
                    default:
                        Debug.Log($"[{obj.GetType()}] [{obj.name}] : <color=white>{print}</color>\n", obj);
                        break;
                }
            }

            else
            {
                Debug.Log($"[{obj.GetType()}] [{obj.name}] : <color=white>{print}</color>\n", obj);
            }

        }
    }

}


