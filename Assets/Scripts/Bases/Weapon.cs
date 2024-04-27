using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public enum AttackType { Up, Center, Down, };
    protected abstract void Attack(AttackType type);
}
