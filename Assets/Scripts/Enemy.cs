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

    //private void OnTriggerStay2D(Collider2D col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        //�÷��̾ detect ���� �ȿ� ������
    //        //1. ��ġ �ľ� �� ������ ���� ����
    //        //2. ������ �ʿ���� ������ �� bool ���� ����
    //        //3. ���� �� ���ͷ��� ������ ���� Ȯ�� �� �ڷ�ƾ ����
    //        Detect(col);

    //        //detection ���� �ȿ� �־ ���� ������� ������ �ʿ� x �� ���� �ڷ�ƾ�� ���� ���̶�� �������� �ʰ�
    //        if (isMeeleeRange || !isMoving) return;

    //        //���� �ȿ��� ��� ����Ǿ�� �ϴ� ��� - �Ѿƿ���(soldier) or ����ġ��(civilian)
    //        EnemyMovement();
    //    }
        
    //}

    //void EnemyMovement()
    //{
    //    switch (enemyType)
    //    {
    //        case EnemyType.Civilian:
    //            anim.SetBool("isRun", isMoving);
    //            Vector2 newPos = (Vector2)transform.position + (runDir * moveSpd * Time.deltaTime);
    //            transform.position = newPos;
    //            break;
    //        case EnemyType.Soldier:
    //            anim.SetBool("isChase", isMoving);
    //            Vector2 newPoss = (Vector2)transform.position - (runDir * moveSpd * Time.deltaTime);
    //            transform.position = newPoss;
    //            break;
    //        default:
    //            break;
    //    }

    //}

    //private void OnTriggerExit2D(Collider2D col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        anim.SetBool("isRun", false);
    //        anim.SetBool("isChase", false);
    //    }
    //}

    //void Detect(Collider2D col)
    //{
    //    //1.��ġ �ľ� �� direction ����
    //    detectPoint = col.transform.position;
    //    var curPoint = (Vector2)transform.position;
    //    var dir = curPoint.x - detectPoint.x > 0;
    //    runDir = dir ? new Vector2(1, 0) : new Vector2(-1, 0);

    //    //2. �Ÿ��� ���� ���� ������ Ȯ�� - meelee ���ͷ����� ������ ��
    //    if (Mathf.Abs(curPoint.x - detectPoint.x) < meeleeRange)
    //    {
    //        isMeeleeRange = true;
    //        //runDir = Vector2.zero;

    //        //�ڷ�ƾ�� �� �ѹ� �����ϱ� ���� ���ǹ� - isMoving�� �ڷ�ƾ ���� �� false, ���� �� true�� �Ǳ� ������
    //        //�̹� �ڷ�ƾ�� ����ǰ� �ִٸ� �б�� ���� �ʴ´� 
    //        if (isMoving)
    //        {
    //            StartCoroutine(EnemyBehaviour());
    //        }
    //        return;
    //    }
    //    else isMeeleeRange = false;

    //    transform.localScale = dir ? new Vector2(1,1) : new Vector2(-1,1);
    //}

    //���� �ʿ� x
    //private void OnTriggerEnter2D(Collider2D col)
    //{
    //    if(col.CompareTag("Player"))
    //    {
    //        Debug.Log("Player Detected");
    //        detectPoint = col.transform.position;

    //        var curPoint = (Vector2)transform.position;     
    //        var dir = curPoint.x - detectPoint.x > 0;

    //        runDir = dir ? new Vector2(1,0): new Vector2(-1,0);
    //    }
    //}
    
    //�ܰ谡 �ִ� ������ �� �������� �ڷ�ƾ����
    //IEnumerator EnemyBehaviour()
    //{
    //    Debug.Log("coroutine start");
    //    //���� �ڷ�ƾ �������̸� ismoving false�� ��ȯ
    //    isMoving = false;

    //    //�ڷ�ƾ �����߿��� �ٸ� �ִϸ��̼����� ��ȯ x
    //    anim.SetBool("isChase", isMoving);
    //    anim.SetBool("isRun", isMoving);

    //    switch (enemyType)
    //    {
    //        case EnemyType.Civilian:
    //            //dosth maybe interact?
    //            Debug.Log("Greetings");
    //            break;
    //        case EnemyType.Soldier:
                
    //            anim.SetTrigger("Attack");
    //            //�ִϸ��̼ǰ� �̵� ���߱� ���� �ð� ����
    //            yield return new WaitForSeconds(.5f);
    //            var attack = (Vector2)gameObject.transform.position - (runDir)*0.5f;
    //            gameObject.transform.position = attack;
    //            Debug.Log("attack1");

    //            yield return new WaitForSeconds(1);

    //            //������ ���� �ȿ� ������ 2Ÿ ����
    //            if(isMeeleeRange)
    //            {
    //                anim.SetTrigger("Attack");
                    
    //                Debug.Log("attack2");
    //            }
                
    //            //yield return new WaitForSeconds(.5f);
    //            break;
    //        default:
    //            break;
    //    }

    //    //yield return new WaitForSeconds(.5f);

    //    isMoving = true;
    //    Debug.Log("coroutine end");
    //    yield return null;
       
    //}

    

    

    IEnumerator DisarmEffect()
    {
        isMoving = false;
        //�ִϸ��̼� �߰�?
        yield return new WaitForSeconds(1f);

        isMoving = true;
    }
}
