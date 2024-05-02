using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class JoyconInputManager : MonoBehaviour
{
    private List<Joycon> joycons;

    //joycon index가 필요한가? 굳이?
    public int jc_index = 0;

    public Joycon leftJoyCon;
    public Joycon rightJoyCon;

    [SerializeField] bool isShouldertrigger;
    [SerializeField] bool isShoulderPressed;
    [SerializeField] bool isAttackbuttontrigger;

    [SerializeField] Vector2 leftStick;

    //Callbackcontext를 그대로 이용하는 방법? inputsystem을 사용하지 않을 것이기 때문에
    //매개변수를 어떻게 넣어줄지 의문 조이콘 인풋은 따로 관리해주는 편이 좋을 듯 하다
    [SerializeField] UnityEvent<Vector2> JoyCon_Move;
    public UnityEvent<bool> JoyCon_Shield;
    public UnityEvent<bool> JoyCon_ShieldTrigger;
    public UnityEvent<bool> JoyCon_Attack;
    public UnityEvent JoyCon_Parry;
    // Start is called before the first frame update
    void Start()
    {
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyConManager.Instance.j;
        if (joycons.Count < jc_index + 1)
        {
            Destroy(gameObject);
        }

        //left right joycon initialize
        leftJoyCon = joycons[0];
        rightJoyCon = joycons[1];
    }

    // Update is called once per frame
    void Update()
    {
        JoyConInput();
        JoyConEvent();
    }

    void JoyConInput()
    {
        leftStick = new Vector2(leftJoyCon.GetStick()[0], leftJoyCon.GetStick()[1]).normalized;
        isShouldertrigger = rightJoyCon.GetButtonDown(Joycon.Button.SHOULDER_2);
        isShoulderPressed = rightJoyCon.GetButton(Joycon.Button.SHOULDER_2);
        isAttackbuttontrigger = rightJoyCon.GetButtonDown(Joycon.Button.DPAD_UP);
    }

    void JoyConEvent()
    {
        //if (leftStick == Vector2.zero) return;
        //분기처리에 신경을 써줘야한다 event를 update에서 부르고 있으니 너무 많이 불리면 문제생길 가능성
        if(leftStick!=Vector2.zero) JoyCon_Move.Invoke(leftStick);
        JoyCon_ShieldTrigger.Invoke(isShouldertrigger);
        JoyCon_Shield.Invoke(isShoulderPressed);
        if (isAttackbuttontrigger && isShoulderPressed) JoyCon_Parry.Invoke();
    }
}
