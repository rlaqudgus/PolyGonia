using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointOrder;

    [SerializeField] private bool isRendererEnabled = false; // �ν����Ϳ��� ������ �� �ִ� üũ�ڽ�
    private Renderer checkpointRenderer;

    private void Start()
    {
        // Renderer ������Ʈ�� ��������
        checkpointRenderer = GetComponent<Renderer>();

        // Renderer ������Ʈ�� �ʱ�ȭ�� ������ ����
        if (checkpointRenderer != null)
        {
            checkpointRenderer.enabled = isRendererEnabled;
        }
    }
    private void OnValidate()
    {
        // �ν����Ϳ��� ���� ����� �� Renderer ������Ʈ�� ������Ʈ
        if (checkpointRenderer != null)
        {
            checkpointRenderer.enabled = isRendererEnabled;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            // �÷��̾ üũ����Ʈ�� �������� ���� ����
            RespawnManager.Instance.UpdateCheckpoint(this);
            Debug.Log("Checkpoint reached at position: " + transform.position);
        }
    }
}
