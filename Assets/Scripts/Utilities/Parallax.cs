using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Parallax : MonoBehaviour
{
    public Camera cam;

    public Transform subject;

    Vector2 startPos;
    float startZ;
    float distanceFromSubject => transform.position.z - subject.position.z;
    float clippingPlane => cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane);
    Vector2 travel => (Vector2)cam.transform.position - startPos;
    float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startZ = transform.position.z;
    }

    
    void FixedUpdate()
    {
        Vector2 paralVec = startPos + travel * parallaxFactor;
        transform.position = new Vector3(paralVec.x, transform.position.y, startZ);
    }
}
