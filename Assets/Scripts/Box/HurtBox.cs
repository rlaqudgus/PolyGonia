using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class HurtBox : MonoBehaviour
{
    public void Damage(int hitPnt)
    {
        //��� HurtBox�� Hierarchy ������ IDamageable �������̽� ��ӹ��� Ŭ���� ������ �����־����
        if(transform.parent.TryGetComponent<IDamageable>(out IDamageable d))
        {
            this.Log("Hurtbox method");
            d.Damaged(hitPnt);
        }
        
    }
}
