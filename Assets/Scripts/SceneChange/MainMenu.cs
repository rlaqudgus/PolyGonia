// UnityEngine.SceneManagement 네임스페이스를 추가해야 합니다.
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using Utilities;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    public GameObject _mainButton;
    // 버튼에 할당할 메서드
    public void StartGame()
    {
        Debug.Log("SceneChange");
        SceneManager.LoadScene(3);  // "GameScene"은 전환하려는 씬의 이름
        _mainButton.gameObject.SetActive(false);
    }
}
