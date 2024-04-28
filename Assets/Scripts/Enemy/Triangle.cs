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

    [SerializeField] protected Transform _target;
    [SerializeField] protected RayBox _ray;
    [SerializeField] float _jumpForce;
    
    [Header("Dash")]
    [SerializeField] float _dashForce;
    [SerializeField] float _dashDistance;

    protected bool _reachCliff;
    protected bool _onWall;

    // public enum EnemyType { Civilian, Soldier, Jumper }
    // public EnemyType enemyType;
    [SerializeField] GameObject _hitBox;
    protected Vector2 _moveDir;

    void Start()
    {
        _hp = _maxHp;
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _enemyState = EnemyState.Idle;
        _moveDir = Vector2.left;
        _reachCliff = false;
        _onWall = false;
        StartCoroutine(EnemyStateHandler());
    }

    //매 프레임마다 확인해야 할 것? bool 애니메이션은 애니메이션 함수로 따로 처리하기 파편화되어있으니 보기 싫다
    // Update is called once per frame
    void Update()
    {
        // [SH] [2024-04-16]
        // Triangle이 Civilian, Soldier, Jumper로 분리되었는데
        // Animator는 soldierRun과 civilRun이 분리되지 않고 있다
        // Animator도 Civilian, Soldier, Jumper로 분리하는 것이
        // 장기적으로 봤을 때 훨씬 깔끔할 것 같다
        TriangleAnim();

        // [SH] [2024-04-16] [Refactor]
        // _isMeeleeRange 업데이트 시점 변경
        // 기존에는 TargetDistance 함수 내부에서 업데이트 되었는데 변경 시점 예측이 어렵다
        // Meelee Range가 Detect Range보다 작다고 가정
        // Detect Box를 Meelee Range를 체크하는 데 재사용할 수 없기 때문에 Update 내부에서 상시 체크
        if (_target)
        {
            float dist = TargetDistance(_target);
            _isMeeleeRange = (dist < _meeleeRange);
        }
    }

    //triangle 기사는 가만히 있는 컨셉(근위병 같은 느낌?)
    
    // [SH] [2024-04-16] [Refactor]
    // base 함수를 활용할 수 있도록 리팩토링
    
    protected override IEnumerator Idle()   
    {
        yield return null;
    }

    protected override IEnumerator Detect() 
    {
        yield return null;

        //방향 설정
        CalculateDir();

        //state가 바뀌지 않았다면 이동
        _isMoving = true;
    }

    protected override IEnumerator Attack() 
    {
        yield return null; 
    }

    protected override IEnumerator Die()
    {
        yield return null;

        Destroy(gameObject);
    }

    protected virtual void TriangleAnim() 
    { 
        // isRun, isChase 애니메이션 파라미터를 isMoving으로 통합
        // isMoving은 Idle 애니메이션 전환을 위한 플래그
        // [SH] [2024-04-16] [Refactor]
        // 리팩토링 과정에서 _isMoving은 Idle 애니메이션 전환을 위한 플래그로 사용하기로 결정했다
        // 애니메이션 전환을 고려하지 않고 개념적으로 움직이지 않는 상태라고 해서 _isMoving 플래그를 바꾸게 되면
        // 의도치 않은 애니메이션 전환이 일어날 수 있다
        _anim.SetBool("isMoving", _isMoving);
        _anim.SetBool("isMeelee", _isMeeleeRange);
    }

    // [SH] [2024-04-16] [Refactor]
    // Move 함수가 존재하는데 활용되지 않고 있으므로 이를 활용하였음
    // Enemy Type이나 Enemy State에 따라 이동 방향이 다르므로 Move Direction을 파라미터로 받는다
    // 상황에 따라 속도가 달라지는 연출의 가능성을 생각해서 Move Speed도 파라미터로 추가하였다
    // 현재 Civilian의 경우 civilRun 애니메이션의 sprite가 오른쪽으로 되어 있는데
    // 해당 Move를 사용하기 위해서는 sprite 방향이 일관되어야 해서 왼쪽으로 모두 바꾸었다

    protected void Move(Vector2 dir, float speed)
    {   
        // [SH] [2024-04-16] [Fix]
        // 적과 수직선상에 놓여 있을 때 매우 빠르게 방향이 전환되는 것을 보정
        // Move가 수평 방향으로 일어난다고 가정한다
        // 일반적으로 dir.magnitude를 사용할 수 있다

        float x = dir.x;
        if (Mathf.Abs(x) < Mathf.Epsilon) 
        {
            _isMoving = false;
            return;
        }

        // [SH] [2024-04-16] [Fix]
        // 낭떠러지이거나 벽과 닿으면 Triangle의 이동을 중지하기 위한 로직
        // raycast는 현재 위치가 아닌 예측된 위치를 기반으로 이루어짐 
        
        Vector3 curPos = transform.position;
        Vector3 rayPos = curPos + new Vector3(Mathf.Sign(x), 0, 0);
        Vector3 newPos = curPos + new Vector3(x * speed * Time.deltaTime, 0, 0);

        _reachCliff = !_ray.CheckWithRay(rayPos, Vector2.down, 5f);
        _onWall     =  _ray.CheckWithRay(curPos, dir, 1f);
        
        if (_reachCliff || _onWall)
        {
            _isMoving = false;
            return;
        }

        // 이동 로직
        transform.position = newPos;    
        _isMoving = true;
        
        int k = (x < 0) ? +1 : -1;
        transform.localScale = new Vector2(k, 1);
    }

    protected void TurnAround()
    {
        _moveDir = -_moveDir;
        transform.localScale *= new Vector2(-1, 1);
    }

    protected IEnumerator Jump()
    {
        Vector2 jumpDir = new Vector2(chaseDir.x, 1);
        _rb.AddForce(jumpDir * _jumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
    }

    protected IEnumerator Dash(Vector2 vector, float _dashForce)
    {
        // [SH] Trail Renderer 써 봐도 괜찮을 것 같다
        // TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
        // trailRenderer.enabled = true;
        // trailRenderer.time = vector.magnitude / _dashForce;
        // ...
        // trailRenderer.enabled = false;

        if (_dashForce < Mathf.Epsilon) yield break;
        
        _rb.AddForce(vector.normalized * _dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(vector.magnitude / _dashForce);
        _rb.velocity = Vector2.zero;
    }

    // Attack시 SetTrigger("Attack") 시점 유의
    // Detect -> Attack1 과 Attack1 -> Attack2 애니메이션 전환 시 
    // 동일한 "Attack" 이름의 파라미터를 사용하고 있으므로 주의한다
    // 실수를 방지하려면 서로 다른 파라미터 이름을 사용하는 것이 깔끔하다

    protected IEnumerator AttackCombo()
    {
        yield return FirstAttack();
        
        //target이 null이 아니고 아직도 범위 안에 있으면 2타 실행
        if (_target && _isMeeleeRange)
        {
            CalculateDir();
            Move(chaseDir, _moveSpd);
            yield return SecondAttack();
        }
    }

    protected IEnumerator FirstAttack()
    {
        _anim.SetTrigger("Attack");

        //애니메이션과 이동 맞추기 위해 시간 조정
        yield return new WaitForSeconds(.2f);

        // 효과음 재생
        SoundManager.Instance.PlaySFX("First Attack");
        
        //공격할때 앞으로 돌진하는 모션
        yield return Dash(chaseDir * _dashDistance, _dashForce);

        yield return new WaitForSeconds(.3f);

        this.Log("attack1");
    }

    protected IEnumerator SecondAttack()
    {
        _anim.SetTrigger("Attack");

        //애니메이션과 이동 맞추기 위해 시간 조정
        yield return new WaitForSeconds(.25f);
        
        // 효과음 재생
        SoundManager.Instance.PlaySFX("Second Attack");

        //공격할때 앞으로 돌진하는 모션
        yield return Dash(chaseDir * _dashDistance * 0.5f, _dashForce * 0.5f);

        yield return new WaitForSeconds(0.3f);

        this.Log("attack2");
    }

    protected void CalculateDir()
    {
        if (!_target) return;
        //방향 설정
        _detectPoint = _target.position;
        Vector2 curPoint = (Vector2)transform.position;

        int dir;
        float deltaX = _detectPoint.x - curPoint.x;
        
        // [SH] [2024-04-16] [Fix]
        // 적과 수직선상에 놓여 있을 때 매우 빠르게 방향이 전환되는 것을 보정
        // 현재 runDir.x가 활용되고 있는 상황은 MeeleeRange 내에서만 있으므로
        // MeeleeRange 내에 있지 않은 경우 runDir 이 영벡터가 되어도 현재까지는 문제가 없다
        if (!_isMeeleeRange && Mathf.Abs(deltaX) < 0.1f) dir = 0;
        else dir = deltaX > 0 ? 1 : -1 ;

        chaseDir = new Vector2( dir, 0);
        runDir   = new Vector2(-dir, 0);
    }
   
    protected float TargetDistance(Transform target)
    {
        float dist = Vector2.Distance(transform.position, target.position);
        return dist;
    }

    // IDetectable
    //얘는 감지하는 놈 - DetectBox가 호출
    public void Detect(Transform target)
    {
        //target이 null이 아니면
        if (target != null)
        {
            //매개변수 받은 놈으로 초기화시켜주고
            this._target = target;

            //state 바꾸기
            StateChange(EnemyState.Detect);

            // CalculateDir 이 IEnumerator Detect 에서 수행되므로 방향 설정은 여기서 안 해도 됨
        }

        else
        {
            this.Log($"{this._target} Out of Sight");
            this._target = null;

            //state 바꾸기
            StateChange(EnemyState.Idle);
        }    
    }

    public void ByParry(Shield shield)
    {
        //패링하면 disarm 컨셉 - 시민으로 돌아감
        this.Log("Attacked by Parrying");
        //넉백 이펙트 EffectManager
        EffectManager.Instance.KnockBackEffect(this.transform, runDir, 1);
        //EnemyKnockBack(1.0f);
        //패링 이펙트 EffectManager
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
            _hitBox.SetActive(false);
            //enemyType = EnemyType.Civilian;
            EnemyManager.Instance.Replace(this, "Civilian");
        }
    }

    // IAttackable
    public void ByShield(Shield shield)
    {
        this.Log("Attacked by Shield");

        gameObject.GetComponent<Rigidbody2D>().AddForce(runDir * shield.weaponForce * (1 / level), ForceMode2D.Impulse);
        //shield.ShieldEffect();
    }

    // [TG] [2024-04-04] [feat]
    // 1. Triangle이 Player의 무기로 공격당했을 때
    // 2. blood 효과?
    public void ByWeapon(Weapon weapon)
    {
        this.Log("Attacked by Weapon");
        //attack.AttackEffect();
        gameObject.GetComponent<Rigidbody2D>().AddForce(runDir * weapon.weaponForce * (1 / level), ForceMode2D.Impulse);
    }

    // IDamageable
    public void Damaged(int dmg)
    {
        //여기서 분기처리?
        //코루틴 최상위에서 hp가 0이 되면 자동으로 Death 코루틴이 실행되도록 했으나, Attack 하위 코루틴이 실행될때는 코드 흐름 중지,
        //최상위 코루틴은 딜레이되어서 0.n초간 딜레이가 있음;
        //
        this.Log($"currentHp : {_hp} - {dmg} = {_hp - dmg}");
        _hp -= dmg;
        
        //blood effect?
        //dosth

        //Death effect는 enemy.Die()에서 처리
        //enemy.Die()에서 EnemyManager를 통해 Destroy가 진행된다
        if (_hp <= 0) { StateChange(EnemyState.Die); }
    }

}
