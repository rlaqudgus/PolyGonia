using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;

public class MusicBox : MonoBehaviour
{
    public string musicName;
    private AudioSource _musicSource;

    // Sprite는 Circle을 가정한다
    // Sprite의 크기에 맞춰 Max Distance가 설정되도록 설계했다
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _musicSource = GetComponent<AudioSource>();
        if (_musicSource.outputAudioMixerGroup == null)
        {
            AudioMixerGroup group = SoundManager.Instance.mixer.FindMatchingGroups("Music")[0];
            _musicSource.outputAudioMixerGroup = group;
        }

        // sprite가 타원이 아닌 원이 되도록 강제
        // 반지름은 scale의 절반
        float tau = Mathf.Sqrt(transform.localScale.x * transform.localScale.y);
        float radius = tau * 0.5f;
        transform.localScale = new Vector3(tau, tau, 1.0f);
        _musicSource.maxDistance = radius;

        // Audio Source의 z값은 Audio Listener의 z 값과 다르도록 설정한다
        // 둘이 동일하면 panning이 조금 부자연스럽다
        // z값의 차이의 절댓값이 너무 크면 Circle의 크기와 AudioSource의 maxDistance가 일치하지 않는다
        // 이는 maxDistance가 z값도 고려하여 계산되기 때문이므로 z값은 적당한 크기로 조절한다
        transform.position = new Vector3(transform.position.x, transform.position.y, -radius * 0.2f);

        // 음악 재생
        _musicSource.clip = SoundManager.Instance.GetMusicClip(musicName);
        _musicSource.loop = true;
        _musicSource.Play();
    }
}
