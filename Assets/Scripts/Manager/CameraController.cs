using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject target;
    Vector3 cameraPos;
    [SerializeField] Vector3 offset;

    public float shakeAmount = 1.0f;
    public float shakeTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        target= GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //z°ªÀº ³öµÎ±â
        cameraPos = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        transform.position = cameraPos + offset;
    }

    public IEnumerator Shake()
    {
        float timer = 0;
        while (timer <= shakeTime)
        {
            transform.position =
                transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * shakeAmount;
            timer += Time.deltaTime;
            yield return null;
        }

        
       
    }
}
