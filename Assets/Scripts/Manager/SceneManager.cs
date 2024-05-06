using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;  // ��ȯ�� �� �̸�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾�� �浹���� �� �� ��ȯ ����
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetScene);

            //GameManager.Instance.UpdateGameState(GameState.Init);
        }
    }
}
