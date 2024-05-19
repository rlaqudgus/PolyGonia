using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Button")]
    public Button resumeButton;
    public Button settingButton;
    public Button exitButton;

    private void Start()
    {
        resumeButton?.onClick.AddListener(UIManager.Instance.ClosePopupUI);
        settingButton?.onClick.AddListener(openSetting);
    }

    private void openSetting() => UIManager.Instance.OpenPopupUI(UIManager.SETTING_MENU);
}
