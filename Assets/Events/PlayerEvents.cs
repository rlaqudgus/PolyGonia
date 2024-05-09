using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents
{
    public event Action<int> OnPlayerLevelChange;

    public void PlayerLevelChange(int level)
    {
        if (OnPlayerLevelChange != null)
        {
            OnPlayerLevelChange(level);
        }
    }
}
