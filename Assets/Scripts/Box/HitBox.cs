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
    }
}
