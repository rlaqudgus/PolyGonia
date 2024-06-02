using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utilities;

public class KeyboardInputManager : Singleton<KeyboardInputManager>
{
    public PlayerInput _playerInput;

    #region InputState

    public const string PLAYER = "Player";
    public const string UI = "UI";
    public const string GLOBAL = "Global";

    #endregion

    #region Action

    public Action<Vector2, int> MoveAction = null;
    public Action<float> LookAction = null;
    public Action JumpAction = null;
    public Action JumpUpAction = null;

    public Action<bool> ShieldAction = null;
    public Action AttackStartedAction = null;
    public Action AttackCanceledAction = null;

    public Action InteractAction = null;

    #endregion

    protected override void Init()
    {
        this.Log("init");
        _playerInput ??= GetComponent<PlayerInput>();
    }

    private void Awake()
    {
        CreateSingleton(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // SetInputState(GLOBAL, gameState != Init);
        SetInputState(GLOBAL, true);
    }

    public void UpdateInputState(string inputState)
    {
        if (_playerInput.currentActionMap.ToString() == inputState) return;
        _playerInput.SwitchCurrentActionMap(inputState);
    }

    public void SetInputState(string inputState, bool state)
    {
        InputActionMap actionMap = _playerInput.actions.FindActionMap(inputState);
        if (actionMap == null) Debug.LogWarning("There is no action map: " + name);

        if (actionMap.enabled ^ state)
        {
            if (state) actionMap.Enable();
            else       actionMap.Disable();
        }
    }

    #region MoveInput

    public void OnMove(InputAction.CallbackContext context)
    {
        var contextVec = context.ReadValue<Vector2>();
        int dir = 0;

        if (contextVec.x > Mathf.Epsilon) dir = 1;
        else if (contextVec.x < -Mathf.Epsilon) dir = -1;

        MoveAction?.Invoke(contextVec, dir);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 playerInput = context.ReadValue<Vector2>();

        LookAction?.Invoke(playerInput.y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                JumpAction?.Invoke();
                break;
            case InputActionPhase.Canceled:
                JumpUpAction?.Invoke();
                break;
        }
    }

    #endregion

    #region WeaponInput

    public void OnShield(InputAction.CallbackContext context)
    {
        bool isShield = context.ReadValueAsButton();

        ShieldAction?.Invoke(isShield);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                AttackStartedAction?.Invoke();
                break;

            case InputActionPhase.Canceled:
                AttackCanceledAction?.Invoke();
                break;
        }
    }

    #endregion

    #region OtherInput

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!GameManager.Instance.isPaused) GameManager.Instance.Pause();
            else GameManager.Instance.Resume();
        }
    }

    public void OnMap(InputAction.CallbackContext context)
    {
        if (context.started) UIManager.Instance.OpenPopupUI(UIManager.MAP_CANVAS);
    }

    public void OnInteract(InputAction.CallbackContext input)
    {
        if (input.started) InteractAction?.Invoke();
    }

    #endregion
}