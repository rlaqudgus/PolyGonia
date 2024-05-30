using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : Singleton<CameraManager>
{
    private CameraController _controller;

    private void Awake()
    {
        CreateSingleton(this);
    }

    private void OnEnable()
    {
        GameManager.Instance.playerEvents.OnPlayerLowHealth += LowHpPostProcess;
    }

    private void OnDisable()
    {
        GameManager.Instance.playerEvents.OnPlayerLowHealth -= LowHpPostProcess;
    }

    protected override void Init()
    {
        if (_controller == null) _controller = GameObject.Find("Virtual Camera").GetComponent<CameraController>();
    }

    public void Zoom(float size, float time) => _controller.Zoom(size, time);
    public void Shake(float amount, float freq, float time) => _controller.Shake(amount, freq, time);
    public void Shake() => _controller.Shake(3f, 3f, .5f);
    public void Look(Vector3 offset, float time) => _controller.Look(offset, time);
    public void ResetCamera(float time) => _controller.ResetCamera(time);
    public void FollowTarget(GameObject target, float time) => _controller.FollowTarget(target, time);
    public void FollowTarget(GameObject target) => _controller.FollowTarget(target);

    [ContextMenu("hp1Effect")]
    public void LowHpPostProcess() => _controller.LowHpPostProcess(2, 1, .6f);
}