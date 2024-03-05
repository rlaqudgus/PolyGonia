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

    //private void OnTriggerStay2D(Collider2D col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        //플레이어가 detect 범위 안에 들어오면
    //        //1. 위치 파악 후 움직일 방향 설정
    //        //2. 움직일 필요없는 조건일 때 bool 변수 변경
    //        //3. 공격 등 인터랙션 가능한 범위 확인 후 코루틴 실행
    //        Detect(col);

    //        //detection 범위 안에 있어도 공격 범위라면 움직일 필요 x 또 만약 코루틴이 실행 중이라면 움직이지 않게
    //        if (isMeeleeRange || !isMoving) return;

    //        //범위 안에서 계속 실행되어야 하는 기능 - 쫓아오기(soldier) or 도망치기(civilian)
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
    //    //1.위치 파악 후 direction 설정
    //    detectPoint = col.transform.position;
    //    var curPoint = (Vector2)transform.position;
    //    var dir = curPoint.x - detectPoint.x > 0;
    //    runDir = dir ? new Vector2(1, 0) : new Vector2(-1, 0);

    //    //2. 거리가 일정 범위 내인지 확인 - meelee 인터랙션이 가능할 때
    //    if (Mathf.Abs(curPoint.x - detectPoint.x) < meeleeRange)
    //    {
    //        isMeeleeRange = true;
    //        //runDir = Vector2.zero;

    //        //코루틴을 딱 한번 실행하기 위한 조건문 - isMoving은 코루틴 실행 시 false, 종료 시 true가 되기 때문에
    //        //이미 코루틴이 실행되고 있다면 분기로 들어가지 않는다 
    //        if (isMoving)
    //        {
    //            StartCoroutine(EnemyBehaviour());
    //        }
    //        return;
    //    }
    //    else isMeeleeRange = false;

    //    transform.localScale = dir ? new Vector2(1,1) : new Vector2(-1,1);
    //}

    //아직 필요 x
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
    
    //단계가 있는 복잡한 적 움직임은 코루틴으로
    //IEnumerator EnemyBehaviour()
    //{
    //    Debug.Log("coroutine start");
    //    //공격 코루틴 실행중이면 ismoving false로 전환
    //    isMoving = false;

    //    //코루틴 실행중에는 다른 애니메이션으로 전환 x
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
    //            //애니메이션과 이동 맞추기 위해 시간 조정
    //            yield return new WaitForSeconds(.5f);
    //            var attack = (Vector2)gameObject.transform.position - (runDir)*0.5f;
    //            gameObject.transform.position = attack;
    //            Debug.Log("attack1");

    //            yield return new WaitForSeconds(1);

    //            //아직도 범위 안에 있으면 2타 실행
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
        //애니메이션 추가?
        yield return new WaitForSeconds(1f);

        isMoving = true;
    }
}
