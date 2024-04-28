using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class HurtBox : MonoBehaviour
{
    public void Attacked(Weapon weapon, int dmg)
    {
        if (transform.parent.TryGetComponent<IAttackable>(out IAttackable attackTarget))
        {
            attackTarget.ByWeapon(weapon);
            if (dmg != 0) attackTarget.Damaged(dmg);
        }
    }
}
