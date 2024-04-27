using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class WeaponController : Singleton<WeaponController> //Player에 달아두기
{
    [SerializeField] Weapon[] _weaponArray;
    public int _currentWeaponIdx;

    private void Start()
    {
        CreateSingleton(this);
        _currentWeaponIdx = 0;

        _weaponArray = gameObject.GetComponentsInChildren<Weapon>();
    }
    
    public void UseShield() => this._weaponArray[_currentWeaponIdx].UseShield();
    public void UseWeapon(int idx) => this._weaponArray[_currentWeaponIdx].UseWeapon(idx);

    public bool GetWeaponType<T>() where T : Weapon => _weaponArray[_currentWeaponIdx].GetType() == typeof(T);
}

public abstract class Weapon : MonoBehaviour
{
    public float weaponForce;
    public abstract void UseWeapon(int idx);
    public abstract void UseShield();
}