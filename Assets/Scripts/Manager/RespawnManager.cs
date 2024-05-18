using System.Diagnostics;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    private Vector3 _checkpointPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        _checkpointPosition = checkpointPosition;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = _checkpointPosition;

        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus != null)
        {
            UnityEngine.Debug.Log("HP reset");
            playerStatus.ResetHealth();
        }
    }

    // �÷��̾ �׾��� �� �������ϴ� �޼��� ����
    public void OnPlayerDeath(GameObject player)
    {
        if (Instance != null)
        {
            UnityEngine.Debug.Log("RespawnManager instance is not null!");
            RespawnPlayer(player);
        }
        else
        {
            // ���ο� �ν��Ͻ� ������ ���⿡ ���Ե��� �ʽ��ϴ�.
            // ���, �ν��Ͻ��� �̹� �������� ������ ������ �α׷� ����մϴ�.
            UnityEngine.Debug.Log("RespawnManager instance is null!");
        }
    }
}
