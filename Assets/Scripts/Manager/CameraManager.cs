using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : Singleton<CameraManager>
{
    private CameraController controller;

    private void Awake()
    {
        CreateSingleton(this);
        if (controller == null) controller = GameObject.Find("Virtual Camera").GetComponent<CameraController>();
        GameManager.OnGameStateChanged += CameraByGameState;
    }
    public void Zoom(float size, float time) => controller.Zoom(size, time);
    public void Shake(float amount, float freq, float time)=>controller.Shake(amount, freq, time);
    public void Shake() => controller.Shake(3f, 3f, .5f);
    public void Look(Vector3 offset, float time) => controller.Look(offset, time);
    public void ResetCamera(float time) => controller.ResetCamera(time);
    public void FollowTarget(GameObject target, float time) => controller.FollowTarget(target, time);
    public void FollowTarget(GameObject target) => controller.FollowTarget(target);
    
    [ContextMenu("hp1Effect")]
    public void LowHPPostProcess(float time)
    {
        if (controller == null) controller = GameObject.Find("Virtual Camera").GetComponent<CameraController>();
        controller.LowHpPostProcess(time, 1, .4f);
    }
    

    private void CameraByGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Init:
                break;
            case GameState.LowHealth:
                LowHPPostProcess(3f);
                break;
        }

    }
}