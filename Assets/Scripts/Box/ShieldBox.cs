using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ShieldBox : MonoBehaviour
{
    //[SerializeField] PlayerController controller;

    public void Shield()
    {
        //shieldbox는 Shield 하위로 존재 Shield 인터페이스를 따로 만들자!
        //parent parent는 쌉에바다
        if (transform.parent.parent.TryGetComponent<IAttackable>(out IAttackable a))
        {
            var shield = this.GetComponentInParent<Shield>();
            //shield.ShieldEffect();
            //a.ByShield(shield);
        }
    }

    //private void OnTriggerEnter2D(Collider2D col)
    //{
    //    // ShieldBox가 HitBox와 trigger 되었을 경우 
    //    if (col.TryGetComponent<HitBox>(out HitBox h))
    //    {
    //        var parent = col.transform.parent;
    //        if (parent.TryGetComponent<IAttackable>(out IAttackable a)&&!h.isBody)
    //        {
    //            var shield = this.GetComponentInParent<Shield>();
    //            this.Log(h);
    //            a.ByShield(shield);
    //            controller.PlayerKnockBack();
    //            this.Log("shield");
    //        }
    //    }

    //}
}
