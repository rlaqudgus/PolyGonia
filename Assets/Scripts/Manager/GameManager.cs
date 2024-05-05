using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public  static GameManager  Instance { get { return _instance; } }

    public static event Action<GameState> OnGameStateChanged;

    public SoundManager soundManager;
    public UIManager uiManager;
    public CameraManager cameraManager;
    public PlayerController playerController;
    public PlayerInput playerInput;

    public GameState gameState;

    public PlayerEvents playerEvents;
    public MiscEvents miscEvents;
    public QuestEvents questEvents;

    private void Start()
    {
        
    }

    private void Awake()
    {
        //instance가 초기화되지 않았다면
        if (_instance == null)
        {
            //자신으로 초기화
            _instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        miscEvents = new MiscEvents();
        playerEvents = new PlayerEvents();
        questEvents = new QuestEvents();
    }

    public void UpdateGameState(GameState newGameState)
    {
        this.Log($"Game State is updated to {newGameState.ToString()}");

        gameState = newGameState;

        switch (newGameState)
        {
            case GameState.Init:
                Init();
                break;
            case GameState.Adventure:
                Adventure();
                break;
            case GameState.Inventory:   
                Inventory();    
                break;  
            case GameState.Cinematic:
                Cinematic();
                break;
            case GameState.Die:
                Die();
                break;
            case GameState.LowHealth:
                break;
            case GameState.CutScene:
                CutScene();
                break;
            default:
                throw new ArgumentException(
                    $"Invalid Game State: {newGameState.ToString()}", 
                    nameof(newGameState)
                );
                // break;
                // GameState 추가 시 break 거는 것 유의
        }

        OnGameStateChanged?.Invoke(newGameState);
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

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        
        #elif UNITY_WEBPLAYER
            Application.OpenURL("http://google.com");
        
        #else
            Application.Quit();
        
        #endif
    }
}
