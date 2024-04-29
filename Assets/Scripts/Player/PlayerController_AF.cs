using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using Utilities;


// PlayerInput의 Invoke Unity Events 사용
// Action에 미리 mapping 해놓은 키가 불렸을 때 Unity Events를 호출한다.
// [TG] [2024-04-29]
// 기존 PlayerController에 움직임 관련 기능들을 모두 AddForce로 통일한 버전
public class PlayerController_AF : MonoBehaviour, IAttackable
{   
    public PlayerData data; // player에 관한 변수들을 저장한 scriptable object
    [SerializeField] private RayBox _ray;
    [SerializeField] private float _moveSpd;
    [SerializeField] private float _invincibleTime;
    [SerializeField] private int _jumpCounter;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;

    private int _initJumpCounter;

    private Vector2 _moveDir;
    private int _dir;
    private bool _isMoving;
    private bool _canTeeter;
    public bool _isLookUp { get; private set; }
    public bool _isLookDown{ get; private set; }

    public bool _isShield;
    private bool _isParry;
    public bool _isAttacking { get; private set; }
    
    private bool _isJumping;
    private bool _isJumpingDown => _rb.velocity.y < 0;

    public bool _isInvincible; 

    private PlayerInput _playerInput;

    private Rigidbody2D _rb;

    private SpriteRenderer _spriteRenderer;

    private Animator _anim;

    private Shield _shield;

    // [TG] [2024-04-29]
    // 플레이어 이동, 대쉬, 점프, 벽점프 관련 변수들 추가
    #region PLAYER MOVE
    
    // Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    // Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    // Dash
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;
    
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsSliding { get; private set; }

    // Timers
    public float TimeAfterOnGround { get; private set; }
    public float TimeAfterOnWall { get; private set; }
    public float TimeAfterOnWallRight { get; private set; }
    public float TimeAfterOnWallLeft { get; private set; }
    
    #endregion
    
    #region INPUT PARAMETERS
    private Vector2 _moveInput;

    public float TimeAfterPressJumpBtn { get; private set; }
    public float TimeAfterPressDashBtn { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")] 
    [SerializeField] private Transform _groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion
    
    void Start()
    {
	    // [TG] [2024-04-29]
	    SetGravityScale(data.gravityScale);
	    IsFacingRight = true;
	    
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _shield = GetComponentInChildren<Shield>(true);

        _initJumpCounter = _jumpCounter;
        _HP = _maxHP;
    }
    
    void Update()
    {
	    // [TG] [2024-04-29]
	    #region TIMERS
	    TimeAfterOnGround -= Time.deltaTime;
	    TimeAfterOnWall -= Time.deltaTime;
	    TimeAfterOnWallRight -= Time.deltaTime;
	    TimeAfterOnWallLeft -= Time.deltaTime;
	    TimeAfterPressJumpBtn -= Time.deltaTime;
	    TimeAfterPressDashBtn -= Time.deltaTime;
	    #endregion
	    
	    #region COLLISION CHECKS
	    if (!IsDashing && !IsJumping)
	    {
		    //Ground Check
		    if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping)//checks if set box overlaps with ground
		    {
			    TimeAfterOnGround = data.coyoteTime; //if so sets the lastGrounded to coyoteTime
		    }		

		    //Right Wall Check
		    if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
		         || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
			    TimeAfterOnWallRight = data.coyoteTime;

		    //Right Wall Check
		    if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
		         || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
			    TimeAfterOnWallLeft = data.coyoteTime;

		    //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
		    TimeAfterOnWall = Mathf.Max(TimeAfterOnWallLeft, TimeAfterOnWallRight);
	    }
	    #endregion
	    
        Move();
        if (!_ray.CheckWithBox())
        {
            coyoteTime -= Time.deltaTime;
        }
        else
        {
            coyoteTime = _coyoteTimeDuration;
        }

        _canTeeter = !_isJumping && !_isMoving && !_isLookUp && !_isLookDown; // 흠
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
    //input에 따라 _moveDir를 변경
    public void OnMove(InputAction.CallbackContext input)
    {
        Vector2 playerInput = input.ReadValue<Vector2>();

        if (playerInput.x > Mathf.Epsilon) _dir = 1;
        else if (playerInput.x < -Mathf.Epsilon) _dir = -1;
        else _dir = 0;
        
        _moveDir = Vector2.right * _dir;
        // this.Log($"Move Direction : {_moveDir}");

        _isMoving = (_dir != 0);
        // this.Log($"_isMoving : {_isMoving}");
    }

    

    private void Move()
    {
        if (_isMoving)
        {
            //방어상태 이동 그냥 이동 차이
            float moveAmount = 1.0f;
            if (_isShield) moveAmount *= 0.25f;
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

        _isLookUp = (playerInput.y > 0);
        _isLookDown = (playerInput.y < 0);

        //위나 아래를 보고 있을 때 움직이면 바로 움직이게
        //키를 뗄 때도 호출되기 때문에 0입력되고 false시켜줌
        if (_isMoving) 
        {
            _isLookUp = false;
            _isLookDown = false;
        }

        Look();
    }

    private void Look() 
    {
        _anim.SetBool("isLookUp", _isLookUp);
        _anim.SetBool("isLookDown", _isLookDown);
    }

    // Axis는 눌렀을 때 1, 뗐을 때 0으로 변하는 float return
    // 계속 누르고 있으면 계속 True를 반환하는 isShield 변수 - 이놈은 shield idle 애니메이션 할 때 쓰기
    // 한번 눌렀을 때 딱 한번 실행하는 애니메이션(방패 뽑는 애니메이션)을 써야하기 때문에 trigger 변수 하나 더 만들어주자 
    public void OnShield(InputAction.CallbackContext input)
    {
        _isShield = input.ReadValueAsButton();
        this.Log($"isShield : {_isShield}");

        Shield();
    }
    
    private void Shield()
    {
        if (_isShield) _anim.SetTrigger("Shield");

        WeaponController.Instance.UseShield();
        _anim.SetBool("isShield", _isShield);

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
                _isAttacking = true;
                Attack();
                break;
            
            case InputActionPhase.Canceled:
                _isAttacking = false;
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
        int idx = _isLookUp ? 0 : _isLookDown ? 2 : 1; //LookUP이 true면 0, lookDown이 true면 2 다 아니면 1 위에서 아래 순

        // 눌렀을 때 shieldbox를 끄고 parrybox를 켠다
        if (_isShield)
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

    // responsive 하게 만들고싶다
    // hold 하는 시간에 따라 점프 높이 달라지게 (Jump Cut)
    // coyote time은 가능하면?
    public void OnJump(InputAction.CallbackContext input)
    {
        this.Log("Jump executed");
        //SceneTest();
        if (input.performed) Jump();
        if (input.canceled) JumpCut();
        
    }

    void Jump()
    {

        if (_jumpCounter <= 0 || coyoteTime <= 0) return;

        this.Log("Performed");
        _isJumping = true;
        if (coyoteTime > 0)
        {
            _jumpCounter = Mathf.Max(_jumpCounter, 1); // 최소한 한 번의 점프는 보장
        }
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
        if (_isJumpingDown) return;
        this.Log("Jump Cut");

        _rb.AddForce(Vector2.down * _rb.velocity.y * _rb.mass * (1 - _jumpCutMultiplier), ForceMode2D.Impulse);
    }
	
    public void OnDash(InputAction.CallbackContext input)
    {
	    IsDashing = input.ReadValueAsButton();
	    
	    Jump();
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

    //ㅈㄴ마음에안든다
    // [TG] [2024-04-05] [Question]
    // 1. OverlapBox을 쓰는 것도 괜찮아 보임
    // 2. 땅 위에 있을 때마다 coyoteTime할당
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            coyoteTime = _coyoteTimeDuration;
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
        this.Log($"currentHp : {_HP} - {dmg} = {_HP - dmg}");
        _HP -= dmg;
    }

    void DamageEffect()
    {

        Timer.Instance.StartTimer(2f, () => _isInvincible = true, () => _isInvincible = false);

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

        _moveDir = Vector2.right * _dir;
        // this.Log($"Move Direction : {_moveDir}");

        _isMoving = (_dir != 0);
        // this.Log($"_isMoving : {_isMoving}");

    }

    public void J_OnLook(Vector2 a)
    {
        Vector2 playerInput = a;

        _isLookUp = (playerInput.y > 0);
        _isLookDown = (playerInput.y < 0);

        //위나 아래를 보고 있을 때 움직이면 바로 움직이게
        //키를 뗄 때도 호출되기 때문에 0입력되고 false시켜줌
        if (_isMoving)
        {
            _isLookUp = false;
            _isLookDown = false;
        }

        Look();
    }

    public void J_OnShield(bool s)
    {
        //조이콘에서 받은 인풋 사용
        _isShield = s;
        this.Log($"isShield : {_isShield}");
        //_shield.ShieldActivate(_isShield);
        _anim.SetBool("isShield", _isShield);
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

    
	private void Update()
	{
		#region INPUT HANDLER
		_moveInput.x = Input.GetAxisRaw("Horizontal");
		_moveInput.y = Input.GetAxisRaw("Vertical");

		if (_moveInput.x != 0)
			CheckDirectionToFace(_moveInput.x > 0);

		if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
			OnJumpInput();
        }

		if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J))
		{
			OnJumpUpInput();
		}

		if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K))
		{
			OnDashInput();
		}
		#endregion
		//Jump
		if (CanJump() && TimeAfterPressJumpBtn > 0)
		{
			IsJumping = true;
			IsWallJumping = false;
			_isJumpCut = false;
			_isJumpFalling = false;
			Jump();
		}
		//WALL JUMP
		else if (CanWallJump() && TimeAfterPressJumpBtn > 0)
		{
			IsWallJumping = true;
			IsJumping = false;
			_isJumpCut = false;
			_isJumpFalling = false;
			_wallJumpStartTime = Time.time;
			_lastWallJumpDir = (TimeAfterOnWallRight > 0) ? -1 : 1;
			
			WallJump(_lastWallJumpDir);
		}
		
		#region JUMP CHECKS
		if (IsJumping && _rb.velocity.y < 0)
		{
			IsJumping = false;

			if(!IsWallJumping) _isJumpFalling = true; // 점프, 벽점프 중이 아니면서 플레이어의 y속도가 음수인 경우 떨어지는 중
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > data.wallJumpTime)
		{
			IsWallJumping = false;
		}

		if (TimeAfterOnGround > 0 && !IsJumping && !IsWallJumping)
        {
			_isJumpCut = false;

			if(!IsJumping) _isJumpFalling = false;
		}

		if (!IsDashing)
		{
			//Jump
			if (CanJump() && TimeAfterPressJumpBtn > 0)
			{
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();
			}
			//WALL JUMP
			else if (CanWallJump() && TimeAfterPressJumpBtn > 0)
			{
				IsWallJumping = true;
				IsJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;

				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = (TimeAfterOnWallRight > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
		}
		#endregion

		#region DASH CHECKS
		if (CanDash() && TimeAfterPressDashBtn > 0)
		{
			//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
			Sleep(data.dashSleepTime); 

			//If not direction pressed, dash forward
			if (_moveInput != Vector2.zero)
				_lastDashDir = _moveInput;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}
		#endregion

		#region SLIDE CHECKS
		if (CanSlide() && ((TimeAfterOnWallLeft > 0 && _moveInput.x < 0) || (TimeAfterOnWallRight > 0 && _moveInput.x > 0)))
			IsSliding = true;
		else
			IsSliding = false;
		#endregion

		#region GRAVITY
		if (!_isDashAttacking)
		{
			//Higher gravity if we've released the jump input or are falling
			if (IsSliding)
			{
				SetGravityScale(0);
			}
			else if (_rb.velocity.y < 0 && _moveInput.y < 0)
			{
				// 아래 방향키 누르고 있을 시 빠른 하강
				SetGravityScale(data.gravityScale * data.fastFallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				// 점프를 중간에 취소할 시 빠른 하강 : 중력값을 그대로 사용할 시 점프컷이 아닌 일반 점프에 점프 높이만 낮아진 것이 되어버림
				SetGravityScale(data.gravityScale * data.jumpCutGravityMult);
				_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -data.maxFallSpeed));
			}
			else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) < data.jumpHangTimeThreshold)
			{
				SetGravityScale(data.gravityScale * data.jumpHangGravityMult);
			}
			else if (_rb.velocity.y < 0)
			{
				//Higher gravity if falling
				SetGravityScale(data.gravityScale * data.fallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
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
			//No gravity when dashing (returns to normal once initial dashAttack phase over)
			SetGravityScale(0);
		}
		#endregion
    }

    private void FixedUpdate()
	{
		//Handle Run
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run(data.wallJumpRunLerp);
			else
				Run(1);
		}
		else if (_isDashAttacking)
		{
			Run(data.dashEndRunLerp);
		}

		//Handle Slide
		if (IsSliding)
			Slide();
    }

    #region INPUT CALLBACKS
	//Methods which whandle input detected in Update()
    public void OnJumpInput()
	{
		TimeAfterPressJumpBtn = data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput()
	{
		TimeAfterPressDashBtn = data.dashInputBufferTime;
	}
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
	{
		_rb.gravityScale = scale;
	}

	private void Sleep(float duration)
    {
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
	}
    #endregion

	//MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput.x * data.runMaxSpeed;
		//We can reduce are control using Lerp() this smooths changes to are direction and speed
		targetSpeed = Mathf.Lerp(_rb.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (TimeAfterOnGround > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDeccelAmount * data.deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) < data.jumpHangTimeThreshold)
		{
			accelRate *= data.jumpHangAccelerationMult;
			targetSpeed *= data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if(data.doConserveMomentum && Mathf.Abs(_rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && TimeAfterOnGround < 0)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0; 
		}
		#endregion

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - _rb.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		_rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

		/*
		 * For those interested here is what AddForce() will do
		 * _rb.velocity = new Vector2(_rb.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / _rb.mass, _rb.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
	}

	private void Turn()
	{
		//stores scale and flips the player along the x axis, 
		Vector3 scale = transform.localScale; 
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
    #endregion

    #region JUMP METHODS
    private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		TimeAfterPressJumpBtn = 0;
		TimeAfterOnGround = 0;

		// 하강 시 force값을 증가시킴 
		//We increase the force applied if we are falling
		// 현재 속도에 상관없이 항상 일정한 점프를 할 수 있도록 함
		float force = data.jumpForce;
		if (_rb.velocity.y < 0) force -= _rb.velocity.y;

		_rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
	}

	private void WallJump(int dir)
	{
		TimeAfterPressJumpBtn = 0;
		TimeAfterOnGround = 0;
		TimeAfterOnWallRight = 0;
		TimeAfterOnWallLeft = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2(data.wallJumpForce.x, data.wallJumpForce.y);
		force.x *= dir; //apply force in opposite direction of wall

		if (Mathf.Sign(_rb.velocity.x) != Mathf.Sign(force.x))
			force.x -= _rb.velocity.x;

		if (_rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y
                          //(counteracting force of gravity). This ensures the player always reaches our desired jump
                          //force or greater
			force.y -= _rb.velocity.y;

		//Unlike in the run we want to use the Impulse mode.
		//The default mode will apply are force instantly ignoring masss
		_rb.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region DASH METHODS
	//Dash Coroutine
	private IEnumerator StartDash(Vector2 dir)
	{
		//Overall this method of dashing aims to mimic Celeste, if you're looking for
		// a more physics-based approach try a method similar to that used in the jump

		TimeAfterOnGround = 0;
		TimeAfterPressDashBtn = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;

		SetGravityScale(0);

		//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
		while (Time.time - startTime <= data.dashAttackTime)
		{
			_rb.velocity = dir.normalized * data.dashSpeed;
			//Pauses the loop until the next frame, creating something of a Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
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

		//Dash over
		IsDashing = false;
	}

	//Short period before the player is able to dash again
	private IEnumerator RefillDash(int amount)
	{
		//SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
		_dashRefilling = true;
		yield return new WaitForSeconds(data.dashCoolTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(data.dashAmount, _dashesLeft + 1);
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide()
	{
		//Works the same as the Run but only in the y-axis
		//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
		float speedDif = data.slideSpeed - _rb.velocity.y;	
		float movement = speedDif * data.slideAccel;
		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		_rb.AddForce(movement * Vector2.up);
	}
    #endregion


    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

    private bool CanJump()
    {
		return TimeAfterOnGround > 0 && !IsJumping;
    }

	private bool CanWallJump()
    {
		return TimeAfterPressJumpBtn > 0 && TimeAfterOnWall > 0 && TimeAfterOnGround <= 0 && (!IsWallJumping ||
			 (TimeAfterOnWallRight > 0 && _lastWallJumpDir == 1) || (TimeAfterOnWallLeft > 0 && _lastWallJumpDir == -1));
	}

	private bool CanJumpCut()
    {
		return IsJumping && _rb.velocity.y > 0;
    }

	private bool CanWallJumpCut()
	{
		return IsWallJumping && _rb.velocity.y > 0;
	}

	private bool CanDash()
	{
		if (!IsDashing && _dashesLeft < data.dashAmount && TimeAfterOnGround > 0 && !_dashRefilling)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashesLeft > 0;
	}

	public bool CanSlide()
    {
		if (TimeAfterOnWall > 0 && !IsJumping && !IsWallJumping && !IsDashing && TimeAfterOnGround <= 0)
			return true;
		else
			return false;
	}
    #endregion
}
