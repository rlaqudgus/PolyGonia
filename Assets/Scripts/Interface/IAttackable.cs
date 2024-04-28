using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    //Weapon으로 합병
    public void ByParry(Shield shield);
    public void ByWeapon(Weapon weapon);
    public void Damaged(int Dmg);
}
