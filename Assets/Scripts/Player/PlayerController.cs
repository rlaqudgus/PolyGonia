using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;


// PlayerInput의 Invoke Unity Events 사용
// Action에 미리 mapping 해놓은 키가 불렸을 때 Unity Events를 호출한다. 
public class PlayerController : MonoBehaviour,IDamageable, IAttackable
{
    Vector2 moveDir;
    public CameraController cam;
    [SerializeField] float moveSpd;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCutMultiplier;
    [SerializeField] float invincibleTime;
    [SerializeField] int jumpCounter;
    [SerializeField] int maxHP;
    [SerializeField] int HP;

    int initJumpCounter;

    bool isMoving;
    bool isLookUp;
    bool isLookDown;
    public bool isShield;
    bool isAttack;
    bool isParry;
    bool isJumping;
    bool isInvincible; 

    Animator anim;

    Shield shield;

    Rigidbody2D rb;

    bool isJumpingDown => rb.velocity.y < 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shield = GetComponentInChildren<Shield>(true);
        initJumpCounter = jumpCounter;
        HP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = (moveDir != Vector2.zero);

        PlayerMove();
        PlayerAnim();
    }

    //Event를 통해 호출되는 함수
    //input에 따라 moveDir를 변경
    public void OnMove(InputAction.CallbackContext input)
    {
        Vector2 playerInput = input.ReadValue<Vector2>();

        if (playerInput != null)
        {
            moveDir = new Vector2(playerInput.x, 0);
            this.Log($"Move Direction : {moveDir}");
        }
    }

    public void OnLook(InputAction.CallbackContext input)
    {
        Vector2 playerInput = input.ReadValue<Vector2>();

        //키를 뗄 때도 호출되기 때문에 0입력되고 false시켜줌
        if (playerInput != null)
        {
            this.Log($"Look Direction{playerInput}");
            switch (playerInput.y)
            {
                case 0:
                    isLookDown = false;
                    isLookUp = false;
                    break;
                case 1:
                    isLookUp = true;
                    break;
                case -1:
                    isLookDown = true;
                    break;
            }
        } 
    }

    // Axis는 눌렀을 때 1, 뗐을 때 0으로 변하는 float return
    // 계속 누르고 있으면 계속 True를 반환하는 isShield 변수 - 이놈은 shield idle 애니메이션 할 때 쓰기
    // 한번 눌렀을 때 딱 한번 실행하는 애니메이션(방패 뽑는 애니메이션)을 써야하기 때문에 trigger 변수 하나 더 만들어주자 
    public void OnShield(InputAction.CallbackContext input)
    {
        isShield = input.ReadValueAsButton();

        if (input.performed)
        {
            this.Log(isShield);
            anim.SetTrigger("Shield");
            shield.ShieldActivate(isShield);
        }

        if (input.canceled)
        {
            this.Log(isShield);
            shield.ShieldActivate(isShield);
        }
        
    }

   
    // shield 상태일때와 아닐 때 구분
    public void OnAttack(InputAction.CallbackContext input)
    {
        isAttack = input.ReadValueAsButton();

        if (input.performed)
        {
            // 눌렀을 때 shieldbox를 끄고 parrybox를 켠다
            if (isShield)
            {
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

        //너무 빨리 뗐을 때 트리거가 reset되지 않음 수동으로 reset 해주자..
        if (input.canceled)
        {
            anim.ResetTrigger("Parry");
        }
    }

    // responsive 하게 만들고싶다
    // hold 하는 시간에 따라 점프 높이 달라지게 (Jump Cut)
    // coyote time은 가능하면?
    public void OnJump(InputAction.CallbackContext input)
    {
        this.Log("executed");
        if (input.performed)
        {
            this.Log("Performed");
            Jump();
        }

        //점프중에 키를 빠르게 놓았을 때 근데 내려갈때는 작동되면 안됨
        if(input.canceled && isJumping && !isJumpingDown)
        {
            this.Log("Jump Cut");
            JumpCut();
            //dosth
        }
    }

    void Jump()
    {
        if (jumpCounter>0) 
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            jumpCounter--;
        }
    }

    void JumpCut()
    {
        //Vector2 jumpCutVec = new Vector2(rb.velocity.x, 0);
        //rb.velocity = jumpCutVec;
        rb.AddForce(Vector2.down * rb.velocity.y * jumpCutMultiplier, ForceMode2D.Impulse);
    }

    void PlayerMove()
    {   //위나 아래를 보고 있을 때 움직이면 바로 움직이게
        if (isMoving && !isParry && !isShield)
        {
            //방어상태 이동 그냥 이동 차이
            var moveAmount = moveDir.x * moveSpd * Time.deltaTime;
            var move = isShield ? moveAmount * 0.5f : moveAmount;

            Vector3 newPos = new Vector3(transform.position.x + move, transform.position.y, transform.position.z);
            transform.position = newPos;

            //x축 부호 바꾸기 (좌우반전)
            Vector2 newScale = move < 0 ? new Vector2(-1, transform.localScale.y) : new Vector2(1, transform.localScale.y);
            transform.localScale = newScale;
            
        }
    }
    void PlayerAnim()
    {
        
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isShield", isShield);

        // 움직이면 바라보는 애니메이션 중지
        if (isMoving ) 
        {
            anim.SetBool("isLookUp", false);
            anim.SetBool("isLookDown", false);
            return;
        }

        anim.SetBool("isLookUp", isLookUp);
        anim.SetBool("isLookDown", isLookDown);
        
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
            isJumping = false;
            jumpCounter = initJumpCounter;
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
        StartCoroutine(cam.Shake());
        
    }

    IEnumerator InvincibleEffect()
    {
        //최상위 콜라이더 invincible로 변경
        gameObject.layer = 31;
        //하위 모든 오브젝트 invincible로 변경
        foreach (Transform child in transform)
        {
            child.gameObject.layer = 31;
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
            child.gameObject.layer = 6;
        }

    }

    IEnumerator InvincibleTimer()
    {
        float timer = 0;
        while (timer <= invincibleTime) 
        {
            isInvincible = true;
            ;
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
