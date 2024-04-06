using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utilities;


// PlayerInput의 Invoke Unity Events 사용
// Action에 미리 mapping 해놓은 키가 불렸을 때 Unity Events를 호출한다. 
public class PlayerController : MonoBehaviour,IDamageable, IAttackable
{
    
    // [TG] [2024-04-01] [Refactor]
    // 1. private 변수명 _camelCase로 변경
    // 2. 접근 제한자 private 명시
    // 3. 프로퍼티 이름 PascalCase로 변경
    // 4. 변수명 sr이 직관성이 떨어져 _spriteRenderer로 변경 (_rb는 관례에 따라 수정x)
    // 5. PlayerData를 따로 만들어서 Scriptable Object로 관리하는 것이 편해 보여서 일부분 사용해봄(좋으면 전부 수정)
    
    public PlayerData data;
    public CameraController cam;
    public enum AttackType { HorizontalAttack = 0, VerticalAttack, }
    
    
    // [TG] [2024-04-05] [feat]
    // 1. private 변수명 _camelCase로 변경
    [SerializeField] private RayBox _ray;
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCutMultiplier;
    [SerializeField] private float _invincibleTime;
    [SerializeField] private int _jumpCounter;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;

    private int _initJumpCounter;

    private Vector2 _moveDir;
    private int _dir;
    private bool _isMoving;
    private bool _canTeeter;
    public bool IsLookUp { get; private set; }
    public bool IsLookDown{ get; private set; }

    public bool isShield;
    private bool _isParry;
    public bool IsAttacking { get; private set; }
    
    public bool IsWallJumping { get; private set; }
    private bool _isJumping;
    private bool IsJumpingDown => _rb.velocity.y < 0;

    private bool _isInvincible; 

    private PlayerInput _playerInput;

    private Rigidbody2D _rb;

    private SpriteRenderer _spriteRenderer;

    private Animator _anim;

    private Shield _shield;

    private Attack _attack;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _shield = GetComponentInChildren<Shield>(true);
        _attack = GetComponentInChildren<Attack>();

        _initJumpCounter = _jumpCounter;
        _HP = _maxHP;
    }
    
    void Update()
    {
        Move();
        // [TG] [2024-04-06] [Refactor]
        // 1. 기존 Raybox의 trasnform.position이 (0, 0, 0), BoxCollider의 offset이 (0, -0.45)
        // 2. raybox의 왼쪽에서 아래로 raycast한 것과 오른쪽에서 raycast한 것에서 ground가 발견되지 않았을 경우 teetering 실행
        // 3. 기존 값을 확인하는 것은 비효율적이기 때문에 참조를 할 수 있게 수정이 필요해 보임
        // 4. 따로 함수로 구현?
        _canTeeter = !_isJumping && !_isMoving && !IsLookUp && !IsLookDown; // 흠
        if (_canTeeter)
        {
            if (!_ray.CheckWithRay(transform.position + new Vector3(-0.2f, -0.45f, 0), Vector2.down, 0.1f)
                || !_ray.CheckWithRay(transform.position + new Vector3(0.2f, -0.45f, 0), Vector2.down, 0.1f))
            {
                _anim.SetBool("isTeetering", true);
            }
            else
            {
                _anim.SetBool("isTeetering", false);
            }
        }
        else
        {
            _anim.SetBool("isTeetering", false);
        }
    }

    //Event를 통해 호출되는 함수
    //input에 따라 moveDir를 변경
    public void OnMove(InputAction.CallbackContext input)
    {
        Vector2 playerInput = input.ReadValue<Vector2>();

        if (playerInput.x > Mathf.Epsilon) _dir = 1;
        else if (playerInput.x < -Mathf.Epsilon) _dir = -1;
        else _dir = 0;
        
        _moveDir = Vector2.right * _dir;
        // this.Log($"Move Direction : {moveDir}");

        _isMoving = (_dir != 0);
        // this.Log($"isMoving : {isMoving}");
    }

    private void Move()
    {
        if (_isMoving)
        {
            //방어상태 이동 그냥 이동 차이
            float moveAmount = 1.0f;
            if (isShield) moveAmount *= 0.25f;
            if (_isParry) moveAmount *= 0.25f;
            
            // Move
            transform.Translate(_moveDir * _moveSpd * moveAmount * Time.deltaTime);
            
            //x축 부호 바꾸기 (좌우반전)
            transform.localScale = new Vector2(_dir, transform.localScale.y);
        }
        
        // Animation Transition
        _anim.SetBool("isMoving", _isMoving);
    }

    public void OnLook(InputAction.CallbackContext input)
    {
        Vector2 playerInput = input.ReadValue<Vector2>();

        IsLookUp = (playerInput.y > 0);
        IsLookDown = (playerInput.y < 0);

        //위나 아래를 보고 있을 때 움직이면 바로 움직이게
        //키를 뗄 때도 호출되기 때문에 0입력되고 false시켜줌
        if (_isMoving) 
        {
            IsLookUp = false;
            IsLookDown = false;
        }

        Look();
    }

    private void Look() 
    {
        _anim.SetBool("isLookUp", IsLookUp);
        _anim.SetBool("isLookDown", IsLookDown);
    }

    // Axis는 눌렀을 때 1, 뗐을 때 0으로 변하는 float return
    // 계속 누르고 있으면 계속 True를 반환하는 isShield 변수 - 이놈은 shield idle 애니메이션 할 때 쓰기
    // 한번 눌렀을 때 딱 한번 실행하는 애니메이션(방패 뽑는 애니메이션)을 써야하기 때문에 trigger 변수 하나 더 만들어주자 
    public void OnShield(InputAction.CallbackContext input)
    {
        isShield = input.ReadValueAsButton();
        this.Log($"isShield : {isShield}");

        Shield();
    }
    
    private void Shield()
    {
        if (isShield)
        {
            _anim.SetTrigger("Shield");
        }

        _shield.ShieldActivate(isShield);
        _anim.SetBool("isShield", isShield);

    }
    
    // [TG] [2024-04-04] [Refactor]
    // 1. OnAttack을 switch문으로 변경
    // 2. 수동 리셋 위치 옮김
    // 3. Attack Script 제작
    public void OnAttack(InputAction.CallbackContext input)
    {
        switch (input.phase)
        {
            case InputActionPhase.Performed:
                break;
            
            case InputActionPhase.Started:
                IsAttacking = true;
                Attack();
                break;
            
            case InputActionPhase.Canceled:
                IsAttacking = false;
                // 너무 빨리 뗐을 때 트리거가 reset되지 않음 수동으로 reset 해주자..
                _anim.ResetTrigger("Parry");
                break;
        }
    }

    // [TG] [2024-04-04] [Refactor]
    // 1. 윗 공격, 하단 공격, 좌우 공격 추가
    // 2. 상 하 공격은 디버깅을 위해 일단 합치지 않았음
    // 2. 마음에 안듬 => input system의 one modifier..?
    private void Attack() {
        // 눌렀을 때 shieldbox를 끄고 parrybox를 켠다
        if (isShield)
        {
            this.Log($"isParry: {_isParry}");
            _anim.SetTrigger("Parry");
            StartCoroutine(CheckParry());
            _shield.ShieldParry();
        }
        else if(IsLookUp && !IsLookDown)
        {
            this.Log("Up Attack");
            _anim.SetTrigger("Attack");
            _attack.DoAttack((int)AttackType.VerticalAttack);
        }
        else if(!IsLookUp && IsLookDown)
        {
            this.Log("Down Attack");
            _anim.SetTrigger("Attack");
            _attack.DoAttack((int)AttackType.VerticalAttack);
        }
        else
        {
            this.Log("Attack");
            _anim.SetTrigger("Attack");
            _attack.DoAttack((int)AttackType.HorizontalAttack);
        }
    }
    IEnumerator CheckParry()
    {
        _isParry = true;
        yield return new WaitForSeconds(.5f);
        _isParry = false;
    }

    // responsive 하게 만들고싶다
    // hold 하는 시간에 따라 점프 높이 달라지게 (Jump Cut)
    // coyote time은 가능하면?
    public void OnJump(InputAction.CallbackContext input)
    {
        this.Log("Jump executed");

        if (input.performed) Jump();
        if (input.canceled) JumpCut();
        
    }

    void Jump()
    {
        if (_jumpCounter <= 0) return;

        this.Log("Performed");
        _isJumping = true;
        _jumpCounter--;

        // Y velocity after adding force is the same as the initial jump velocity
        // It keeps the jump height whenever the jump key is pressed
        _rb.AddForce(Vector2.up * _rb.mass * (_jumpForce - _rb.velocity.y), ForceMode2D.Impulse);

        // This is the old jump model
        // jump height does not stay the same because the net force is changing
        // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void JumpCut()
    {
        // Adjustable jump height
        if (!_isJumping) return;
        if (IsJumpingDown) return;
        this.Log("Jump Cut");

        _rb.AddForce(Vector2.down * _rb.velocity.y * _rb.mass * (1 - _jumpCutMultiplier), ForceMode2D.Impulse);
    }
    
    public void PlayerKnockBack(float knockBackDist)
    {
        transform.position = (Vector2)transform.position - new Vector2(transform.localScale.x * knockBackDist,0);
    }

    //ㅈㄴ마음에안든다
    // [TG] [2024-04-05] [Question]
    // 1. OverlapBox을 쓰는 것도 괜찮아 보임
    // 2. 땅 위에 있을 때마다 coyoteTime할당
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            if (_ray.CheckWithBox()) 
            {
                _isJumping = false;
                _jumpCounter = _initJumpCounter;
                // PlayerOnGroundTime = data.coyoteTime;
                
            }
        }
    }
    // 3. Ground를 벗어났을 때 coyoteTime 감소 시작
    // 4. 코루틴 사용하여 coyoteTime동안은 isJumping이 True로 유지되게
    // private void OnCollisionExit2D(Collision2D col)
    // {
    //     if (col.gameObject.CompareTag("Ground"))
    //     {
    //         _isJumping = false;
    //         _jumpCounter = _initJumpCounter;
    //         PlayerOnGroundTime = data.coyoteTime;
    //     }
    // }
    
    public void Damaged(int dmg)
    {
        DamageEffect();
        this.Log($"currentHp : {_HP} - {dmg} = {_HP-=dmg}");
    }

    void DamageEffect()
    {
        StartCoroutine(InvincibleTimer());
        StartCoroutine(InvincibleEffect());
        StartCoroutine(cam.Shake());
        
    }

    IEnumerator InvincibleEffect()
    {
        //최상위 콜라이더 invincible로 변경
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        //하위 모든 오브젝트 invincible로 변경
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Invincible");
        }
        while (_isInvincible)
        {   
            yield return new WaitForSeconds(.2f);
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(.2f);
            GetComponent<SpriteRenderer>().enabled = true;
        }
        //하위 모든 오브젝트 
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Player1");
        }

    }

    IEnumerator InvincibleTimer()
    {
        float timer = 0;
        while (timer <= _invincibleTime) 
        {
            _isInvincible = true;
            
            timer += Time.deltaTime;
            yield return null;
        }
        _isInvincible = false;
        
    }

    // [TG] [2024-04-04] [question]
    // Player의 ByShield은 단순 transform.position 변경이지만 Triangle의 ByShield는 Addforce 사용 (?)
    // 나중에 AddForce로 통일하는 것이 좋아 보임
    public void ByShield(Shield shield)
    {
        PlayerKnockBack(0.5f);
    }

    public void ByParry(Shield shield)
    {
        throw new System.NotImplementedException();
    }

    public void ByWeapon(Attack attack)
    {
        throw new System.NotImplementedException();
    }
}
