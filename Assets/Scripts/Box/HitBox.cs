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
    }
}
