using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    public override void UseShield() { }

    public override void UseWeapon(int idx)
    {
        transform.GetChild(idx).gameObject.SetActive(true);
        StartCoroutine(DisableAttackAfterDelay(idx));
    }

    private IEnumerator DisableAttackAfterDelay(int idx)
    {
        yield return new WaitForSeconds(0.05f); // 너무 길면 왼쪽공격하고 오른쪽으로 돌면 오른쪽 몬스터가 피격됨 (HitBox가 Player에 붙어 다니기 때문)
        transform.GetChild(idx).gameObject.SetActive(false);
    }
}
