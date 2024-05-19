using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class HitBox : MonoBehaviour
{
    public bool isBody;
    public bool dontHitPlayer;
    
    [SerializeField] private int hitPoint;
    [SerializeField]private Weapon _weapon;
    
    private void Start()
    {
        if (_weapon == null) _weapon = GetComponentInParent<Weapon>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // HitBox가 HurtBox와 trigger 되었을 경우 
        if (col.TryGetComponent(out HurtBox hbox) && (!dontHitPlayer|| !col.CompareTag("Player")))
        {
            this.Log($"{this.gameObject.name} collided with {hbox.gameObject.name}");
            hbox.Attacked(_weapon, hitPoint);
        }
    }
}
