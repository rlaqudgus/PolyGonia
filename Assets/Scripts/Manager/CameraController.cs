using System;
using System.Collections;
using System.Drawing;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    GameObject target;
    Vector3 cameraPos;
    [SerializeField] Vector3 offset;
    [SerializeField] float cameraSpeed;

    float height;
    float width;

    Vector3 defaultOffset;
    float defaultSize;

    public float shakeAmount = 1.0f;
    public float shakeTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        target= GameObject.Find("Player");
        cam = this.GetComponent<Camera>();

        defaultOffset = offset;
        defaultSize = cam.orthographicSize;
        
        height = cam.orthographicSize;
        width = height * Screen.width / Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        //z°ªÀº ³öµÎ±â
        cameraPos = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        cameraPos += offset;
        transform.position = Vector3.Lerp(transform.position, cameraPos, Time.deltaTime * cameraSpeed);

        Debug.DrawRay(transform.position + new Vector3(width, 0, 0), transform.forward * 10f);
        RaycastHit hit;
        if(Physics.Raycast(transform.position + new Vector3(width, 0, 0), transform.forward, out hit, 20))
        {
            if(hit.transform.name == "WALL")
            {

            }
        }
    }

    public void Shake() => StartCoroutine(IEShake());
    public void Zoom(float size, float time) => StartCoroutine(IEZoom(size, time));
    public void Look(Vector3 offset, float time)=>StartCoroutine(IELook(offset, time));
    public void ResetCamera(float time)=>StartCoroutine(IEReset(time));

    private IEnumerator IEShake()
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
    private IEnumerator IEZoom(float size, float time)
    {
        float elapsedTime = 0f;
        float startSize = cam.orthographicSize;
        float targetSize = size;

        while (elapsedTime < time)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        cam.orthographicSize = targetSize;
    }
    private IEnumerator IELook(Vector3 newOffset, float time)
    {
        float elapsedTime = 0f;
        Vector3 startOffset = this.offset;
        Vector3 targetOffset = newOffset;

        while (elapsedTime < time)
        {
            this.offset = Vector3.Lerp(startOffset, targetOffset, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        this.offset = targetOffset;
    }
    private IEnumerator IEReset(float time)
    {
        float elapsedTime = 0f;
        Vector3 startOffset = this.offset;
        Vector3 targetOffset = defaultOffset;
        float startSize = cam.orthographicSize;
        float targetSize = defaultSize;


        while (elapsedTime < time)
        {
            this.offset = Vector3.Lerp(startOffset, targetOffset, elapsedTime / time);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        cam.orthographicSize = targetSize;
        this.offset = targetOffset;
    }
}
