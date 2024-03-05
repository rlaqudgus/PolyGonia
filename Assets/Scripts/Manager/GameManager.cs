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
        //instance�� �ʱ�ȭ���� �ʾҴٸ�
        if (instance == null)
        {
            //�ڽ����� �ʱ�ȭ
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
