using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class FollowBox : MonoBehaviour
{
    [SerializeField] GameObject _target;
    [SerializeField] float _time;
    [SerializeField] bool _changeOnBox;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_time == 0) _time = 0.1f;
        if (collision.CompareTag("Player"))
        {
            if (_changeOnBox) CameraManager.FollowTarget(_target);
            else CameraManager.FollowTarget(_target, _time);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_changeOnBox && collision.CompareTag("Player"))
        {
            CameraManager.FollowTarget(collision.gameObject);
        }
    }
}
