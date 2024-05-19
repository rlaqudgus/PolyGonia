using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Sword : Weapon
{
    private void Awake()
    {
        for (var i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);
    }

    public override void UseShield(bool on) { }

    public override void UseWeapon(int idx)
    {
        transform.GetChild(idx).gameObject.SetActive(true);
        StartCoroutine(DisableAttackAfterDelay(idx));
    }

    private IEnumerator DisableAttackAfterDelay(int idx)
    {
        yield return new WaitForSeconds(0.1f); 
        transform.GetChild(idx).gameObject.SetActive(false);
    }
}
