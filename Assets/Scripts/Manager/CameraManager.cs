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
    }
    public void Zoom(float size, float time) => controller.Zoom(size, time);
    public void Shake(float amount, float freq, float time)=>controller.Shake(amount, freq, time);
    public void Shake() => controller.Shake(3f, 3f, .5f);
    public void Look(Vector3 offset, float time) => controller.Look(offset, time);
    public void ResetCamera(float time) => controller.ResetCamera(time);
    public void FollowTarget(GameObject target, float time) => controller.FollowTarget(target, time);
    public void FollowTarget(GameObject target) => controller.FollowTarget(target);
}