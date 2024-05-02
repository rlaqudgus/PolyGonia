using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class JoyconInputManager : MonoBehaviour
{
    private List<Joycon> joycons;

    //joycon index�� �ʿ��Ѱ�? ����?
    public int jc_index = 0;

    public Joycon leftJoyCon;
    public Joycon rightJoyCon;

    [SerializeField] bool isShouldertrigger;
    [SerializeField] bool isShoulderPressed;
    [SerializeField] bool isAttackbuttontrigger;

    [SerializeField] Vector2 leftStick;

    //Callbackcontext�� �״�� �̿��ϴ� ���? inputsystem�� ������� ���� ���̱� ������
    //�Ű������� ��� �־����� �ǹ� ������ ��ǲ�� ���� �������ִ� ���� ���� �� �ϴ�
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
        //�б�ó���� �Ű��� ������Ѵ� event�� update���� �θ��� ������ �ʹ� ���� �Ҹ��� �������� ���ɼ�
        if(leftStick!=Vector2.zero) JoyCon_Move.Invoke(leftStick);
        JoyCon_ShieldTrigger.Invoke(isShouldertrigger);
        JoyCon_Shield.Invoke(isShoulderPressed);
        if (isAttackbuttontrigger && isShoulderPressed) JoyCon_Parry.Invoke();
    }
}
