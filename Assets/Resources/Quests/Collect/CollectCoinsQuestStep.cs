using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    [SerializeField] private int _coinsCollected;
    [SerializeField] private int _coinsToComplete;

    private void OnEnable()
    {
        GameManager.Instance.miscEvents.OnItemCollected += CoinCollected;
    }

    private void OnDisable()
    {
        GameManager.Instance.miscEvents.OnItemCollected -= CoinCollected;
    }

    private void Start()
    {
        _coinsCollected = 0;
    }

    private void CoinCollected(Item item)
    {
        if (item.itemType != ItemType.Coin) return;
        
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
