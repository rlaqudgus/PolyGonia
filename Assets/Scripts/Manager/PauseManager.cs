using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Utilities;


public class PauseManager : MonoBehaviour
{
    private static PauseManager _instance;
    public  static PauseManager  Instance { get { return _instance; } }

    public bool isPaused;
    private InputActionMap _lastActionMap;

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
        _lastActionMap = GameManager.Instance.playerInput.currentActionMap;
    }

    public void Pause()
    {
        GameManager.Instance.playerInput = FindObjectOfType<PlayerInput>();

        isPaused = true;
        _lastActionMap = GameManager.Instance.playerInput.currentActionMap;
        Time.timeScale = 0;

        // Sound
        SoundManager.Instance.pauseSnapshot.TransitionTo(1f);
        SoundManager.Instance.PauseVoice();

        // UI
        UIManager.Instance.OpenMainMenu();

        UIManager.Instance.CloseMobileUI();

        // Input System
        if (_lastActionMap.name != "UI")
        {
            GameManager.Instance.playerInput.SwitchCurrentActionMap("UI");   
        }

        this.Log("Pause");
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;

        // Sound
        SoundManager.Instance.Transition(1f);
        SoundManager.Instance.ResumeVoice();

        // UI
        UIManager.Instance.CloseAllMenus();

        UIManager.Instance.OpenMobileUI();

        // Input System
        if (_lastActionMap.name == "Player")
        {
            GameManager.Instance.playerInput.SwitchCurrentActionMap("Player");
        }
        else if (_lastActionMap.name == "UI")
        {
            int n_windows = UIManager.Instance.windows.Count;

            Debug.Assert(
                n_windows > 0,
                "Last Action Map is UI but nothing is in the windows list"
            );

            Window recentWindow = UIManager.Instance.windows[n_windows-1];
            recentWindow.Reload();

        }

        this.Log("Resume");
    }

    public void TogglePause()
    {
        if (!isPaused) Pause();
        else Resume();
    }
}
