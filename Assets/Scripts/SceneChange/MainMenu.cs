// UnityEngine.SceneManagement ���ӽ����̽��� �߰��ؾ� �մϴ�.
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
    // ��ư�� �Ҵ��� �޼���
    public void StartGame()
    {
        Debug.Log("SceneChange");
        SceneManager.LoadScene(3);  // "GameScene"�� ��ȯ�Ϸ��� ���� �̸�
        _mainButton.gameObject.SetActive(false);
    }
}
