using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public SoundManager soundManager;
    public UIManager uiManager;
    public PlayerController playerController;
    public CameraController cameraController;

    public enum GameState { Init, Adventure, Inventory, Pause, Cinematic, Die, CutScene }

    private void Start()
    {
        
    }

    private void Awake()
    {
        //instance가 초기?�되지 ?�았?�면
        if (instance == null)
        {
            //?�신?�로 초기??
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        //dosth
        //GameStart UI Animation
        //GameStart Sound
        //Player Spawn
    }

    public void Adventure()
    {
        //dosth
        //���� ���� �����ϰ� ���� ��
        //���� ��Ȳ�� ���� BGM, sound ��� �ٸ���
        //
    }

    public void Inventory()
    {
        //dosth
        //�ҷο쳪��Ʈ�� ��� �κ��丮�� ���� ���� ���� �� ���� ��Ȳ�� �޶���
        //ex �ӵ��� �������� ������ ���� �÷��̾� �ִϸ��̼� ����
        //Inventory�� ���� ���� �� player�� ������ �� �ְ� �� ���ΰ�?
        //�ƿ� ���� pause�� �� ���ΰ�?
        //sound on / off?
    }

    public void Pause() 
    {
        //dosth
        //Pause sound
        //sound on / off
        //Pause UI
    }

    public void Cinematic()
    {
        //dosth
        //sound
        //UI
        //Camera
        //Timeline?
    }

    public void CutScene()
    {
        //dosth
        //sound
        //UI
        //Camera
        //TimeLine?
    }

    public void Die()
    {
        //doth
        //sound
        //UI
        //Camera
    }
}
