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

    [Header("First Selected Options")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;

    [Header("Sound Volume Controller")]
    [SerializeField] private Slider _musicVolumeController;
    [SerializeField] private Slider _sfxVolumeController;
    

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
        _mainMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
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

    public void MusicVolume()
    {   
        SoundManager.Instance.SetMusicVolume(_musicVolumeController.value);
    }   

    public void SFXVolume()
    {   
        SoundManager.Instance.SetSFXVolume(_sfxVolumeController.value);
    }   
}
