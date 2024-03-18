using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;


// PlayerInput�� Invoke Unity Events ���
// Action�� �̸� mapping �س��� Ű�� �ҷ��� �� Unity Events�� ȣ���Ѵ�. 
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

    //Event�� ���� ȣ��Ǵ� �Լ�
    //input�� ���� moveDir�� ����
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

        //Ű�� �� ���� ȣ��Ǳ� ������ 0�Էµǰ� false������
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

    // Axis�� ������ �� 1, ���� �� 0���� ���ϴ� float return
    // ��� ������ ������ ��� True�� ��ȯ�ϴ� isShield ���� - �̳��� shield idle �ִϸ��̼� �� �� ����
    // �ѹ� ������ �� �� �ѹ� �����ϴ� �ִϸ��̼�(���� �̴� �ִϸ��̼�)�� ����ϱ� ������ trigger ���� �ϳ� �� ��������� 
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

   
    // shield �����϶��� �ƴ� �� ����
    public void OnAttack(InputAction.CallbackContext input)
    {
        isAttack = input.ReadValueAsButton();

        if (input.performed)
        {
            // ������ �� shieldbox�� ���� parrybox�� �Ҵ�
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

        //�ʹ� ���� ���� �� Ʈ���Ű� reset���� ���� �������� reset ������..
        if (input.canceled)
        {
            anim.ResetTrigger("Parry");
        }
    }

    // responsive �ϰ� �����ʹ�
    // hold �ϴ� �ð��� ���� ���� ���� �޶����� (Jump Cut)
    // coyote time�� �����ϸ�?
    public void OnJump(InputAction.CallbackContext input)
    {
        this.Log("executed");
        if (input.performed)
        {
            this.Log("Performed");
            Jump();
        }

        //�����߿� Ű�� ������ ������ �� �ٵ� ���������� �۵��Ǹ� �ȵ�
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
    {   //���� �Ʒ��� ���� ���� �� �����̸� �ٷ� �����̰�
        if (isMoving && !isParry && !isShield)
        {
            //������ �̵� �׳� �̵� ����
            var moveAmount = moveDir.x * moveSpd * Time.deltaTime;
            var move = isShield ? moveAmount * 0.5f : moveAmount;

            Vector3 newPos = new Vector3(transform.position.x + move, transform.position.y, transform.position.z);
            transform.position = newPos;

            //x�� ��ȣ �ٲٱ� (�¿����)
            Vector2 newScale = move < 0 ? new Vector2(-1, transform.localScale.y) : new Vector2(1, transform.localScale.y);
            transform.localScale = newScale;
            
        }
    }
    void PlayerAnim()
    {
        
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isShield", isShield);

        // �����̸� �ٶ󺸴� �ִϸ��̼� ����
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

    //�����������ȵ��
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
        //�ֻ��� �ݶ��̴� invincible�� ����
        gameObject.layer = 31;
        //���� ��� ������Ʈ invincible�� ����
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
        //���� ��� ������Ʈ 
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
