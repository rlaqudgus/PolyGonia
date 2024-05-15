using System.Collections;
using System.Collections.Generic;
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
        
    }

    private void ItemCollected(Item item)
    {
        if (item.itemType != _itemType) return;

        if (_itemsCollected < _itemsToComplete)
        {
            _itemsCollected++;
            UpdateState();
        }
        
        if (_itemsCollected >= _itemsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = _itemsCollected.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this._itemsCollected = System.Int32.Parse(state);
    }
}
