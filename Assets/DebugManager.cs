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
        //����Ʈ�� �ƹ��͵� ������ true - ���� �� ���
        if (_className.Count==0)
            return true;
        //����װ� ȣ��� ���� Ŭ���� �̸��� ����Ʈ �ȿ� �ִٸ� ���
        else if (_className.Contains(className))
            return true;
        //�̸��� �ٸ� ��� ��� x
        else return false;
    }
}
