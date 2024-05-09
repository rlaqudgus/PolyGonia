using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utilities;

public class ZoomBox : MonoBehaviour
{
    [SerializeField] float _size;
    [SerializeField] float _time;
    [SerializeField] bool _resetAfterExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_time == 0) _time = 0.1f;
        if (_size == 0) _size = 0.1f;
        if (collision.CompareTag("Player"))
        {
            CameraManager.Instance.Zoom(_size, _time);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_resetAfterExit && collision.CompareTag("Player"))
        {
            CameraManager.Instance.ResetCamera(_time);
        }
    }
}
