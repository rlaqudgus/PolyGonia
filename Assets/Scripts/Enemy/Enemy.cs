using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
