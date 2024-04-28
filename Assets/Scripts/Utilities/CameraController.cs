using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera _virtualCamera;
    CinemachineTransposer _transposer;
    CinemachineBasicMultiChannelPerlin _multiChannelPerlin;
    CinemachineConfiner2D _confiner2D;

    [SerializeField] Vector3 _startOffset;
    [SerializeField] float _cameraDamping;

    Vector3 defaultOffset;
    float defaultSize;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>(); //offset
        _multiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(); //shake
        _confiner2D = GetComponent<CinemachineConfiner2D>();

        defaultOffset = _startOffset + new Vector3(0, 0, -10);
        defaultSize = _virtualCamera.m_Lens.OrthographicSize;

        _transposer.m_FollowOffset = defaultOffset;
        _transposer.m_XDamping = _cameraDamping;
        _transposer.m_YDamping = _cameraDamping;
    }

    public void Shake(float amount, float freq, float time) => StartCoroutine(IEShake(amount, freq, time));
    public void Zoom(float size, float time) => StartCoroutine(IEZoom(size, time));
    public void Look(Vector3 offset, float time) => StartCoroutine(IELook(offset, time));
    public void ResetCamera(float time) => StartCoroutine(IEReset(time));
    public void FollowTarget(GameObject target, float time) => StartCoroutine(IEFollowTarget(target, time));
    public void FollowTarget(GameObject target) => _virtualCamera.m_Follow = target.transform;

    private IEnumerator IEShake(float amount, float freq, float time)
    {

        _multiChannelPerlin.m_AmplitudeGain = amount;
        _multiChannelPerlin.m_FrequencyGain = freq;

        yield return new WaitForSeconds(time);
        
        _multiChannelPerlin.m_AmplitudeGain = 0f;
        _multiChannelPerlin.m_FrequencyGain = 0f;
    }

    private IEnumerator IEZoom(float size, float time) // Ȯ�� �� confiner �ʰ��ϴ� ���� �߻�
    {
        float elapsedTime = 0f;
        float startSize = _virtualCamera.m_Lens.OrthographicSize;
        float targetSize = size;

        while (elapsedTime < time)
        {

            _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _virtualCamera.m_Lens.OrthographicSize = targetSize;
    }


    private IEnumerator IELook(Vector3 targetOffset, float time)
    {
        float elapsedTime = 0f;
        Vector3 startOffset = _transposer.m_FollowOffset;
        targetOffset += new Vector3(0, 0, -10);

        while (elapsedTime < time)
        {
            _transposer.m_FollowOffset = Vector3.Lerp(startOffset, targetOffset, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _transposer.m_FollowOffset = targetOffset;
    }
    private IEnumerator IEReset(float time)
    {
        float elapsedTime = 0f;
        Vector3 startOffset = _transposer.m_FollowOffset;
        Vector3 targetOffset = defaultOffset;
        float startSize = _virtualCamera.m_Lens.OrthographicSize;
        float targetSize = defaultSize;


        while (elapsedTime < time)
        {
            _transposer.m_FollowOffset = Vector3.Lerp(startOffset, targetOffset, elapsedTime / time);
            _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _virtualCamera.m_Lens.OrthographicSize = targetSize;
        _transposer.m_FollowOffset = targetOffset;
    }

    private IEnumerator IEFollowTarget(GameObject target, float time)
    {
        _virtualCamera.m_Follow = target.transform;

        yield return new WaitForSeconds(time);

        _virtualCamera.m_Follow = GameObject.Find("Player").transform;
    }
}
