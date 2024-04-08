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
        //instance가 초기화되지 않았다면
        if (instance == null)
        {
            //자신으로 초기화
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
        //실제 게임 실행하고 있을 때
        //게임 상황에 따른 BGM, sound 등등 다르게
        //
    }

    public void Inventory()
    {
        //dosth
        //할로우나이트의 경우 인벤토리나 맵을 보고 있을 때 게임 상황이 달라짐
        //ex 속도가 느려지고 지도를 보는 플레이어 애니메이션 실행
        //Inventory를 보고 있을 때 player가 움직일 수 있게 할 것인가?
        //아예 게임 pause를 할 것인가?
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
