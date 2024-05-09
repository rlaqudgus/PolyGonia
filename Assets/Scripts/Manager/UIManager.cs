using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utilities;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public  static UIManager  Instance { get { return _instance; } }

    [Header("Menu Objects")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _settingsMenu;
    
    [Header("Window Objects")]
    [HideInInspector] public List<Window> windows = new List<Window>();
    [SerializeField] private GameObject _dialogueWindow;

    [Header("First Selected Options")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;

    [Header("Sound Volume Controller")]
    [SerializeField] private Slider _musicVolumeController;
    [SerializeField] private Slider _sfxVolumeController;
    

    [Header("Mobile UI")]
    [SerializeField] private GameObject _mobileUI;

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

    void Start()
    {
        // Sound
        _musicVolumeController.value = 1f;
        _sfxVolumeController.value   = 1f;
        
        // UI
        CloseAllMenus();

        // Subscribe UI Action by case
        GameManager.OnGameStateChanged += UIByGameState;
    }

    private void OnDestroy()
    {
        // Not gonna be destroyed because it's Singleton but unsubscribe anyway..?
        GameManager.OnGameStateChanged -= UIByGameState;
    }

    private void UIByGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Init:
                break;
            case GameState.Pause:
                break;
            case GameState.Adventure:
                break;
            case GameState.Inventory:
                break;
            case GameState.Cinematic:
                break;
            case GameState.Die:
                break;
            case GameState.LowHealth:
                break;
            case GameState.CutScene:
                break;
            default:
                break;
        }
    }

    public void OpenMainMenu()
    {
        CloseAllMenus();
        this.Log("close finish");
        _mainMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
        this.Log("setactive true");
    }
    
    public void OpenSettingsMenu()
    {
        CloseAllMenus();
        _settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }
    
    public void CloseAllMenus()
    {
        _mainMenu.SetActive(false);
        _settingsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }


    #region Dialogue

    // [SH] [2024-04-29]
    // Dialogue 상황에서 Pause를 처리하는 것이 까다롭다
    // 여러 개의 UI가 있을 때 이 UI들의 순서를 기억하는 시스템이 필요하다
    // 그래서 Window 클래스를 만들고 리스트를 사용하였음

    public void OpenDialogueWindow()
    {
        Window window = _dialogueWindow.GetComponent<Window>();
        window.Open();
    }

    public void CloseDialogueWindow()
    {
        Window window = _dialogueWindow.GetComponent<Window>();
        window.Close();
    }

    #endregion

    public void MusicVolume()
    {   
        SoundManager.Instance.SetMusicVolume(_musicVolumeController.value);
    }   

    public void SFXVolume()
    {   
        SoundManager.Instance.SetSFXVolume(_sfxVolumeController.value);
    }

    public void CloseMobileUI()
    {
        _mobileUI.SetActive(false);
    }

    public void OpenMobileUI()
    {
        _mobileUI.SetActive(true);
    }
}
