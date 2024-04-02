using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

//기본적인 상태 구조는 상위 스크립트에서 해결
//여기서는 각 상태 세부 구현 및 애니메이션 짜기

public class Triangle : Enemy, IAttackable, IDetectable, IDamageable
{
    [SerializeField] Transform target;
    [SerializeField] RayBox ray;
    [SerializeField] float jumpPower;
    
    [Header("Dash")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDistance;

    public enum EnemyType { Civilian, Soldier, Jumper}
    public EnemyType enemyType;
    [SerializeField] GameObject hitBox;
    Vector2 moveDir;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyState = EnemyState.Idle;
        moveDir = Vector2.left;
        StartCoroutine(EnemyStateHandler());
    }

    //매 프레임마다 확인해야 할 것? bool 애니메이션은 애니메이션 함수로 따로 처리하기 파편화되어있으니 보기 싫다
    // Update is called once per frame
    void Update()
    {
        TriangleAnim();
    }

    protected override IEnumerator Idle()
    {
        switch (enemyType)
        {
            case EnemyType.Civilian:
                isMoving = false;
                yield break;

            case EnemyType.Soldier:
                isMoving = false;
                yield break;

            case EnemyType.Jumper:
                isMoving = true;
                Move();
                if (!ray.CheckWithRay(Vector2.down, 5) || ray.CheckWithRay(moveDir, .5f))
                {
                    isMoving = false;
                    yield return new WaitForSeconds(2f);
                    TurnAround();
                    isMoving = true;
                }
                yield break;
        }

        //triangle 기사는 가만히 있는 컨셉(근위병 같은 느낌?)

        yield return null;
    }

    void Move()
    {
        Vector2 newPos = (Vector2)transform.position + (moveDir * moveSpd * Time.deltaTime);
        transform.position = newPos;

        transform.localScale = (moveDir == Vector2.left) ? new Vector2(1, 1) : new Vector2(-1, 1);
    }

    void TurnAround()
    {
        moveDir = -moveDir;
        transform.localScale = new Vector2(-transform.localScale.x,transform.localScale.y);
    }
    protected override IEnumerator Detect()
    {
        yield return null;
        //방향 설정
        CalculateDir();

        ////meelee range인지 아닌지 체크하고 Attack state로 전환해야한다
        //if (TargetDistance(target) < meeleeRange)
        //{
        //    StateChange(EnemyState.Attack);
        //}

        //state가 바뀌지 않았다면 이동
        isMoving = true;

        //타입에 따른 이동 로직
        switch (enemyType)
        {
            case EnemyType.Civilian:
                //meelee range인지 아닌지 체크하고 Attack state로 전환해야한다
                if (TargetDistance(target) < meeleeRange)
                {
                    StateChange(EnemyState.Attack);
                }
                Vector2 newPos = (Vector2)transform.position + (runDir * moveSpd * Time.deltaTime);
                transform.position = newPos;
                break;
            case EnemyType.Soldier:
                //meelee range인지 아닌지 체크하고 Attack state로 전환해야한다
                if (TargetDistance(target) < meeleeRange)
                {
                    StateChange(EnemyState.Attack);
                }
                Vector2 newPoss = (Vector2)transform.position - (runDir * moveSpd * Time.deltaTime);
                transform.position = newPoss;
                break;
            case EnemyType.Jumper:
                if (TargetDistance(target) < meeleeRange)
                {
                    StateChange(EnemyState.Attack);
                }
                Vector2 runPos = (Vector2)transform.position - (runDir * (moveSpd * 1.5f) * Time.deltaTime);
                transform.position = runPos;
                
                break;

            default:
                break;
        }


    }
    protected override IEnumerator Attack()
    {
        

        // 거리에 들어오면 수행할 동작
        switch (enemyType)
        {
            case EnemyType.Civilian:
                isMoving = false;
                this.Log("dosth");
                StateChange(EnemyState.Idle);
                break;
            case EnemyType.Soldier:
                isMoving = false;
                yield return AttackCombo();
                break;
            case EnemyType.Jumper:
                isMoving = true;
                yield return Jump();
                break;
        }

    }
    protected override IEnumerator Die()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Jump()
    {
        var jumpDir = new Vector2(-runDir.x, 1);
        rb.AddForce(jumpDir * jumpPower, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        StateChange(EnemyState.Detect);
    }
    IEnumerator AttackCombo()
    {
        anim.SetTrigger("Attack");

        //애니메이션과 이동 맞추기 위해 시간 조정
        yield return new WaitForSeconds(.5f);

        //공격할때 앞으로 돌진하는 모션
        yield return Dash(-runDir * dashDistance, dashForce);

        yield return new WaitForSeconds(.3f);
        
        //아직도 범위 안에 있으면 2타 실행
        if (TargetDistance(target) < meeleeRange)
        {
            anim.SetTrigger("Attack");

            //애니메이션과 이동 맞추기 위해 시간 조정
            yield return new WaitForSeconds(.25f);

            //공격할때 앞으로 돌진하는 모션
            yield return Dash(-runDir * dashDistance * 0.5f, dashForce * 0.5f);

            this.Log("attack2");
        }
        
        //공격패턴 끝나면 다시 감지
        StateChange(EnemyState.Detect);
    }


    void CalculateDir()
    {
        //방향 설정
        detectPoint = target.position;
        var curPoint = (Vector2)transform.position;
        var dir = curPoint.x - detectPoint.x > 0;
        runDir = dir ? new Vector2(1, 0) : new Vector2(-1, 0);

        transform.localScale = dir ? new Vector2(1, 1) : new Vector2(-1, 1);
    }
   

    float TargetDistance(Transform target)
    {
        float dis = Vector2.Distance(transform.position, target.position);
        isMeeleeRange = dis < meeleeRange;
        return dis;
    }

    //얘는 감지하는 놈 - DetectBox가 호출
    public void Detect(Transform target)
    {
        //target이 null이 아니면
        if (target != null)
        {
            //매개변수 받은 놈으로 초기화시켜주고
            this.target = target;

            //방향 설정
            detectPoint = target.position;
            var curPoint = (Vector2)transform.position;
            var dir = curPoint.x - detectPoint.x > 0;
            runDir = dir ? new Vector2(1, 0) : new Vector2(-1, 0);

            //state 바꾸기
            StateChange(EnemyState.Detect);
        }

        else
        {
            this.Log($"{target} Out of Sight");
            //state 바꾸기
            StateChange(EnemyState.Idle);
        }
        
    }

    void TriangleAnim()
    {
        switch (enemyType) 
        {
            case EnemyType.Civilian:
                anim.SetBool("isRun", isMoving);
                break;

            case EnemyType.Soldier:
                anim.SetBool("isChase", isMoving);
                break;
            case EnemyType.Jumper:
                anim.SetBool("isChase", isMoving);
                break;
        }
        anim.SetBool("isMeelee", isMeeleeRange);
    }

    public void ByParry(Shield shield)
    {
        //패링하면 disarm 컨셉 - 시민으로 돌아감
        this.Log("Attacked by Parrying");
        ParryKnockBack();
        shield.ParryEffect();
        ParryDisarm();
        
    }

    void ParryKnockBack()
    {
        var parryEffect = (Vector2)gameObject.transform.position + runDir;
        gameObject.transform.position = parryEffect;
    }

    void ParryDisarm()
    {
        disArmCnt++;
        if (disArmCnt == level)
        {
            hitBox.SetActive(false);
            enemyType = EnemyType.Civilian;
        }
    }
    public void ByShield(Shield shield)
    {
        this.Log("Attacked by Shield");

        gameObject.GetComponent<Rigidbody2D>().AddForce(runDir * shield.shieldForce * (1 / level), ForceMode2D.Impulse);
        //shield.ShieldEffect();
    }

    public void BySpear()
    {
        this.Log("Attacked by Spear");
    }

    public void Damaged(int dmg)
    {
        //blood effect?
        throw new System.NotImplementedException();
    }

    IEnumerator Dash(Vector2 vector, float dashForce)
    {
        if (dashForce < Mathf.Epsilon) yield break;
        
        rb.AddForce(vector.normalized * dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(vector.magnitude / dashForce);
        rb.velocity = Vector2.zero;

        yield break;
    }
}
