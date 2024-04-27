using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
    bool isColliding;

    [SerializeField] GameObject _shieldBox;
    [SerializeField] GameObject _shieldParry;


    public override void UseWeapon(int idx) => StartCoroutine(ParryReset());

    IEnumerator ParryReset()
    {
        _shieldParry.SetActive(true);
        _shieldBox.SetActive(false);
        yield return new WaitForSeconds(.3f);
        _shieldParry.SetActive(false);

        //if (!player._isShield) yield break;

        _shieldBox.SetActive(true);
    }

    public override void UseShield() => _shieldBox.SetActive(true);
}
