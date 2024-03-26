using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

//기본적인 상태 구조는 상위 스크립트에서 해결
//여기서는 각 상태 세부 구현 및 애니메이션 짜기

public class Triangle : Enemy, IAttackable, IDetectable, IDamageable
{
    [SerializeField] protected Transform target;
    [SerializeField] protected RayBox ray;
    [SerializeField] float jumpPower;
    [SerializeField] float dashForce;
    [SerializeField] float dashDistance;
    [SerializeField] protected GameObject hitBox;
    protected Vector2 moveDir;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyState = EnemyState.Idle;
        moveDir = Vector2.left;
    }

    public override void InitSetting()
    {
        enemyController = GetComponent<Enemy>().enemyController;
    }

    public override IEnumerator Behaviour(EnemyState enemyState)
    { 
        yield return null; 
    }

    protected void Move()
    {
        Vector2 newPos = (Vector2)transform.position + (moveDir * moveSpd * Time.deltaTime);
        transform.position = newPos;

        // localScale 방향 착각 가능성 있음
        transform.localScale = (moveDir == Vector2.left) ? new Vector2(1, 1) : new Vector2(-1, 1);
    }

    protected void TurnAround()
    {
        moveDir = -moveDir;
        transform.localScale = new Vector2(-transform.localScale.x,transform.localScale.y);
    }

    protected IEnumerator Jump()
    {
        var jumpDir = new Vector2(-runDir.x, 1);
        rb.AddForce(jumpDir * jumpPower, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        StateChange(EnemyState.Detect);
    }

    protected IEnumerator Dash(float dashForce, float dashDistance)
    {
        if (dashForce < Mathf.Epsilon) yield break;

        rb.AddForce(-runDir * dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashDistance / dashForce);
        rb.velocity = Vector2.zero;

        yield break;
    }

    protected IEnumerator AttackCombo()
    {
        anim.SetTrigger("Attack");

        //애니메이션과 이동 맞추기 위해 시간 조정
        yield return new WaitForSeconds(.5f);

        //공격할때 앞으로 돌진하는 모션
        yield return Dash(dashForce, dashDistance);
        
        yield return new WaitForSeconds(.3f);
        
        //아직도 범위 안에 있으면 2타 실행
        if (TargetDistance(target) < meeleeRange)
        {
            anim.SetTrigger("Attack");

            //애니메이션과 이동 맞추기 위해 시간 조정
            yield return new WaitForSeconds(.25f);

            //공격할때 앞으로 돌진하는 모션
            yield return Dash(dashForce * 0.5f, dashDistance * 0.5f);

            this.Log("attack2");
        }
        
        //공격패턴 끝나면 다시 감지
        StateChange(EnemyState.Detect);
    }

    protected void CalculateDir()
    {
        //방향 설정
        detectPoint = target.position;
        var curPoint = (Vector2)transform.position;
        var dir = curPoint.x - detectPoint.x > 0;
        runDir = dir ? new Vector2(1, 0) : new Vector2(-1, 0);

        transform.localScale = dir ? new Vector2(1, 1) : new Vector2(-1, 1);
    }
   

    protected float TargetDistance(Transform target)
    {
        float dis = Vector2.Distance(transform.position, target.position);
        isMeeleeRange = dis < meeleeRange;
        return dis;
    }

    // IDetectable
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

    // IAttackable
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
        transform.position = (Vector2)transform.position + new Vector2(transform.localScale.x * 0.5f, 0);
    }

    void ParryDisarm()
    {
        disArmCnt++;
        if (disArmCnt == level)
        {
            hitBox.SetActive(false);
            EnemyChange(EnemyType.Civilian);
        }
    }

    // IAttackable
    public void ByShield(Shield shield)
    {
        this.Log("Attacked by Shield");

        gameObject.GetComponent<Rigidbody2D>().AddForce(runDir * shield.shieldForce * (1 / level), ForceMode2D.Impulse);
        //shield.ShieldEffect();
    }

    // IAttackable
    public void BySpear()
    {
        this.Log("Attacked by Spear");
    }

    // IDamageable
    public void Damaged(int dmg)
    {
        //blood effect?
        throw new System.NotImplementedException();
    }

}
