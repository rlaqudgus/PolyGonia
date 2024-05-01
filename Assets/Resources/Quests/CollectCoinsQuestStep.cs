using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    private int _coinsCollected  = 0;
    private int _coinsToComplete = 3;

    private void OnEnable()
    {
        GameManager.Instance.miscEvents.OnCoinCollected += CoinCollected;
    }

    private void OnDisable()
    {
        GameManager.Instance.miscEvents.OnCoinCollected -= CoinCollected;
    }

    private void CoinCollected()
    {
        if (_coinsCollected < _coinsToComplete)
        {
            _coinsCollected++;
        }

        if (_coinsCollected >= _coinsToComplete) 
        {
            FinishQuestStep();
        }
    }
}
