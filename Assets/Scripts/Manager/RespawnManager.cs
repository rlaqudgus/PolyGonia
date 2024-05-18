using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    private Vector3 _checkpointPosition;
    private int _currentCheckpointOrder = -1; // 초기값을 -1로 설정하여 체크포인트가 아직 설정되지 않았음을 나타냄


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
            Debug.Log("Checkpoint updated to order: " + checkpointOrder);
        }
        else
        {
            Debug.Log("Checkpoint order " + checkpointOrder + " is not higher than the current checkpoint order " + _currentCheckpointOrder);
        }
    }
    public void UpdateCheckpoint(Checkpoint checkpoint)
    {
        SetCheckpoint(checkpoint.transform.position, checkpoint.checkpointOrder);
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

    // 플레이어가 죽었을 때 리스폰하는 메서드 예시
    public void OnPlayerDeath(GameObject player)
    {
        if (Instance != null)
        {
            UnityEngine.Debug.Log("RespawnManager instance is not null!");
            RespawnPlayer(player);
        }
        else
        {
            // 새로운 인스턴스 생성은 여기에 포함되지 않습니다.
            // 대신, 인스턴스가 이미 존재하지 않으면 에러를 로그로 출력합니다.
            UnityEngine.Debug.Log("RespawnManager instance is null!");
        }
    }
}
