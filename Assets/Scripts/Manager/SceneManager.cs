using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;  // 전환할 씬 이름

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했을 때 씬 전환 실행
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetScene);

            //GameManager.Instance.UpdateGameState(GameState.Init);
        }
    }
}
