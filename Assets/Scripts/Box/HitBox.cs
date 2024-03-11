using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class HitBox : MonoBehaviour
{
    public bool isBody;
    [SerializeField] int hitPoint;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // HitBox가 HurtBox와 trigger 되었을 경우 
        if (col.TryGetComponent<HurtBox>(out HurtBox hbox))
        {
            this.Log($"{this.gameObject.name} collided with {hbox.gameObject.name}");
            hbox.Damage(hitPoint);
        }

        // HitBox가 패리 오브젝트와 trigger 되었을 경우 패링되게 할 것인가?
        // 좀 애매 패링 부분은 따로 빼는게 나을 것 같다
        if (col.TryGetComponent<ParryBox>(out ParryBox pbox))
        {
            this.Log($"{this.gameObject.name} collided with {pbox.gameObject.name}");

        }

        // HitBox가 ShieldBox와 trigger 되었을 경우
        // HitBox는 두종류,
        // 1. enemy의 몸에 붙어있는 것 2. 무기에 붙어있는 것
        // 칼에 붙어있는 히트박스가 쉴드박스에 트리거되었을 때만!

        if(col.TryGetComponent<ShieldBox>(out ShieldBox sbox) && !isBody)
        {
            this.Log($"{this.gameObject.name} collided with {sbox.gameObject.name}");

            var parent = GetComponentsInParent<IAttackable>();
            //히트박스 본인의 이펙트
            //parent[0].gameObject.TryGetComponent<IAttackable>(out IAttackable a);
            parent[0].ByShield(sbox.GetComponentInParent<Shield>());

            //트리거된 쉴드박스의 이펙트
            sbox.Shield();

        }
    }

    void HitBoxParent()
    {

    }

    Shield GetShield()
    {
        return GetComponentInParent<Shield>();
    }
}
