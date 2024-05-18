using UnityEngine;

public class DeathZoneBox : MonoBehaviour
{
    private Renderer spriteRenderer;

    private void Start()
    {
        // Renderer 컴포넌트를 가져와서 비활성화
        spriteRenderer = GetComponent<Renderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.name == "Player"))
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            
            if (playerStatus != null)
            {
                Debug.Log("dd");
                playerStatus.PlayerDied();
            }
        }
    }
}
