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
        yield return new WaitForSeconds(0.05f); // �ʹ� ��� ���ʰ����ϰ� ���������� ���� ������ ���Ͱ� �ǰݵ� (HitBox�� Player�� �پ� �ٴϱ� ����)
        transform.GetChild(idx).gameObject.SetActive(false);
    }
}
