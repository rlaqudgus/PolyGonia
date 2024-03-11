using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //���� Ŭ�������� type ����?
    
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

    //�÷��̾� ���� �� behaviour
    protected abstract IEnumerator Idle();
    //�÷��̾� ���� �� ���� �������� behaviour
    protected abstract IEnumerator Detect();
    //���� behaviour
    protected abstract IEnumerator Attack();
    //��� behaviour
    protected abstract IEnumerator Die();
}
