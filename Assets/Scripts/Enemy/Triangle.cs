using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilities;

//기본적인 상태 구조는 상위 스크립트에서 해결
//여기서는 각 상태 세부 구현 및 애니메이션 짜기

public class Triangle : Enemy, IAttackable, IDetectable, IDamageable
{

    [SerializeField] protected Transform target;
    [SerializeField] protected RayBox ray;
    [SerializeField] float jumpPower;
    
    [Header("Dash")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDistance;

    // public enum EnemyType { Civilian, Soldier, Jumper }
    // public EnemyType enemyType;
    [SerializeField] GameObject hitBox;
    protected Vector2 moveDir;

    void Start()
    {
        hp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyState = EnemyState.Idle;
        moveDir = Vector2.left;
        StartCoroutine(EnemyStateHandler());
        EnemyManager.instance.AddEnemy(gameObject);
    }

    //매 프레임마다 확인해야 할 것? bool 애니메이션은 애니메이션 함수로 따로 처리하기 파편화되어있으니 보기 싫다
    // Update is called once per frame
    void Update()
    {
        TriangleAnim();
    }

    protected override IEnumerator Idle()
    {

        /*
            case EnemyType.Jumper:
                isMoving = true;
                Move();
                // [TG] [2024-04-06] [Refactor]
                // 1. Raybox의 CheckWithRay에 시작 위치 매개변수를 추가하였으므로 같이 수정 
                // 2. 기존 Raybox의 trasnform.position이 (-0.5, 0, 0) 이고 BoxCollider의 Offset이 (0, 0) 임을 반영하여 변경
                // 3. 기존 값을 확인하는 것은 비효율적이기 때문에 참조를 할 수 있게 수정이 필요해 보임
                if (!ray.CheckWithRay(transform.position + new Vector3(-0.5f, 0, 0), Vector2.down, 5)
                    || ray.CheckWithRay(transform.position + new Vector3(-0.5f, 0, 0), moveDir, .5f))
                {
                    isMoving = false;
                    yield return new WaitForSeconds(2f);
                    TurnAround();
                    isMoving = true;
                }
                yield break;
        */
        
        //triangle 기사는 가만히 있는 컨셉(근위병 같은 느낌?)

        yield return null;
    }

    protected override IEnumerator Detect()
    {
        yield return null;
    }

    protected override IEnumerator Attack()
    {   
        yield return null;  
    }   

    protected override IEnumerator Die()
    {
        throw new System.NotImplementedException();
    }
    
    protected virtual void TriangleAnim()
    {
        
    }

    protected void Move()
    {   
        Vector2 newPos = (Vector2)transform.position + (moveDir * moveSpd * Time.deltaTime);
        transform.position = newPos;

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

    protected IEnumerator Dash(Vector2 vector, float dashForce)
    {
        if (dashForce < Mathf.Epsilon) yield break;
        
        rb.AddForce(vector.normalized * dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(vector.magnitude / dashForce);
        rb.velocity = Vector2.zero;
        yield break;
    }   

    protected IEnumerator AttackCombo()
    {
        
        anim.SetTrigger("Attack");

        //애니메이션과 이동 맞추기 위해 시간 조정
        yield return new WaitForSeconds(.2f);

        //공격할때 앞으로 돌진하는 모션
        yield return Dash(-runDir * dashDistance, dashForce);

        yield return new WaitForSeconds(.3f);
        
        //target이 null이 아니고 아직도 범위 안에 있으면 2타 실행
        if (target && TargetDistance(target) < meeleeRange)
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
    

    protected void CalculateDir()
    {
        if (!target) return;
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
            this.target = target;
            isMeeleeRange = false;
            this.Log($"{target} Out of Sight");
            //state 바꾸기
            StateChange(EnemyState.Idle);
        }    
    }

    public void ByParry(Shield shield)
    {
        //패링하면 disarm 컨셉 - 시민으로 돌아감
        this.Log("Attacked by Parrying");
        EnemyKnockBack(1.0f);
        shield.ParryEffect();
        ParryDisarm();
        
    }

    // [TG] [2024-04-04] [refactor]
    // 1. 기존 ParryKnockBack을 Parent객체의 EnemyKnockBack을 override하는 방식으로 변경 
    protected override void EnemyKnockBack(float knockBackDist)
    {
        transform.position = (Vector2)transform.position + new Vector2(runDir.x * knockBackDist,0);
    }

    // IAttackable - ByParry
    protected void ParryDisarm()
    {
        disArmCnt++;
        if (disArmCnt == level)
        {
            hitBox.SetActive(false);
            //enemyType = EnemyType.Civilian;
            EnemyManager.instance.ChangeEnemy(gameObject, "Civilian");
        }
    }

    // IAttackable
    public void ByShield(Shield shield)
    {
        this.Log("Attacked by Shield");

        gameObject.GetComponent<Rigidbody2D>().AddForce(runDir * shield.shieldForce * (1 / level), ForceMode2D.Impulse);
        //shield.ShieldEffect();
    }

    // [TG] [2024-04-04] [feat]
    // 1. Triangle이 Player의 무기로 공격당했을 때
    // 2. blood 효과?
    public void ByWeapon(Attack attack)
    {
        this.Log("Attacked by Weapon");
        attack.AttackEffect();
        gameObject.GetComponent<Rigidbody2D>().AddForce(runDir * attack.weaponForce * (1 / level), ForceMode2D.Impulse);
    }

    // IDamageable
    public void Damaged(int dmg)
    {
        //여기서 분기처리?
        //코루틴 최상위에서 hp가 0이 되면 자동으로 Death 코루틴이 실행되도록 했으나, Attack 하위 코루틴이 실행될때는 코드 흐름 중지,
        //최상위 코루틴은 딜레이되어서 0.n초간 딜레이가 있음;
        //
        hp -= dmg;
        this.Log($"currentHp : {hp} - {dmg} = {hp-dmg}");
        
        if (hp<=0)
        {
            //Death effect
            Destroy(this.gameObject);
        }
        else
        {
            //blood effect?
            //dosth
        }

    }

}
