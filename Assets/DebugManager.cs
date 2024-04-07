using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] List <string> _findClass = new List<string>();
    public static List <string> _className = new List<string>();

    private void Awake()
    {
        _className = _findClass;
    }

    public static bool CheckDebugName(string className)
    {
        //리스트에 아무것도 없으면 true - 전부 다 출력
        if (_className.Count==0)
            return true;
        //디버그가 호출된 현재 클래스 이름이 리스트 안에 있다면 출력
        else if (_className.Contains(className))
            return true;
        //이름이 다른 경우 출력 x
        else return false;
    }
}
