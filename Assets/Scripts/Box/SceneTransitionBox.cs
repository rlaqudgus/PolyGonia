using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionBox : MonoBehaviour
{
    public string targetScene;  // ��ȯ�� �� �̸�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾�� �浹���� �� �� ��ȯ ����
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.LoadScene(targetScene);
        }
    }
}
