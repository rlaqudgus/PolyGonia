using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class CollectItemsQuestStep : QuestStep
{
    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _itemsCollected;
    [SerializeField] private int _itemsToComplete;

    private void OnEnable()
    {
        GameManager.Instance.miscEvents.OnItemCollected += ItemCollected;
    }

    private void OnDisable()
    {
        GameManager.Instance.miscEvents.OnItemCollected -= ItemCollected;
    }

    private void Start()
    {
        _itemsCollected = 0;   
    }

    private void ItemCollected(Item item)
    {
        if (item.itemType != _itemType) return;

        _itemsCollected++;
        
        if (_itemsCollected >= _itemsToComplete)
        {
            FinishQuestStep();
        }
    }
}
