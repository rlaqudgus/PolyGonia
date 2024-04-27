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

    public static void Zoom(float size, float time) => Instance.controller.Zoom(size, time);
    public static void Shake(float amount, float freq, float time)=>Instance.controller.Shake(amount, freq, time);
    public static void Shake() => Instance.controller.Shake(3f, 3f, .5f);
    public static void Look(Vector3 offset, float time) => Instance.controller.Look(offset, time);
    public static void ResetCamera(float time) => Instance.controller.ResetCamera(time);
    public static void FollowTarget(GameObject target, float time) => Instance.controller.FollowTarget(target, time);
    public static void FollowTarget(GameObject target) => Instance.controller.FollowTarget(target);
}