using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //하위 클래스에서 type 구현?
    
    [SerializeField] protected enum EnemyState {Idle, Detect, Attack, Die }
    [SerializeField] protected EnemyState enemyState;

    [SerializeField] protected int maxHp;
    [SerializeField] protected int hp;
    [SerializeField] protected int dmg;
    [SerializeField] protected float moveSpd;
    [SerializeField] protected float meeleeRange;

    protected Animator anim;

    protected bool isMeeleeRange;
    protected bool isMoving = true;

    public Vector2 runDir;
    protected Vector2 detectPoint;

    public int level;
    public int disArmCnt;

    protected Rigidbody2D rb;

    protected IEnumerator EnemyStateHandler()
    {
        while (hp > 0)
        {
            yield return StartCoroutine(enemyState.ToString());   
        }

        StartCoroutine(Die());
    }

    protected void StateChange(EnemyState state)
    {
        enemyState = state;
    }

    //플레이어 감지 전 behaviour
    protected abstract IEnumerator Idle();
    //플레이어 감지 후 공격 전까지의 behaviour
    protected abstract IEnumerator Detect();
    //공격 behaviour
    protected abstract IEnumerator Attack();
    //사망 behaviour
    protected abstract IEnumerator Die();
}
