using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public SoundManager soundManager;
    public UIManager uiManager;
    public PlayerController playerController;

    public enum GameState { Init, Adventure, Inventory, Pause, Cinematic, Die }

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
}
