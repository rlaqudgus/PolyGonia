using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class WeaponController : Singleton<WeaponController> //Player�� �޾Ƶα�
{
    [SerializeField] Weapon[] _weaponArray;

    PlayerController _player;

    const int _swordIdx = 0;
    const int _shieldIdx = 1;


    private void Start()
    {
        CreateSingleton(this);

        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _weaponArray = gameObject.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in _weaponArray) weapon.player = _player;
    }
    
    public void UseShield() => this._weaponArray[_shieldIdx].UseShield();
    public void UseWeapon(int idx)
    {
        if(_player.isShield) this._weaponArray[_shieldIdx].UseWeapon(idx);
        else this._weaponArray[_swordIdx].UseWeapon(idx);
    }
}

public abstract class Weapon : MonoBehaviour
{
    public float weaponForce;
    public PlayerController player;
    public abstract void UseWeapon(int idx);
    public abstract void UseShield();
}