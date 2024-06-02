using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int maxHealth = 5;
    [SerializeField] public int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
       
        Debug.Log($"currentHp : {currentHealth} - {dmg} = {currentHealth - dmg}");
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            PlayerDied();
        }
    }
    

    public void PlayerDied()
    {
        RespawnManager.Instance.OnPlayerDeath(gameObject);
    }
}
