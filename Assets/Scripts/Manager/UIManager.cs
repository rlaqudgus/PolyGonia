using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

public class UIManager : Singleton<UIManager>
    {
        public GameObject[] uiArray;
        private readonly Dictionary<string, GameObject> _uiDic = new Dictionary<string, GameObject>();
        private int _order = 10;
        private bool _pause = false;
        public bool isPaused { get { return _pause; } }

        private readonly Stack<GameObject> _popupStack = new Stack<GameObject>();
        private GameObject _background = null;
        private GameObject _mobilePad;
        
        #region UIObjectName

        public const string MAIN_MENU = "Main Menu";
        public const string SETTING_MENU = "Settings Menu";
        public const string MOBILE_CANVAS = "Mobile Canvas";
        public const string DIALOGUE_CANVAS = "Dialogue Canvas";
        public const string MAP_CANVAS = "Map Canvas";
        public const string BACKGROUND_CANVAS = "Background Canvas";
        
        #endregion

        public Action onPause = null;
        
        
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

        public GameObject OpenPopupUI(string uiName) => OpenPopupUI(uiName, false);
        public GameObject OpenPopupUI(string uiName, bool pauseOnUI)
        {
            GameObject uiObj = OpenUI(uiName, _order);
            if (uiObj == null) return null;
            
            _order++;
            var obj = uiObj.GetComponentInChildren<Button>().gameObject;
            EventSystem.current.SetSelectedGameObject(obj);
            _popupStack.Push(uiObj);
            
            if(pauseOnUI) Pause();

            KeyboardInputManager.Instance.SetInputState(KeyboardInputManager.PLAYER, false);
            return uiObj;
        }

        public GameObject OpenBackgroundUI(string uiName)
        {
            GameObject uiObj = OpenUI(uiName, -1);
            if (uiObj == null) return null;
            
            _background?.SetActive(false);
            _background = uiObj;

            return uiObj;
        }

        public void ClosePopupUI()
        {
            if(_popupStack.Count==0) return;

            GameObject popup = _popupStack.Pop();
            popup.SetActive(false);

            if (_popupStack.Count >= 1)
            {
                var obj = _popupStack.Peek().GetComponentInChildren<Button>().gameObject;
                EventSystem.current.SetSelectedGameObject(obj);
            }

            _order--;

            if (_pause) Resume();

            KeyboardInputManager.Instance.SetInputState(KeyboardInputManager.PLAYER, _popupStack.Count == 0);
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

        // Pause 및 Resume 이 UI 로 종속되므로
        // Input System 을 설정하는 부분을 OpenPopupUI 와 ClosePopupUI 로 이동

        public void Pause()
        {
            _pause = true;
            Time.timeScale = 0;

            // Sound
            SoundManager.Instance.pauseSnapshot.TransitionTo(1f);
            SoundManager.Instance.PauseVoice();
            
            onPause?.Invoke();
        }

        public void Resume()
        {
            _pause = false;
            Time.timeScale = 1;

            // Sound
            SoundManager.Instance.Transition(1f);
            SoundManager.Instance.ResumeVoice();
        }
        
        #endregion
    }