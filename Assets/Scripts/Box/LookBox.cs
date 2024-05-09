using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utilities;

public class LookBox : MonoBehaviour
{
    [SerializeField] Vector3 _offset;
    [SerializeField] float _time;
    [SerializeField] bool __resetAfterExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_time == 0) _time = 0.1f;
        if (collision.CompareTag("Player"))
        {
            this.Log("Look in TEST");
            CameraManager.Instance.Look(_offset, _time);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(__resetAfterExit && collision.CompareTag("Player"))
        {
            CameraManager.Instance.ResetCamera(_time);
        }
    }
}
