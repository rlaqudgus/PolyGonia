using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;


// PlayerInput의 Invoke Unity Events 사용
// Action에 미리 mapping 해놓은 키가 불렸을 때 Unity Events를 호출한다. 
public class PlayerController : MonoBehaviour,IDamageable, IAttackable
{
    public CameraController cam;
    [SerializeField] RayBox ray;
    [SerializeField] float moveSpd;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCutMultiplier;
    [SerializeField] float invincibleTime;
    [SerializeField] int jumpCounter;
    [SerializeField] int maxHP;
    [SerializeField] int HP;

    int initJumpCounter;

    Vector2 moveDir;
    int dir;
    bool isMoving;
    bool isLookUp;
    bool isLookDown;

    public bool isShield;
    bool isAttacking;
    bool isParry;
    
    bool isJumping;
    bool isJumpingDown => rb.velocity.y < 0;

    bool isInvincible; 

    PlayerInput playerInput;

    Rigidbody2D rb;

    SpriteRenderer sr;

    Animator anim;

    Shield shield;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        shield = GetComponentInChildren<Shield>(true);

        initJumpCounter = jumpCounter;
        HP = maxHP;
    }


    // Update is called once per frame
    void Update()
    {
        // Do not put Move() inside the OnMove()
        // OnMove() is invoked when the input is changed - i.e. pressed or released
        Move();
    }

    //Event를 통해 호출되는 함수
    //input에 따라 moveDir를 변경
    public void OnMove(InputAction.CallbackContext input)
    {
        Vector2 playerInput = input.ReadValue<Vector2>();

        if (playerInput.x > Mathf.Epsilon) dir = 1;
        else if (playerInput.x < -Mathf.Epsilon) dir = -1;
        else dir = 0;
        
        moveDir = Vector2.right * dir;
        // this.Log($"Move Direction : {moveDir}");

        isMoving = (dir != 0);
        // this.Log($"isMoving : {isMoving}");
    }

    private void Move()
    {
        if (isMoving)
        {
            //방어상태 이동 그냥 이동 차이
            float moveAmount = 1.0f;
            if (isShield) moveAmount *= 0.25f;
            if (isParry) moveAmount *= 0.25f;
            
            // Move
            transform.Translate(moveDir * moveSpd * moveAmount * Time.deltaTime);
            
            //x축 부호 바꾸기 (좌우반전)
            transform.localScale = new Vector2(dir, transform.localScale.y);;
        }
        
        // Animation Transition
        anim.SetBool("isMoving", isMoving);
    }

    public void OnLook(InputAction.CallbackContext input)
    {
        Vector2 playerInput = input.ReadValue<Vector2>();

        isLookUp = (playerInput.y > 0);
        isLookDown = (playerInput.y < 0);

        //위나 아래를 보고 있을 때 움직이면 바로 움직이게
        //키를 뗄 때도 호출되기 때문에 0입력되고 false시켜줌
        if (isMoving) 
        {
            isLookUp = false;
            isLookDown = false;
        }

        Look();
    }

    private void Look() 
    {
        anim.SetBool("isLookUp", isLookUp);
        anim.SetBool("isLookDown", isLookDown);

        if (isLookUp) CameraManager.Look(new Vector3(0f, 5f, 0f), 0.1f);
        else if (isLookDown) CameraManager.Look(new Vector3(0f, 0f, 0f), 0.1f);
        else CameraManager.Look(new Vector3(0f, 2.5f, 0f), 0.1f);
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
            anim.SetTrigger("Shield");
        }

        shield.ShieldActivate(isShield);
        anim.SetBool("isShield", isShield);

    }

   
    // shield 상태일때와 아닐 때 구분
        public void OnAttack(InputAction.CallbackContext input)
    {
        isAttacking = input.ReadValueAsButton();
        Attack();
    }

    private void Attack() {
        if (isAttacking)
        {
            // 눌렀을 때 shieldbox를 끄고 parrybox를 켠다
            if (isShield)
            {
                this.Log($"isParry: {isParry}");
                anim.SetTrigger("Parry");
                StartCoroutine(CheckParry());
                shield.ShieldParry();

            }
            else
            {
                this.Log("Attack");
                anim.SetTrigger("Attack");
            }
        }

        ////너무 빨리 뗐을 때 트리거가 reset되지 않음 수동으로 reset 해주자..
        else
        {
            anim.ResetTrigger("Parry");
        }
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
        if (jumpCounter <= 0) return;

        this.Log("Performed");
        isJumping = true;
        jumpCounter--;

        // Y velocity after adding force is the same as the initial jump velocity
        // It keeps the jump height whenever the jump key is pressed
        rb.AddForce(Vector2.up * rb.mass * (jumpForce - rb.velocity.y), ForceMode2D.Impulse);

        // This is the old jump model
        // jump height does not stay the same because the net force is changing
        // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void JumpCut()
    {
        // Adjustable jump height
        if (!isJumping) return;
        if (isJumpingDown) return;
        this.Log("Jump Cut");

        rb.AddForce(Vector2.down * rb.velocity.y * rb.mass * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
    }

    IEnumerator CheckParry()
    {
        isParry = true;
        yield return new WaitForSeconds(.5f);
        isParry = false;
    }

    public void PlayerKnockBack()
    {
        transform.position = (Vector2)transform.position - new Vector2(transform.localScale.x * 0.5f,0);
    }

    //ㅈㄴ마음에안든다
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            if (ray.CheckWithBox()) 
            {
                isJumping = false;
                jumpCounter = initJumpCounter;
            }
        }
    }

   

    public void Damaged(int dmg)
    {
        DamageEffect();
        this.Log($"currentHp : {HP} - {dmg} = {HP-=dmg}");
    }

    void DamageEffect()
    {
        StartCoroutine(InvincibleTimer());
        StartCoroutine(InvincibleEffect());
        CameraManager.Shake();
        
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
        while (isInvincible)
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
        while (timer <= invincibleTime) 
        {
            isInvincible = true;
            
            timer += Time.deltaTime;
            yield return null;
        }
        isInvincible = false;
        
    }

    public void ByShield(Shield shield)
    {
        PlayerKnockBack();
    }

    public void ByParry(Shield shield)
    {
        throw new System.NotImplementedException();
    }

    public void BySpear()
    {
        throw new System.NotImplementedException();
    }
}
