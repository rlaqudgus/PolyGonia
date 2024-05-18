using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointOrder;

    [SerializeField] private bool isRendererEnabled = false; // 인스펙터에서 제어할 수 있는 체크박스
    private Renderer checkpointRenderer;

    private void Start()
    {
        // Renderer 컴포넌트를 가져오기
        checkpointRenderer = GetComponent<Renderer>();

        // Renderer 컴포넌트를 초기화된 값으로 설정
        if (checkpointRenderer != null)
        {
            checkpointRenderer.enabled = isRendererEnabled;
        }
    }
    private void OnValidate()
    {
        // 인스펙터에서 값이 변경될 때 Renderer 컴포넌트를 업데이트
        if (checkpointRenderer != null)
        {
            checkpointRenderer.enabled = isRendererEnabled;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            // 플레이어가 체크포인트에 도달했을 때의 동작
            RespawnManager.Instance.UpdateCheckpoint(this);
            Debug.Log("Checkpoint reached at position: " + transform.position);
        }
    }
}
