using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class FollowBox : MonoBehaviour
{
    [SerializeField] GameObject _target;
    [SerializeField] float _time;
    [SerializeField] bool _resetAfterExit; //camera가 target 영구적으로 따라갈지 time만큼 따라갈지 결정

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_time == 0) _time = 0.1f;
        if (collision.CompareTag("Player"))
        {
            if (_resetAfterExit) CameraManager.Instance.FollowTarget(_target);
            else CameraManager.Instance.FollowTarget(_target, _time);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_resetAfterExit && collision.CompareTag("Player"))
        {
            CameraManager.Instance.FollowTarget(collision.gameObject);
        }
    }
}
