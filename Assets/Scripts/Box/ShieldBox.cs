using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ShieldBox : MonoBehaviour
{
    [SerializeField] PlayerController controller;

    //public void Shield()
    //{
    //    //shieldbox�� Shield ������ ���� Shield �������̽��� ���� ������!
    //    if (transform.parent.parent.TryGetComponent<IAttackable>(out IAttackable a)) 
    //    {
    //        var shield = this.GetComponentInParent<Shield>();
    //        a.ByShield(shield);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D col)
    {
        // ShieldBox�� ParryBox�� trigger �Ǿ��� ��� 
        if (col.TryGetComponent<HitBox>(out HitBox h))
        {
            var parent = col.transform.parent;
            if (parent.TryGetComponent<IAttackable>(out IAttackable a)&&!h.isBody)
            {
                var shield = this.GetComponentInParent<Shield>();
                this.Log(h);
                a.ByShield(shield);
                controller.PlayerKnockBack();
                this.Log("shield");
            }
        }

    }
}
