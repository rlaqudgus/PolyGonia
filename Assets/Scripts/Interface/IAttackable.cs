using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public void Damaged(int dmg, Weapon weapon);
}
