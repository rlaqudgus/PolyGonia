using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;

// AudioMixer Snapshots - Unity Official Tutorials
// https://www.youtube.com/watch?v=2nYyws0qJOM

// MixerBox와 Trigger 시 새로운 Effect를 적용하는 방법
// Tutorial에서는 Snapshot 교체를 통해 동적으로 믹싱을 변경하고 있다
// 그러나 이 방법은 Trigger 영역이 겹쳤을 때 문제가 생길 수 있다

// 기본적으로 OnTriggerEnter / OnTriggerExit 시에 snapshot을 교체하는 로직이 아닌 
// Effect를 Add 또는 Remove 하는 식으로 구현하고 싶었다
// 그런데 Effect를 동적으로 Add 또는 Remove하는 방법을 찾을 수가 없었다
// 에디터에서 ByPass를 설정할 수는 있는데 이게 동적으로 스크립트로 제어 가능한 것 같지는 않다
// 그리고 이를 제어하면 모든 snapshot에 공통적으로 적용되었으므로 사용할 수 없었다

// 그리고 Snapshot 교체 로직으로 구현했다 해도 항상 Effect를 적용한 채로 있어야 하는 부작용이 있다
// 그저 그 값을 0으로 해서 적용되지 않는 것처럼 보이게 하는 것이므로 리소스 낭비가 될 수 있다

// 그래도 전역적으로 Snapshot을 List로 관리하면서 AudioMixer.TransitionToSnapshots을 사용하는 방법을 떠올려 냈다
// Trigger 영역이 겹쳤을 때 Multiply의 느낌으로 Effect를 동적으로 처리할 수 있다
// 다만 이는 interpolation을 이용한 처리이므로 Add를 하는 것과는 느낌이 조금 다르다

public class MixerBox : MonoBehaviour
{
    // SoundManager.Instance.mixer를 사용하고 있다고 가정
    // collision이 일어날 때 mixer의 snapshot을 변경

    // SnapshotInfo 클래스는 SoundManager가 위치한 폴더의 SnapshotInfo 스크립트에 선언되어 있다 
    // AudioMixerSnapshot snapshot 과 float weight 로 구성된다
    public SnapshotInfo snapshotInformation;
    public float timeToReach;
    public bool resetAfterExit;

    private bool _isMixed;

    void Start()
    {
        _isMixed = false;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (_isMixed) return;
        if (other.gameObject.GetComponent<AudioListener>() == null) return;
        _isMixed = true;
        
        SoundManager.Instance.snapshotInfoList.Add(snapshotInformation);
        SoundManager.Instance.Transition(timeToReach);
        
        // this.Log($"{snapshotInformation.snapshot.name} is working!");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!resetAfterExit) return;
        if (!_isMixed) return;
        if (other.gameObject.GetComponent<AudioListener>() == null) return;
        _isMixed = false;

        SoundManager.Instance.snapshotInfoList.Remove(snapshotInformation);
        SoundManager.Instance.Transition(timeToReach);
        
        // this.Log($"{snapshotInformation.snapshot.name} is removed!");
    }
}
