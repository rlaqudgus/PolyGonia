using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;

// Unity Audio Manager Tutorial
// https://www.youtube.com/watch?v=rdX7nhH6jdM - Structure
// https://www.youtube.com/watch?v=KJfzT1VfOaM - Audio Mixer

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public  static SoundManager  Instance { get { return _instance; } }

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource, voiceSource;
    public AudioMixer mixer;

    private float _muteVolume;
    private float _sfxVolume, _musicVolume, _voiceVolume;
    private bool _isMusicMuted, _isSFXMuted, _isVoiceMuted;
    public  bool  isVoiceMuted { get { return _isVoiceMuted; } }

    private float _savedVoiceTime;

    [HideInInspector] 
    public List<SnapshotInfo> snapshotInfoList = new List<SnapshotInfo>();

    public AudioMixerSnapshot rawSnapshot;
    public AudioMixerSnapshot pauseSnapshot;

    private Dictionary<string, AudioClip> _musicSoundDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _sfxSoundDict   = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            // SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {   
        foreach (Sound sound in musicSounds) { _musicSoundDict.Add(sound.name, sound.clip); }
        foreach (Sound sound in sfxSounds)   { _sfxSoundDict.Add(sound.name, sound.clip)  ; }

        musicSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        sfxSource.outputAudioMixerGroup   = mixer.FindMatchingGroups("SFX")[0];
        voiceSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Voice")[0];

        _muteVolume = 1e-4f;
        _isMusicMuted = false;
        _isSFXMuted   = false;
        _isVoiceMuted = false;
    }

    public AudioClip GetMusicClip(string name) { return _musicSoundDict[name]; }
    public AudioClip GetSFXClip(string name)   { return _sfxSoundDict[name]  ; }
    
    public void PlayMusic(string name)
    {
        if (musicSource.isPlaying) StopMusic();

        AudioClip clip = _musicSoundDict[name];
        musicSource.clip = clip;
        musicSource.loop = true;

        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource.clip == null) return;
        musicSource.Stop();
    }

    /**
     * using UnityEngine.SceneManagement;
     *
     * private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
     * {
     *    PlayMusic(arg0.name);
     * }
     */

    public void PlaySFX(string name)
    {
        AudioClip clip = _sfxSoundDict[name];
        sfxSource.clip = clip;
        sfxSource.PlayOneShot(clip);
    }

    // [SH] [2024-05-01]
    // Voice 채널 분리
    // Music, SFX 와 모두 다른 특성을 가지고 있다고 판단
    // AudioClip 은 Dialogue 시 Sentence 클래스에서 바로 가져옴

    public void PlayVoice(AudioClip clip)
    {
        if (clip == null) return;
        if (voiceSource.isPlaying) StopVoice();

        voiceSource.clip = clip;
        voiceSource.loop = false;

        voiceSource.Play();
    }

    public void StopVoice()
    {
        if (voiceSource.clip == null) return;

        voiceSource.Stop();
        voiceSource.clip = null;
    }

    public void PauseVoice()
    {
        if (voiceSource.clip == null) return;

        // Voice가 재생 중이면 현재 재생 위치를 저장
        // Voice의 재생이 모두 끝난 이후에 Pause를 하고 Resume을 했을 때 다시 재생되는 것을 방지
        if (voiceSource.isPlaying) _savedVoiceTime = voiceSource.time;
        else _savedVoiceTime = voiceSource.clip.length;

        voiceSource.Stop();
    }

    public void ResumeVoice()
    {
        if (voiceSource.clip == null) return;
        voiceSource.time = _savedVoiceTime;
        voiceSource.Play();
    }

    public void Transition(float timeToReach)
    {
        // snapshot.TransitionTo(timeToReach);
        // AudioMixer.TransitionToSnapshots(snapshots, weights, timeToReach);

        int n_snapshot = snapshotInfoList.Count;

        if (n_snapshot == 0)
        {
            rawSnapshot.TransitionTo(timeToReach);
            return;
        }

        AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[n_snapshot];
        float[] weights = new float[n_snapshot];
        for (int i = 0; i < n_snapshot; ++i)
        {
            snapshots[i] = snapshotInfoList[i].snapshot;
            weights[i]   = snapshotInfoList[i].weight;
        }
        
        mixer.TransitionToSnapshots(snapshots, weights, timeToReach);

        return;
    }

    #region Volume

    private float convertToDecibel(float value)
    {
        value = Mathf.Max(value, 0.0001f);
        return 20f * Mathf.Log10(value);
    }

    public void SetMusicVolume(float value)
    {
        _musicVolume = value;
        if (_isMusicMuted) return;
        mixer.SetFloat("Music Volume", convertToDecibel(value));
    }

    public void SetSFXVolume(float value)
    {
        _sfxVolume = value;
        if (_isSFXMuted) return;
        mixer.SetFloat("SFX Volume", convertToDecibel(value));
    }

    public void SetVoiceVolume(float value)
    {
        _voiceVolume = value;
        if (_isVoiceMuted) return;
        mixer.SetFloat("Voice Volume", convertToDecibel(value));
    }

    #endregion

    #region Mute

    private void MuteMusic()
    {
        Debug.Assert(!_isMusicMuted, "Music is trying to be muted while it is already muted");
        
        _isMusicMuted = true;
        mixer.SetFloat("Music Volume", convertToDecibel(_muteVolume));
    }

    private void MuteSFX()
    {
        Debug.Assert(!_isSFXMuted, "SFX is trying to be muted while it is already muted");

        _isSFXMuted = true;
        mixer.SetFloat("SFX Volume", convertToDecibel(_muteVolume));
    }

    private void MuteVoice()
    {
        Debug.Assert(!_isVoiceMuted, "Voice is trying to be muted while it is already muted");

        _isVoiceMuted = true;
        mixer.SetFloat("Voice Volume", convertToDecibel(_muteVolume));
    }

    private void UnmuteMusic()
    {
        Debug.Assert(_isMusicMuted, "Music is trying to be unmuted while it is already unmuted");

        _isMusicMuted = false;
        mixer.SetFloat("Music Volume", convertToDecibel(_musicVolume));
    }

    private void UnmuteSFX()
    {
        Debug.Assert(_isSFXMuted, "SFX is trying to be unmuted while it is already unmuted");

        _isSFXMuted = false;
        mixer.SetFloat("SFX Volume", convertToDecibel(_sfxVolume));
    }

    private void UnmuteVoice()
    {
        Debug.Assert(_isVoiceMuted, "Voice is trying to be unmuted while it is already unmuted");

        _isVoiceMuted = false;
        mixer.SetFloat("Voice Volume", convertToDecibel(_voiceVolume));
    }

    public void ToggleMusic()
    {
        if (!_isMusicMuted) MuteMusic();
        else UnmuteMusic();
    }

    public void ToggleSFX()
    {
        if (!_isSFXMuted) MuteSFX();
        else UnmuteSFX();
    }

    public void ToggleVoice()
    {
        if (!_isVoiceMuted) MuteVoice();
        else UnmuteVoice();
    }

    #endregion

}
