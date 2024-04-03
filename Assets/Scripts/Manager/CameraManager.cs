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
        if (controller == null) controller = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    public static void Zoom(float size, float time) => instance.controller.Zoom(size, time);
    public static void Shake()=>instance.controller.Shake();
    public static void Look(Vector3 offset, float time) => instance.controller.Look(offset, time);
    public static void ResetCamera(float time) => instance.controller.ResetCamera(time);
}