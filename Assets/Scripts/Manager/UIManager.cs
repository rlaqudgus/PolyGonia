using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public  static UIManager  Instance { get { return _instance; } }

    public GameObject[] userInterface;
    private Dictionary<string, GameObject> _uiDict = new Dictionary<string, GameObject>();

    public GameObject pauseUI;

    public Slider MusicVolumeController;
    public Slider SFXVolumeController;

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
        foreach (GameObject ui in userInterface) 
        {
            _uiDict.Add(ui.name, ui);
        }

        MusicVolumeController.value = 1f;
        SFXVolumeController.value   = 1f;
    }   

    public bool IsActive(string name)
    {   
        GameObject ui = _uiDict[name];
        return ui.activeSelf;
    }   

    public void EnableUI(string name)
    {   
        GameObject ui = _uiDict[name];
        ui.SetActive(true);
    }   

    public void DisableUI(string name)
    {   
        GameObject ui = _uiDict[name];
        ui.SetActive(false);
    }   

    public void ToggleUI(string name)
    {   
        GameObject ui = _uiDict[name];
        ui.SetActive(!ui.activeSelf);

        string state = ui.activeSelf ? "on" : "off";
        this.Log($"{name} is {state}");
    }   

    public void MusicVolume()
    {   
        SoundManager.Instance.SetMusicVolume(MusicVolumeController.value);
    }   

    public void SFXVolume()
    {   
        SoundManager.Instance.SetSFXVolume(SFXVolumeController.value);
    }   
}
