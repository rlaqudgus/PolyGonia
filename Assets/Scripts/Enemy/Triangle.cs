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
        StartCoroutine(EnemyStateHandler());
    }

    //매 프레임마다 확인해야 할 것? bool 애니메이션은 애니메이션 함수로 따로 처리하기 파편화되어있으니 보기 싫다
    // Update is called once per frame
    void Update()
    {
        TriangleAnim();
    }

    //triangle 기사는 가만히 있는 컨셉(근위병 같은 느낌?)
    protected override IEnumerator Idle()   { yield return null; }
    protected override IEnumerator Detect() { yield return null; }
    protected override IEnumerator Attack() { yield return null; }
    protected override IEnumerator Die()    { yield return null; }

    protected virtual void TriangleAnim()   { return; }

    protected void Move()
    {   
        Vector2 newPos = (Vector2)transform.position + (_moveDir * _moveSpd * Time.deltaTime);
        transform.position = newPos;

        transform.localScale = (_moveDir == Vector2.left) ? new Vector2(1, 1) : new Vector2(-1, 1);
    }

    protected void TurnAround()
    {
        _moveDir = -_moveDir;
        transform.localScale = new Vector2(-transform.localScale.x,transform.localScale.y);
    }

    protected IEnumerator Jump()
    {
        Vector2 jumpDir = new Vector2(-runDir.x, 1);
        _rb.AddForce(jumpDir * _jumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        StateChange(EnemyState.Detect);
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

    protected IEnumerator AttackCombo()
    {
        
        _anim.SetTrigger("Attack");

        //애니메이션과 이동 맞추기 위해 시간 조정
        yield return new WaitForSeconds(.2f);

        //공격할때 앞으로 돌진하는 모션
        yield return Dash(-runDir * _dashDistance, _dashForce);

        yield return new WaitForSeconds(.3f);
        
        //target이 null이 아니고 아직도 범위 안에 있으면 2타 실행
        if (_target && TargetDistance(_target) < _meeleeRange)
        {
            _anim.SetTrigger("Attack");

            //애니메이션과 이동 맞추기 위해 시간 조정
            yield return new WaitForSeconds(.25f);

            //공격할때 앞으로 돌진하는 모션
            yield return Dash(-runDir * _dashDistance * 0.5f, _dashForce * 0.5f);

            this.Log("attack2");
        }
        
        //공격패턴 끝나면 다시 감지
        StateChange(EnemyState.Detect);
    }

    protected void CalculateDir()
    {
        if (!_target) return;
        //방향 설정
        _detectPoint = _target.position;
        Vector2 curPoint = (Vector2)transform.position;
        bool dir = curPoint.x - _detectPoint.x > 0;
        runDir = dir ? new Vector2(1, 0) : new Vector2(-1, 0);

        transform.localScale = dir ? new Vector2(1, 1) : new Vector2(-1, 1);
    }
   
    protected float TargetDistance(Transform target)
    {
        float dis = Vector2.Distance(transform.position, target.position);
        _isMeeleeRange = dis < _meeleeRange;
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
            this._target = target;

            //방향 설정
            _detectPoint = target.position;
            Vector2 curPoint = (Vector2)transform.position;
            bool dir = curPoint.x - _detectPoint.x > 0;
            runDir = dir ? new Vector2(1, 0) : new Vector2(-1, 0);

            //state 바꾸기
            StateChange(EnemyState.Detect);
        }

        else
        {
            this.Log($"{this._target} Out of Sight");
            this._target = null;
            _isMeeleeRange = false;
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
            _hitBox.SetActive(false);
            //enemyType = EnemyType.Civilian;
            EnemyManager.Instance.Replace(this, "Civilian");
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
        this.Log($"currentHp : {_hp} - {dmg} = {_hp - dmg}");
        _hp -= dmg;
        
        //blood effect?
        //dosth

        //Death effect는 enemy.Die()에서 처리
        //enemy.Die()에서 EnemyManager를 통해 Destroy가 진행된다
        if (_hp <= 0) { StateChange(EnemyState.Die); }
    }

}
