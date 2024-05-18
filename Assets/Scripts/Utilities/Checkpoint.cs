using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointOrder;

    private void Start()
    {
        // ���� ���� �� Renderer ������Ʈ�� ��Ȱ��ȭ
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
            // �÷��̾ üũ����Ʈ�� �������� ���� ����
            RespawnManager.Instance.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint reached at position: " + transform.position);
        }
    }
}
