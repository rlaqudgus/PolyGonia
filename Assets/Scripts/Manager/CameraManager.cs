using System.Collections;
using System.Collections.Generic;
using Bases;
using UnityEngine;
using Utilities;

namespace Manager
{
    public class CameraManager : Singleton<CameraManager>
    {
        private CameraController _controller;

        private void Awake()
        {
            CreateSingleton(this);
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
        public void LowHpPostProcess(float time) => _controller.LowHpPostProcess(time, 1, .4f);
    }
}