using System;
using System.Collections.Generic;
using Bases;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace Manager
{
    public class UIManager : Singleton<UIManager>
    {
        public GameObject[] uiArray;
        private readonly Dictionary<string, GameObject> _uiDic = new Dictionary<string, GameObject>();
        private int _order = 10;
        private bool _pause = false;

        private readonly Stack<GameObject> _popupStack = new Stack<GameObject>();
        private GameObject _background;

        #region UIObjectName

        public const string MAIN_MENU = "Main Menu";
        public const string SETTING_MENU = "Settings Menu";
        public const string MOBILE_CANVAS = "Mobile Canvas";
        public const string DIALOGUE_CANVAS = "Dialogue Canvas";
        
        #endregion
        
        private void Awake()
        {
            CreateSingleton(this);

            foreach (var uiObj in uiArray)
            {
                uiObj.SetActive(false);
                _uiDic.Add(uiObj.name, uiObj);
            }
        }

        #region UI

        public void OpenPopupUI(string uiName) => OpenPopupUI(uiName, false);
        public void OpenPopupUI(string uiName, bool pauseOnUI)
        {
            GameObject uiObj = OpenUI(uiName, _order);
            if (uiObj == null) return;
            
            _order++;
            EventSystem.current.SetSelectedGameObject(uiObj.transform.GetChild(0).gameObject);
            _popupStack.Push(uiObj);
            
            if(pauseOnUI) Pause();
        }

        public void OpenBackgroundUI(string uiName)
        {
            GameObject uiObj = OpenUI(uiName, -1);
            if (uiObj == null) return;
            
            _background?.SetActive(false);
            _background = uiObj;
        }

        public void ClosePopupUI()
        {
            if(_popupStack.Count==0) return;

            GameObject popup = _popupStack.Pop();
            popup.SetActive(false);

            if (_popupStack.Count >= 1)
                EventSystem.current.SetSelectedGameObject(_popupStack.Peek().transform.GetChild(0).gameObject);
            _order--;

            if (_pause) Resume();
        }
        
        public void CloseAllPopupUI()
        {
            while(_popupStack.Count >0) ClosePopupUI();
            EventSystem.current.SetSelectedGameObject(null);
        }

        private GameObject OpenUI(string uiName, int sortingOrder)
        {
            GameObject uiObj = _uiDic[uiName];

            if (uiObj.activeSelf) return null;
            
            Canvas canvas = uiObj.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;

            uiObj.GetOrAddComponent<GraphicRaycaster>();
            uiObj.SetActive(true);
            
            return uiObj;
        }

        #endregion

        #region Pause

        public void Pause()
        {
            _pause = true;
            Time.timeScale = 0;

            // Sound
            SoundManager.Instance.pauseSnapshot.TransitionTo(1f);
            SoundManager.Instance.PauseVoice();

            // Input System
            KeyboardInputManager.Instance.UpdateInputState(KeyboardInputManager.UI);
        }

        public void Resume()
        {
            _pause = false;
            Time.timeScale = 1;

            // Sound
            SoundManager.Instance.Transition(1f);
            SoundManager.Instance.ResumeVoice();

            // Input System
            KeyboardInputManager.Instance.UpdateInputState(KeyboardInputManager.PLAYER);
        }
        
        #endregion
    }
}