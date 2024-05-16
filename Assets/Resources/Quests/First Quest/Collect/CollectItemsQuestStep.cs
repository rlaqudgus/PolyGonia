using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItemsQuestStep : QuestStep
{
    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _itemsCollected;
    [SerializeField] private int _itemsToComplete;

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.miscEvents.OnItemCollected += ItemCollected;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.Instance.miscEvents.OnItemCollected -= ItemCollected;
    }

    private void Start()
    {
        UpdateQuestStepState();
    }

    private void ItemCollected(Item item)
    {
        if (item.itemType != _itemType) return;

        if (_itemsCollected < _itemsToComplete)
        {
            _itemsCollected++;
            UpdateQuestStepState();
        }
        
        if (_itemsCollected >= _itemsToComplete)
        {
            FinishQuestStep();
        }
    }

    protected override void UpdateQuestStepState()
    {
        QuestStepState state = new QuestStepState(_itemsCollected.ToString());
        ChangeQuestStepState(state);
    }

    protected override void SetQuestStepState(QuestStepState state)
    {
        this._itemsCollected = System.Int32.Parse(state.state);
    }
}
