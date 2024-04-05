using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class HurtBox : MonoBehaviour
{
    public void Damage(int hitPnt)
    {
        //모든 HurtBox는 Hierarchy 상으로 IDamageable 인터페이스 상속받은 클래스 하위에 속해있어야함
        if(transform.parent.TryGetComponent<IDamageable>(out IDamageable d))
        {
            this.Log("Hurtbox method");
            d.Damaged(hitPnt);
        }
    }
}
