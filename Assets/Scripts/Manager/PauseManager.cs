using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;
 
public class PauseManager : MonoBehaviour
{
    private static PauseManager _instance;
    public  static PauseManager  Instance { get { return _instance; } }

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
        isPaused = true;
        Time.timeScale = 0;

        // Sound
        SoundManager.Instance.pauseSnapshot.TransitionTo(1f);

        // UI
        UIManager.Instance.OpenMainMenu();

        UIManager.Instance.CloseMobileUI();

        // Input System
        GameManager.Instance.playerInput.SwitchCurrentActionMap("UI");

        this.Log("Pause");
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;

        // Sound
        SoundManager.Instance.Transition(1f);

        // UI
        UIManager.Instance.CloseAllMenus();

        UIManager.Instance.OpenMobileUI();

        // Input System
        GameManager.Instance.playerInput.SwitchCurrentActionMap("Player");

        this.Log("Resume");
    }
}
