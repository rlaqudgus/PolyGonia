using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    //하위 클래스에서 type 구현?
    
    [SerializeField] public EnemyState enemyState;
    [SerializeField] public EnemyType enemyType;
    [SerializeField] public EnemyController enemyController;
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

    protected void StateChange(EnemyState state)
    {
        enemyState = state;
        enemyController.enemyState = state;
    }

    protected void EnemyChange(EnemyType type)
    {
        enemyType = type;
        enemyController.EnemyChange(type);
    }

    // Enemy 초기 세팅
    public abstract void InitSetting();

    // Enemy 행동 전략 - Idle, Detect, Attack, Die
    public abstract IEnumerator Behaviour(EnemyState enemyState);
}
