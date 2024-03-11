using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

//�⺻���� ���� ������ ���� ��ũ��Ʈ���� �ذ�
//���⼭�� �� ���� ���� ���� �� �ִϸ��̼� ¥��

public class Triangle : Enemy, IAttackable, IDetectable, IDamageable
{
    [SerializeField] Transform target;
    [SerializeField] RayBox ray;
    [SerializeField] float jumpPower;
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

    //�� �����Ӹ��� Ȯ���ؾ� �� ��? bool �ִϸ��̼��� �ִϸ��̼� �Լ��� ���� ó���ϱ� ����ȭ�Ǿ������� ���� �ȴ�
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

        //triangle ���� ������ �ִ� ����(������ ���� ����?)

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
        //���� ����
        CalculateDir();

        ////meelee range���� �ƴ��� üũ�ϰ� Attack state�� ��ȯ�ؾ��Ѵ�
        //if (TargetDistance(target) < meeleeRange)
        //{
        //    StateChange(EnemyState.Attack);
        //}

        //state�� �ٲ��� �ʾҴٸ� �̵�
        isMoving = true;

        //Ÿ�Կ� ���� �̵� ����
        switch (enemyType)
        {
            case EnemyType.Civilian:
                //meelee range���� �ƴ��� üũ�ϰ� Attack state�� ��ȯ�ؾ��Ѵ�
                if (TargetDistance(target) < meeleeRange)
                {
                    StateChange(EnemyState.Attack);
                }
                Vector2 newPos = (Vector2)transform.position + (runDir * moveSpd * Time.deltaTime);
                transform.position = newPos;
                break;
            case EnemyType.Soldier:
                //meelee range���� �ƴ��� üũ�ϰ� Attack state�� ��ȯ�ؾ��Ѵ�
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
        

        // �Ÿ��� ������ ������ ����
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

        //�ִϸ��̼ǰ� �̵� ���߱� ���� �ð� ����
        yield return new WaitForSeconds(.5f);

        //�����Ҷ� ������ �����ϴ� ���
        var attack = (Vector2)gameObject.transform.position - (runDir) * 0.63f;
        gameObject.transform.position = attack;

        yield return new WaitForSeconds(.3f);
        
        //������ ���� �ȿ� ������ 2Ÿ ����
        if (TargetDistance(target) < meeleeRange)
        {
            anim.SetTrigger("Attack");

            this.Log("attack2");
        }
        
        //�������� ������ �ٽ� ����
        StateChange(EnemyState.Detect);
    }


    void CalculateDir()
    {
        //���� ����
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

    //��� �����ϴ� �� - DetectBox�� ȣ��
    public void Detect(Transform target)
    {
        //target�� null�� �ƴϸ�
        if (target != null)
        {
            //�Ű����� ���� ������ �ʱ�ȭ�����ְ�
            this.target = target;

            //���� ����
            detectPoint = target.position;
            var curPoint = (Vector2)transform.position;
            var dir = curPoint.x - detectPoint.x > 0;
            runDir = dir ? new Vector2(1, 0) : new Vector2(-1, 0);

            //state �ٲٱ�
            StateChange(EnemyState.Detect);
        }

        else
        {
            this.Log("??");
            //state �ٲٱ�
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
        //�и��ϸ� disarm ���� - �ù����� ���ư�
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
}
