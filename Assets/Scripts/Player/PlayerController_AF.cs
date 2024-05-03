using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Windows;
using Utilities;


// PlayerInput의 Invoke Unity Events 사용
// Action에 미리 mapping 해놓은 키가 불렸을 때 Unity Events를 호출한다.
// [TG] [2024-04-29]
// 기존 PlayerController에 움직임 관련 기능들을 모두 AddForce로 통일한 버전
// https://github.com/DawnosaurDev/platformer-movement/ 를 참고하였음
public class PlayerController_AF : MonoBehaviour, IAttackable
{   
    public PlayerData data; // player에 관한 변수들을 저장한 scriptable object
    [SerializeField] private RayBox _ray;
    [SerializeField] private float _invincibleTime;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;
    
    private int _dir;
    private bool _isMoving;
    private bool _canTeeter;
    public bool IsLookUp { get; private set; }
    public bool IsLookDown{ get; private set; }

    public bool isShield;
    private bool _isParry;
    public bool IsAttacking { get; private set; }
    private bool IsJumpingDown => _rb.velocity.y < 0; // get 프로퍼티

    public bool isInvincible; 

    private PlayerInput _playerInput;

    private Rigidbody2D _rb;

    private SpriteRenderer _spriteRenderer;

    private Animator _anim;

    private Shield _shield;

    #region PLAYER MOVE
    
    // Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;
    private int _jumpCounter;
    
    // Wall Jump
    private float _lastWallJumpTime;
    private int _lastWallJumpDir;

    // Dash
    private int _dashCounter;
    private bool _isDashCoolTime;
    private bool _isDashAttacking;
    
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsSliding { get; private set; }

    // Timers
    public float TimerOnGround { get; private set; }
    public float TimerOnWall { get; private set; }
    public float TimerOnWallRight { get; private set; }
    public float TimerOnWallLeft { get; private set; }
    
    #endregion
    
    #region INPUT PARAMETERS
    private Vector2 _moveInput;

    public float TimerPressJumpBtn { get; private set; }
    public float TimerPressDashBtn { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")] 
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _wallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.3f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _layerTerrain;
    #endregion
    
    void Start()
    {
	    
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _shield = GetComponentInChildren<Shield>(true);

	    SetGravityScale(data.gravityScale);
        _jumpCounter = data.jumpAmount;
        _HP = _maxHP;
    }
    
    void Update()
    {
	    // Timer가 0보다 큰 경우 타이머에 해당하는 상태가 활성화된 것
	    TimerOnGround -= Time.deltaTime;
	    TimerOnWall -= Time.deltaTime;
	    TimerOnWallRight -= Time.deltaTime;
	    TimerOnWallLeft -= Time.deltaTime;
	    TimerPressJumpBtn -= Time.deltaTime;
	    TimerPressDashBtn -= Time.deltaTime;
	    
	    #region COLLISION CHECKS
	    if (!IsDashing && !IsJumping)
	    {
		    // 기존의 OnCollision2D를 이용한 Ground Check은 실시간 모니터링을 하기 힘들어 Update함수에서 check를 함
		    // CheckWithbox의 BoxCastAll가 동적 함수라 Update내에서 쓰기에는 많은 리소스를 잡아먹을 것 같아 정적 검사인 OverlapBox사용
		    
		    // Ground Check
		    if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _layerTerrain) && !IsJumping)
		    {
			    TimerOnGround = data.coyoteTime;
		    }		

		    // Right Wall Check
		    if ((Physics2D.OverlapBox(_wallCheckPoint.position, _wallCheckSize, 0, _layerTerrain) && _dir == 1) && !IsWallJumping)
		    {
			    TimerOnWallRight = data.coyoteTime;
		    }

		    // Left Wall Check
		    if ((Physics2D.OverlapBox(_wallCheckPoint.position, _wallCheckSize, 0, _layerTerrain) && _dir == -1) && !IsWallJumping)
		    {
			    TimerOnWallLeft = data.coyoteTime;
		    }

		    //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
		    TimerOnWall = Mathf.Max(TimerOnWallLeft, TimerOnWallRight);
	    }
	    #endregion
        
        #region TEETER CHECK
        if (CanTeeter())
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
        #endregion
        
        #region JUMP CHECK
		if (IsJumping && _rb.velocity.y < 0)
		{
			IsJumping = false;

			if (!IsWallJumping)
			{
				_isJumpFalling = true; // jump, wall jump 상태가 아니면서 플레이어의 y속력이 음수인 경우 떨어지는 중
			}
		}

		if (IsWallJumping && Time.time - _lastWallJumpTime > data.wallJumpTime) // wall jump 상태에서 wallJumpTime을 초과한 경우
		{
			IsWallJumping = false;
		}

		if (TimerOnGround > 0 && !IsJumping && !IsWallJumping) // Ground에 있을 경우
        {
			_isJumpCut = false;
			_isJumpFalling = false;
        }
		
		if (!IsDashing)
		{
			//Jump
			if (CanJump() && TimerPressJumpBtn > 0)
			{
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();
			}
			//WALL JUMP
			else if (CanWallJump() && TimerPressJumpBtn > 0)
			{
				IsWallJumping = true;
				IsJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;

				_lastWallJumpTime = Time.time;
				_lastWallJumpDir = (TimerOnWallRight > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
		}
		#endregion
		
		#region GRAVITY
		if (!_isDashAttacking)
		{
			if (IsSliding)
			{
				SetGravityScale(0);
			}
			else if (_rb.velocity.y < 0 && _moveInput.y < 0)
			{
				// 아래 방향키 누르고 있을 시 빠른 하강
				SetGravityScale(data.gravityScale * data.fastFallGravityMult);
				_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				// 점프를 중간에 취소할 시 빠른 하강 : 중력값을 그대로 사용할 시 점프컷이 아닌 일반 점프에 점프 높이만 낮아진 것이 되어버림
				this.Log("jump cut");
				SetGravityScale(data.gravityScale * data.jumpCutGravityMult);
				_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -data.maxFallSpeed));
			}
			else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) < data.jumpHangThreshold)
			{
				// Jump후 Apex에 가까워졌을 때 Gravity값 작게 조정
				SetGravityScale(data.gravityScale * data.jumpHangGravityMult);
			}
			else if (_rb.velocity.y < 0)
			{
				// jumpFalling 시 중력값 더 크게
				SetGravityScale(data.gravityScale * data.fallGravityMult);
				_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -data.maxFallSpeed));
			}
			else
			{
				//Default gravity if standing on a platform or moving upwards
				SetGravityScale(data.gravityScale);
			}
		}
		else
		{
			SetGravityScale(0);
		}
		#endregion
		
		#region SLIDE CHECKS
		// 왼쪽 벽에 붙어있고 왼쪽 방향키 누를 때, 오른쪽 벽에 붙어있고 오른쪽 방향키 누를 때
		if (CanSlide() && ((TimerOnWallLeft > 0 && _moveInput.x < 0) || (TimerOnWallRight > 0 && _moveInput.x > 0)))
		{
			IsSliding = true;
		}
		else
		{
			IsSliding = false;
		}
		#endregion
		
		//x축 부호 바꾸기 (좌우반전)
		if (_isMoving)
		{
			transform.localScale = new Vector2(_dir, transform.localScale.y);
		}
		
		_anim.SetBool("isMoving", _isMoving);
    }
    
    private void FixedUpdate()
    {
	    #region Run
	    if (!IsDashing && IsWallJumping)
	    {
		    Run(data.wallJumpRunLerp);
	    }
	    else if (!IsDashing && !IsWallJumping)
	    {
		    Run(1);
	    }
	    else if (_isDashAttacking)
	    {
		    Run(data.dashEndRunLerp);
	    }
	    else if (isShield || _isParry)
	    {
		    Run(0.25f);
	    }
	    #endregion
	    // Slide
	    if (IsSliding)
	    {
		    Slide();
	    }
    }
    
    //Event를 통해 호출되는 함수
    //input에 따라 _moveDir를 변경
    public void OnMove(InputAction.CallbackContext input)
    {
        _moveInput = input.ReadValue<Vector2>();

        if (_moveInput.x > Mathf.Epsilon) _dir = 1;
        else if (_moveInput.x < -Mathf.Epsilon) _dir = -1;
        else _dir = 0;

        _isMoving = (_dir != 0);
        // this.Log($"_isMoving : {_isMoving}");
        
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
        if (isShield) _anim.SetTrigger("Shield");

        WeaponController.Instance.UseShield();
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
        int idx = IsLookUp ? 0 : IsLookDown ? 2 : 1; //LookUP이 true면 0, lookDown이 true면 2 다 아니면 1 위에서 아래 순

        // 눌렀을 때 shieldbox를 끄고 parrybox를 켠다
        if (isShield)
        {
            this.Log($"isParry: {_isParry}");
            _anim.SetTrigger("Parry");
            StartCoroutine(CheckParry());
        }

        WeaponController.Instance.UseWeapon(idx);
    }

    IEnumerator CheckParry()
    {
        _isParry = true;
        yield return new WaitForSeconds(.5f);
        _isParry = false;
    }

    #region JUMP METHODS
    public void OnJump(InputAction.CallbackContext input)
    {
	    if (input.phase == InputActionPhase.Started)
	    {
		    OnJumpInput();
	    }
        if (input.phase == InputActionPhase.Canceled)
        {
		   
		    OnJumpUpInput();
        }
    }
    
    void Jump()
    {
	    //Ensures we can't call Jump multiple times from one press
	    TimerPressJumpBtn = 0;
	    TimerOnGround = 0;
	 
	    this.Log("Perform Jump");
	    
	    // 현재 속도에 상관없이 항상 일정한 점프를 할 수 있도록 함
	    float force = data.jumpForce;
	    if (_rb.velocity.y < 0) force -= _rb.velocity.y;

	    _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }
    
    private void WallJump(int dir)
    {
	    TimerPressJumpBtn = 0;
	    TimerOnGround = 0;
	    TimerOnWallRight = 0;
	    TimerOnWallLeft = 0;
		
	    this.Log("Perform Wall jump");
	    
	    Vector2 force = new Vector2(data.wallJumpForce.x, data.wallJumpForce.y);
	    force.x *= dir; // 벽과 반대방향으로 wall jump

	    if (Mathf.Sign(_rb.velocity.x) != Mathf.Sign(force.x))
	    {
		    force.x -= _rb.velocity.x;
	    }

	    if (_rb.velocity.y < 0) // 항상 일정한 wall jump
	    {
		    force.y -= _rb.velocity.y;
	    }

	    _rb.AddForce(force, ForceMode2D.Impulse);
    }
    #endregion
    
    public void OnDash(InputAction.CallbackContext input)
    {
	    IsDashing = input.ReadValueAsButton();
	    OnDashInput();
	    
	    if (CanDash() && TimerPressDashBtn > 0)
	    {
		    // Dash Button을 누른 직후 다른 Input이 들어오는 것을 일정 시간동안 방지
		    Sleep(data.dashSleepTime); 
			
		    IsDashing = true;
		    IsJumping = false;
		    IsWallJumping = false;
		    _isJumpCut = false;

		    StartCoroutine(nameof(StartDash), _dir);
	    }
    }
    
    #region Pause / Resume

    // [SH] [2024-04-14]
    // OnPause는 Player Action Map에 할당
    // OnResume은 UI Action Map에 할당
    // Pause 시 Action Map을 UI로 전환시켜서 Pause 상태에서 Player Input이 작동하지 못하도록 만든다

    public void OnPause(InputAction.CallbackContext input)
    {
        bool isPaused = PauseManager.Instance.isPaused;
        if (!isPaused) Pause();
    }

    private void Pause()
    {
        PauseManager.Instance.Pause();
    }

    public void OnResume(InputAction.CallbackContext input)
    {
        bool isPaused = PauseManager.Instance.isPaused;
        if (isPaused) Resume();

    }

    public void Resume()
    {
        PauseManager.Instance.Resume();
    }

    #endregion
    
    public void PlayerKnockBack(float knockBackDist)
    {
        transform.position = (Vector2)transform.position - new Vector2(transform.localScale.x * knockBackDist,0);
    }
    
    public void Damaged(int dmg)
    {
        DamageEffect();
        this.Log($"currentHp : {_HP} - {dmg} = {_HP - dmg}");
        _HP -= dmg;
    }

    void DamageEffect()
    {

        Timer.Instance.StartTimer(2f, () => isInvincible = true, () => isInvincible = false);

        EffectManager.Instance.InvincibleEffect();

        EffectManager.Instance.InstantiateEffect(ParticleColor.RED, transform.position + Vector3.up);

        EffectManager.Instance.TimeStop(.2f);

        CameraManager.Instance.Shake();
        
        JoyConManager.Instance?.j[0].SetRumble(160, 320, 1f, 400);
        
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

    public void ByWeapon(Weapon weapon)
    {
        //throw new System.NotImplementedException();
    }

    void SceneTest()
    {
        //대충 씬 전환 확인하는코드
        SceneManager.LoadScene(0);
    }

    #region JoyCon Functions

    public void J_OnMove(Vector2 a)
    {
        Vector2 playerInput = a;

        if (playerInput.x > Mathf.Epsilon) _dir = 1;
        else if (playerInput.x < -Mathf.Epsilon) _dir = -1;
        else _dir = 0;

        _isMoving = (_dir != 0);
        // this.Log($"_isMoving : {_isMoving}");

    }

    public void J_OnLook(Vector2 a)
    {
        Vector2 playerInput = a;

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

    public void J_OnShield(bool s)
    {
        //조이콘에서 받은 인풋 사용
        isShield = s;
        this.Log($"isShield : {isShield}");
        //_shield.ShieldActivate(_isShield);
        _anim.SetBool("isShield", isShield);
    }

    //이건좀ㅋㅋ
    public void J_ShieldTrigger(bool s)
    {
        if (!s) return;
        this.Log("isShieldtrigger");
        _anim.SetTrigger("Shield");
        Invoke("ResetShield", .2f);
    }

    void ResetShield()
    {
        _anim.ResetTrigger("Shield");
    }

    public void J_OnParry()
    {
        this.Log("Parry");
        _anim.SetTrigger("Parry");
        StartCoroutine(CheckParry());
        //_shield.ShieldParry();
        Invoke("ResetParry", 0.2f);
    }

    void ResetParry()
    {
        _anim.ResetTrigger("Parry");
    }

    #endregion

    #region INPUT CALLBACKS
    public void OnJumpInput()
	{
		TimerPressJumpBtn = data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
		{
			_isJumpCut = true;
		}
	}

	public void OnDashInput()
	{
		TimerPressDashBtn = data.dashInputBufferTime;
	}
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
	{
		_rb.gravityScale = scale;
	}

	private void Sleep(float duration)
    {
	    // duration만큼 게임의 시간을 멈춤
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); // Time.timeScale에 영향을 받지 않기 위해 Realtime 사용
		Time.timeScale = 1;
	}
    #endregion
    
    #region RUN METHODS
    private void Run(float lerpAmount)
	{
		// 이동 Input을 주었을 때의 목표 속도
		float targetSpeed = _moveInput.x * data.runMaxSpeed;
		targetSpeed = Mathf.Lerp(_rb.velocity.x, targetSpeed, lerpAmount);

		// 가속도 계산
		float accelRate = 0f;
		if (TimerOnGround > 0)
		{
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;
		}
		else
		{
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDeccelAmount * data.deccelInAir;
		}

		// Jump Apex 근처에 있을 때 가속도 및 최대 속도 증가
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) < data.jumpHangThreshold)
		{
			accelRate *= data.jumpHangAccelerationMult;
			targetSpeed *= data.jumpHangMaxSpeedMult;
		}
		
		float speedDif = targetSpeed - _rb.velocity.x;
		float moveForce = speedDif * accelRate;

		this.Log(moveForce);
		// 지속적인 힘을 가하기 위해 ForceMode2D.Force 사용 - 물체 질량이 작을 수록 큰 가속도를 받음
		_rb.AddForce(moveForce * Vector2.right, ForceMode2D.Force);

	}
    #endregion
    
	#region DASH METHODS
	//Dash Coroutine
	private IEnumerator StartDash(Vector2 dir)
	{
		TimerOnGround = 0;
		TimerPressDashBtn = 0;

		float startTime = Time.time;

		_dashCounter--;
		_isDashAttacking = true;

		SetGravityScale(0);

		//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
		while (Time.time - startTime <= data.dashAttackTime)
		{
			_rb.velocity = dir.normalized * data.dashSpeed;
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
		SetGravityScale(data.gravityScale);
		_rb.velocity = data.dashEndSpeed * dir.normalized;

		while (Time.time - startTime <= data.dashEndTime)
		{
			yield return null;
		}

		IsDashing = false;
	}

	private IEnumerator RefillDash(int amount)
	{
		_isDashCoolTime = true;
		yield return new WaitForSeconds(data.dashCoolTime);
		_isDashCoolTime = false;
		_dashCounter = Mathf.Min(data.dashAmount, _dashCounter + 1);
	}
	#endregion

	#region SLIDE
	private void Slide()
	{
		float speedDif = data.slideSpeed - _rb.velocity.y;	
		float moveForce = speedDif * data.slideAccel;
		moveForce = Mathf.Clamp(moveForce, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		_rb.AddForce(moveForce * Vector2.up);
	}
    #endregion
    
    #region CHECK METHODS

    private bool CanJump()
    {
		return TimerOnGround > 0 && _jumpCounter > 0;
    }

	private bool CanWallJump()
    {
	    // Wall Jump가 가능한 환경
	    // 1. Jump Button을 누름
	    // 2. Wall과 닿아있으며 Ground에 닿고 있지 않음
	    // 3. Wall Jump를 하고 있지 않음
	    // 4. 
		return TimerPressJumpBtn > 0 && TimerOnWall > 0 && TimerOnGround <= 0 && (!IsWallJumping ||
			 (TimerOnWallRight > 0 && _lastWallJumpDir == 1) || (TimerOnWallLeft > 0 && _lastWallJumpDir == -1));
	}

	private bool CanJumpCut()
    {
	    this.Log(IsJumping.ToString());
		return IsJumping && _rb.velocity.y > 0;
    }

	private bool CanWallJumpCut()
	{
		return IsWallJumping && _rb.velocity.y > 0;
	}

	private bool CanDash()
	{
		// Dash가 가능한 환경
		// 1. Dash를 하고 있지 않으며 남은 Dash Amount가 1 이상
		// 2. Ground에 닿고 있으며 Dash의 CoolTime이 충전이 끝난 상태
		if (!IsDashing && _dashCounter < data.dashAmount && TimerOnGround > 0 && !_isDashCoolTime)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashCounter > 0;
	}

	public bool CanSlide()
    {
	    // Slide가 가능한 환경
	    // 1. 벽에 붙어 있으며 Jump 또는 Wall Jump를 하고 있지 않음
	    // 2. Dash를 하고 있지 않으며 Ground에 닿고 있지 않음
	    if (TimerOnWall > 0 && !IsJumping && !IsWallJumping && !IsDashing && TimerOnGround <= 0)
	    {
		    return true;
	    }
		else return false;
	}

	public bool CanTeeter()
	{
		if (!IsJumping && !_isMoving && !IsLookUp && !IsLookDown)
		{
			return true;
		}
		else return false;
	}
    #endregion
}
