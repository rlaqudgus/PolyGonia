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
    public AudioSource musicSource, sfxSource;
    public AudioMixer mixer;
    
    [HideInInspector] 
    public List<SnapshotInfo> snapshotInfoList = new List<SnapshotInfo>();

    private AudioMixerSnapshot _rawSnapshot;

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
        _rawSnapshot = mixer.FindSnapshot("Default");
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

    public void Transition(float timeToReach)
    {
        // snapshot.TransitionTo(timeToReach);
        // AudioMixer.TransitionToSnapshots(snapshots, weights, timeToReach);

        int n_snapshot = snapshotInfoList.Count;

        if (n_snapshot == 0)
        {
            _rawSnapshot.TransitionTo(timeToReach);
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
}
