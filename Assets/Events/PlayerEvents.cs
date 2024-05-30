using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents
{
    public event Action<int> OnPlayerLevelChange;

    public event Action<int> OnPlayerHealthChange;

    public event Action OnPlayerLowHealth;

    public void PlayerLevelChange(int level)
    {
        if (OnPlayerLevelChange != null)
        {
            OnPlayerLevelChange(level);
        }
    }

    public void PlayerHealthChange(int health) 
    {
        //플레이어의 체력이 바뀔때마다 호출해야하는 것들이 있으면 사용
        //if (OnPlayerHealthChange == null) return;
        //OnPlayerHealthChange(health);
        Debug.Log("PlayerHealth Changed");
        if (health <= 1)
        {
            Debug.Log("Health Low");
            OnPlayerLowHealth();
        }
    }
}
