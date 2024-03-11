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
        // HitBox�� HurtBox�� trigger �Ǿ��� ��� 
        if (col.TryGetComponent<HurtBox>(out HurtBox hbox))
        {
            this.Log($"{this.gameObject.name} collided with {hbox.gameObject.name}");
            hbox.Damage(hitPoint);
        }

        // HitBox�� �и� ������Ʈ�� trigger �Ǿ��� ��� �и��ǰ� �� ���ΰ�?
        // �� �ָ� �и� �κ��� ���� ���°� ���� �� ����
        if (col.TryGetComponent<ParryBox>(out ParryBox pbox))
        {
            this.Log($"{this.gameObject.name} collided with {pbox.gameObject.name}");

        }

        // HitBox�� ShieldBox�� trigger �Ǿ��� ���
        // HitBox�� ������,
        // 1. enemy�� ���� �پ��ִ� �� 2. ���⿡ �پ��ִ� ��
        // Į�� �پ��ִ� ��Ʈ�ڽ��� ����ڽ��� Ʈ���ŵǾ��� ����!

        if(col.TryGetComponent<ShieldBox>(out ShieldBox sbox) && !isBody)
        {
            this.Log($"{this.gameObject.name} collided with {sbox.gameObject.name}");

            var parent = GetComponentsInParent<IAttackable>();
            //��Ʈ�ڽ� ������ ����Ʈ
            //parent[0].gameObject.TryGetComponent<IAttackable>(out IAttackable a);
            parent[0].ByShield(sbox.GetComponentInParent<Shield>());

            //Ʈ���ŵ� ����ڽ��� ����Ʈ
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
