using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscEvents
{
    public event Action OnCoinCollected;

    public void CoinCollected()
    {
        if (OnCoinCollected != null)
        {
            OnCoinCollected();
        }
    }   
}
