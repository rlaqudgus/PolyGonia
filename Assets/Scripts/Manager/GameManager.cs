using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Utilities;

public enum GameState
{
    Init,
    GamePlay,
}

public class GameManager : Singleton<GameManager>
{
    public PlayerController playerController;


    public GameState gameState;

    public PlayerEvents playerEvents;
    public MiscEvents miscEvents;

    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        CreateSingleton(this);
        miscEvents = new MiscEvents();
        playerEvents = new PlayerEvents();
    }

    private void Start()
    {
        UIManager.Instance.OpenBackgroundUI(UIManager.BACKGROUND_CANVAS);
    }


    #region Gamestate

    public void UpdateGameState(GameState newGameState)
    {
        this.Log($"Game State is updated to {newGameState.ToString()}");

        gameState = newGameState;

        switch (newGameState)
        {
            case GameState.Init:
                InitScene();
                break;
            case GameState.GamePlay:
                GamePlay();
                break;
        }
    }

    public void InitScene()
    {
    }





    public void GamePlay()
    {

    }
    #endregion
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

    public void LoadScene(string sceneName)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);
        StartCoroutine(LoadSceneAsync(operation));
    }

    public void LoadScene(int sceneId)
    {
        var operation = SceneManager.LoadSceneAsync(sceneId);
        StartCoroutine(LoadSceneAsync(operation));
    }

    private IEnumerator LoadSceneAsync(AsyncOperation operation)
    {
        // loadingScreen.SetActive(true);
        while (!operation!.isDone)
        {
            this.Log($"Loading...{operation.progress}%");
            float curProgress = Mathf.Clamp01(operation.progress / 0.9f);
            // loadingbar.value = curProgress;

            yield return null; // 다음 프레임에서 실행되도록 대기

        }

        // PauseManager.Instance.isPaused = false;
        Time.timeScale = 1;
        // Sound
        SoundManager.Instance.Transition(1f);
        // UI
        UIManager.Instance.CloseAllPopupUI();
    }


   
}
