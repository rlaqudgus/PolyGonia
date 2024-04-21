using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

// EnemyManager는 Enemy보다 반드시 먼저 초기화되어야 한다
// EnemyManager의 ExecutionOrder가 지켜지지 않으면 초기화 시 문제가 생긴다

[DefaultExecutionOrder(100)]
public abstract class Enemy : MonoBehaviour
{
    //하위 클래스에서 type 구현?
    
    [SerializeField] protected enum EnemyState { Idle, Detect, Attack, Die }
    [SerializeField] protected EnemyState _enemyState;

    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _dmg;
    [SerializeField] protected float _moveSpd;
    [SerializeField] protected float _meeleeRange;

    protected Animator _anim;

    protected bool _isMeeleeRange;
    protected bool _isMoving = true;

    public Vector2 chaseDir;
    public Vector2 runDir;
    protected Vector2 _detectPoint;

    public int level;
    public int disArmCnt;

    protected Rigidbody2D _rb;

    protected IEnumerator EnemyStateHandler()
    {
        while (true)
        {
            yield return StartCoroutine(_enemyState.ToString());
        }
    }

    protected void StateChange(EnemyState state)
    {
        _enemyState = state;
    }

    //플레이어 감지 전 behaviour
    protected abstract IEnumerator Idle();
    //플레이어 감지 후 공격 전까지의 behaviour
    protected abstract IEnumerator Detect();
    //공격 behaviour
    protected abstract IEnumerator Attack();
    //사망 behaviour
    protected abstract IEnumerator Die();
    // [TG] [2024-04-04] [refactor]
    // 1. 적 넉백 구현 (PlayerKnockBack과 같은 로직)
    protected abstract void EnemyKnockBack(float knockBackDist);

    // OnEable: Instantiate, SetActive(true) 시 호출됨
    // OnDisable: Destroy, SetActive(false) 시 호출됨
    // EnemyManager에게 Enemy의 활성/비활성 정보를 전달하기 위해 사용

    protected virtual void OnEnable() 
    {
        EnemyManager.Instance.Add(this);
    }

    protected virtual void OnDisable()
    {   
        // EnemyManager.Instance != null 임을 검사한지 않는다면
        // 프로그램 종료 시 EnemyManger가 Enemy보다 먼저 비활성화되면 에러가 표시된다
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.Remove(this);
        }
    }

}
