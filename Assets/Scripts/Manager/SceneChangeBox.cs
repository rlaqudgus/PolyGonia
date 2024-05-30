using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeBox : MonoBehaviour
{
    public int targetScene;  // ��ȯ�� �� �̸�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾�� �浹���� �� �� ��ȯ ����
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.LoadScene(targetScene);

            //GameManager.Instance.UpdateGameState(GameState.Init);
        }
    }

    
}
