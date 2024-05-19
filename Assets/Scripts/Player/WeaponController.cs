using System;
using System.Collections;
using System.Collections.Generic;
using Bases;
using UnityEngine;
using Utilities;
public class WeaponController : MonoBehaviour//Player�� �޾Ƶα�
{
    [SerializeField] private Weapon[] _weaponArray;
    [HideInInspector] public Action[] WeaponAction;
    
    private PlayerController _player;

    #region WeaponIdx

    private const int SWORD_IDX = 0;
    private const int SHIELD_IDX = 1;

    #endregion

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _weaponArray = gameObject.GetComponentsInChildren<Weapon>();
    }

    public void AddWeaponAction(int idx, Action action) => _weaponArray[idx].WeaponAction += action;
    public void UseShield(bool on) => _weaponArray[SHIELD_IDX].UseShield(on);
    public void UseWeapon(int idx)
    {
        if(_player.isShield) this._weaponArray[SHIELD_IDX].UseWeapon(idx);
        else this._weaponArray[SWORD_IDX].UseWeapon(idx);
    }
}

public abstract class Weapon : MonoBehaviour
{
    public float weaponForce;
    public Action WeaponAction;
    
    public abstract void UseWeapon(int idx);
    public abstract void UseShield(bool on);
}