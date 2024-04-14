using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;

public class PauseManager : MonoBehaviour
{
    private static PauseManager _instance;
    public  static PauseManager  Instance { get { return _instance; } }

    private GameState _lastGameState;
    public bool isPaused;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        isPaused = false;
    }

    public void Pause()
    {
        _lastGameState = GameManager.Instance.currentGameState;
        GameManager.Instance.UpdateGameState(GameState.Pause);
        isPaused = true;
        Time.timeScale = 0;

        // Sound
        SoundManager.Instance.pauseSnapshot.TransitionTo(1f);

        // UI
        UIManager.Instance.pauseUI.SetActive(true);
        // UIManager.Instance.pauseButton.SetActive(false);

        // Input System
        GameManager.Instance.playerInput.SwitchCurrentActionMap("UI");
    }

    public void Resume()
    {
        GameManager.Instance.UpdateGameState(_lastGameState);
        isPaused = false;
        Time.timeScale = 1;

        // Sound
        SoundManager.Instance.Transition(1f);

        // UI
        UIManager.Instance.pauseUI.SetActive(false);
        // UIManager.Instance.pauseButton.SetActive(true);

        // Input System
        GameManager.Instance.playerInput.SwitchCurrentActionMap("Player");
    }
}
