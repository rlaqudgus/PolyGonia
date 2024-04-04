using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    static CameraManager instance;
    public static CameraManager Instance 
    {
        get {
            return instance;
        }
    }
    private CameraController controller;

    private void Awake()
    {
        if (instance == null) instance = this;
        if (controller == null) controller = GameObject.Find("Virtual Camera").GetComponent<CameraController>();
    }

    public static void Zoom(float size, float time) => instance.controller.Zoom(size, time);
    public static void Shake(float amount, float freq, float time)=>instance.controller.Shake(amount, freq, time);
    public static void Shake() => instance.controller.Shake(5f, 5f, 1f);
    public static void Look(Vector3 offset, float time) => instance.controller.Look(offset, time);
    public static void ResetCamera(float time) => instance.controller.ResetCamera(time);
    public static void FollowTarget(GameObject target, float time) => instance.controller.FollowTarget(target, time);
}