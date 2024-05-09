using System;
using Bases;
using UnityEngine.InputSystem;

namespace Manager
{
    public class InputManager : Singleton<InputManager>
    {
        private PlayerInput _playerInput;

        #region InputState

        public const string PLAYER = "Player";
        public const string UI = "UI";

        #endregion
        
        private void Awake()
        {
            CreateSingleton(this);
        }

        protected override void Init() => _playerInput ??= GetComponent<PlayerInput>();

        public void UpdateInputState(string inputState)
        {
            if (_playerInput.currentActionMap.ToString() == inputState) return;
            _playerInput.SwitchCurrentActionMap(inputState);
        }
    }
}