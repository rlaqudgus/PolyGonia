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
        //�÷��̾��� ü���� �ٲ𶧸��� ȣ���ؾ��ϴ� �͵��� ������ ���
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
