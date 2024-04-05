using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

// 패링 공격을 제외한 일반 물리 공격들을 다루는 클래스
// 여러 HitBox들을 참조한 뒤 Attack Type에 맞게 HitBox를 선택
public class Attack : MonoBehaviour
{
    public enum AttackType { NormalAttack, DashAttack, UpAttack, DownAttack, }
    public float weaponForce;
    
    [SerializeField] private GameObject[] hitBoxes;
    [SerializeField] private GameObject[] AttackEffects;
    [SerializeField] private float attackDamage;
    
    private Transform[] _effectPositions;
    private int _curAttackType;

    public int CurAttackType
    {
        get => _curAttackType;
        set => _curAttackType = value;
    }

    public void DoAttack(int attackType)
    {
        hitBoxes[attackType].SetActive(true);
        
        hitBoxes[attackType].SetActive(false);
    }
    public void AttackEffect(int attackType)
    {
        this.Log("AttackEffect");
        Instantiate(AttackEffects[attackType], _effectPositions[attackType].position, Quaternion.identity);
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<IAttackable>(out IAttackable attackTarget))
        {
            attackTarget.ByWeapon(this);
        }
    }
}
