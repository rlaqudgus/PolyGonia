using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointOrder;

    private void Start()
    {
        // 게임 시작 시 Renderer 컴포넌트를 비활성화
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 체크포인트에 도달했을 때의 동작
            RespawnManager.Instance.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint reached at position: " + transform.position);
        }
    }
}
