using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class Shield : Weapon
{
    private bool _isParrying;
    private BoxCollider2D _collider;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();    
        _collider.enabled = false;
        _isParrying = false;
    }

    public override void UseWeapon(int idx) => StartCoroutine(ParryReset());

    private IEnumerator ParryReset()
    {
        _isParrying = true;
        yield return new WaitForSeconds(.3f);
        _isParrying = false;
    }

    public override void UseShield(bool on) => _collider.enabled = on;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Parry") && _isParrying)
        {
            this.Log("parry");
            var parent = col.transform.parent;
            if (!parent.TryGetComponent<IAttackable>(out IAttackable a)) return;

            EffectManager.Instance.InstantiateEffect(ParticleColor.YELLOW, (transform.position + col.transform.position) * .5f);
            
            WeaponAction?.Invoke();

            CameraManager.Instance.Shake();
            CameraManager.Instance.Zoom(6, 0);
            CameraManager.Instance.ResetCamera(.15f);
            
            JoyConManager.Instance?.j[0].SetRumble(160, 320, 0.6f, 200);
        }

        else if(col.CompareTag("Parry") && !_isParrying)
        {
            EffectManager.Instance.InstantiateEffect(ParticleColor.WHITE, (transform.position + col.transform.position) * .5f + Vector3.up);
        }
    }
}
