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
        
        public GameObject OpenPopupUI(string uiName)
        {
            GameObject uiObj = OpenUI(uiName, _order);
            if (uiObj == null) return null;
            
            _order++;
            var obj = uiObj.GetComponentInChildren<Button>().gameObject;
            EventSystem.current.SetSelectedGameObject(obj);
            _popupStack.Push(uiObj);
            
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

        public void Pause() => GameManager.Instance.Pause();
        public void Resume() => GameManager.Instance.Resume();
    }