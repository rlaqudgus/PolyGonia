using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

// [TG] [2024-04-05] [feat]
// 패링 공격을 제외한 일반 물리 공격들을 다루는 클래스
// 여러 HitBox들을 참조한 뒤 Attack Type에 맞게 HitBox를 선택
public class Attack : MonoBehaviour
{
    public float weaponForce;
    
    [SerializeField] private GameObject[] hitBoxes; // 공격 타입에 대한 히트박스들
    [SerializeField] private GameObject[] attackEffects; // 공격 타입에 대한 이펙트들
    [SerializeField] private Transform[] effectPositions; // 각 이펙트들의 발생 위치(수정 필요) 몬스터에 피격이펙트 부여하는 것이 더 좋아보임
    
    private PlayerController _playerController;
    private int _curAttackType = 0;
    public int CurAttackType
    {
        get => _curAttackType;
        set => _curAttackType = value;
    }
    
    private void Start()
    {
        _playerController = transform.parent.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_playerController.IsLookUp)
        {
            transform.localScale = new Vector2(1, 1); // 윗공격을 위한 콜라이더
        }
        else if (_playerController.IsLookDown)
        {
            transform.localScale = new Vector2(1, -1); // 하단 공격을 위한 콜라이더 뒤집기
        }
    }
    
    // Attack Type에 맞게 공객 실행 
    public void DoAttack(int attackType)
    {
        CurAttackType = attackType;
        hitBoxes[attackType].SetActive(true);
        StartCoroutine(DisableAttackAfterDelay());
    }

    private IEnumerator DisableAttackAfterDelay()
    {
        yield return new WaitForSeconds(0.05f); // 너무 길면 왼쪽공격하고 오른쪽으로 돌면 오른쪽 몬스터가 피격됨 (HitBox가 Player에 붙어 다니기 때문)
        hitBoxes[_curAttackType].SetActive(false);
    }
    public void AttackEffect()
    {
        this.Log("AttackEffect");
        Instantiate(attackEffects[CurAttackType], effectPositions[CurAttackType].position, Quaternion.identity);
    }
}
