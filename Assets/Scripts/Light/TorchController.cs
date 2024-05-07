using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchController : MonoBehaviour
{
    [SerializeField] private float _lerpDuration;
    
    private Animator torchAnimator;
    private Light2D spotLight;
    private bool _isLit = false;

    private void Start()
    {
        torchAnimator = GetComponent<Animator>();
        spotLight = GetComponentInChildren<Light2D>();
    }

    public void FireTorch()
    {
        if (!_isLit)
        {
            torchAnimator.SetBool("IsLit", true);
            StartCoroutine(LerpIntensity(spotLight.intensity, 15f));
            _isLit = true;
        }
        else
        {
            torchAnimator.SetBool("IsLit", false);
            StartCoroutine(LerpIntensity(spotLight.intensity, 0f));
            _isLit = false;
        }
    }
    private IEnumerator LerpIntensity(float startValue, float endValue)
    {
        float elapsedTime = 0f;

        while (elapsedTime <_lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _lerpDuration);
            spotLight.intensity = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }

        spotLight.intensity = endValue;
    }
}