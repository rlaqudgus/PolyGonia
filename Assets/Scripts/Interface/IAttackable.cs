using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    
    public void ByShield(Shield shield);

    public void ByParry(Shield shield);

    // [TG] [2024-04-01] [Refactor]
    // 1. BySpear에서 ByWeapon으로 변경
    // 2. 각 무기들 별로 따로 처리하는 것이 아닌 일반 공격으로 통일 후 각 객체에서 조절
    public void ByWeapon(Attack attack);

}
