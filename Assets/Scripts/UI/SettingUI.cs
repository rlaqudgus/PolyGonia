using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [Header("Button")]
    public Button backToMenuButton;

    [Header("Slider")] 
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Toggle")] 
    public Toggle musicToggle;
    public Toggle sfxToggle;
    
    private void Awake()
    {
        musicSlider.value = 1;
        sfxSlider.value = 1;
    }

    private void Start()
    {
        backToMenuButton?.onClick.AddListener(UIManager.Instance.ClosePopupUI);
        musicSlider?.onValueChanged.AddListener(SoundManager.Instance.SetMusicVolume);
        sfxSlider?.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
        musicToggle?.onValueChanged.AddListener(SoundToggle);
        sfxToggle?.onValueChanged.AddListener(SfxToggle);
    }

    private static void SfxToggle(bool toggle) => SoundManager.Instance.ToggleSFX();
    private static void SoundToggle(bool toggle) => SoundManager.Instance.ToggleMusic();
}
