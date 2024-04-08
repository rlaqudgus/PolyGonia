using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class FollowBox : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float time;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (time == 0) time = 0.1f;
        if (collision.CompareTag("Player"))
        {
            this.Log("Look in TEST");
            CameraManager.FollowTarget(target, time);
        }
    }
}
