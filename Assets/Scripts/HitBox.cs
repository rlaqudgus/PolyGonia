using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] bool shieldHit;
    [SerializeField] bool shieldParry;
    [SerializeField] int hitPoint;
    //무조건 수정!!!!
    [SerializeField] PlayerController playerController;
    //private void OnCollisionEnter2D(Collision2D col)
    //{
    //    if (col.gameObject.TryGetComponent<IAttackable>(out IAttackable a))
    //    {
    //        if (shieldHit)
    //        {
    //            var shield = this.GetComponentInParent<Shield>();

    //            //a.ByShield(shield);
    //            Debug.Log("hit");

    //        }

    //        //else if (shieldParry) 
    //        //{
    //        //    var shield = this.GetComponentInParent<Shield>();

    //        //    a.ByParry(shield);
    //        //    Debug.Log("parry");
    //        //}
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Parry"))
        {
            var parent = col.transform.parent;
            if (parent.TryGetComponent<IAttackable>(out IAttackable a))
            {
                var shield = this.GetComponentInParent<Shield>();

                a.ByShield(shield);
                playerController.PlayerKnockBack();
                Debug.Log("shield");
            }
        }

        //damage 입을 수 있는 오브젝트라면 해당 Damaged 메서드 실행
        if (col.TryGetComponent<IDamageable>(out IDamageable d))
        {
            d.Damaged(hitPoint);
        }
    }
}
