using UnityEngine;
using Utilities;
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    private Vector3 _checkpointPosition;
    private int _currentCheckpointOrder = -1; // �ʱⰪ�� -1�� �����Ͽ� üũ����Ʈ�� ���� �������� �ʾ����� ��Ÿ��


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

    public void SetCheckpoint(Vector3 checkpointPosition, int checkpointOrder)
    {
        if (checkpointOrder >= _currentCheckpointOrder)
        {
            _checkpointPosition = checkpointPosition;
            _currentCheckpointOrder = checkpointOrder;
            this.Log("Checkpoint updated to order: " + checkpointOrder);
        }
        else
        {
            this.Log("Checkpoint order " + checkpointOrder + " is not higher than the current checkpoint order " + _currentCheckpointOrder);
        }
    }
    public void UpdateCheckpoint(CheckpointBox checkpoint)
    {
        SetCheckpoint(checkpoint.transform.position, checkpoint.checkpointOrder);
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = _checkpointPosition;

        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus != null)
        {
            this.Log("HP reset");
            playerStatus.ResetHealth();
        }
    }

    // �÷��̾ �׾��� �� �������ϴ� �޼��� ����
    public void OnPlayerDeath(GameObject player)
    {
        if (Instance != null)
        {
            this.Log("RespawnManager instance is not null!");
            RespawnPlayer(player);
        }
        else
        {
            // ���ο� �ν��Ͻ� ������ ���⿡ ���Ե��� �ʽ��ϴ�.
            // ���, �ν��Ͻ��� �̹� �������� ������ ������ �α׷� ����մϴ�.
            this.Log("RespawnManager instance is null!");
        }
    }
}
