using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] QuestInfo questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool _startPoint = true;
    [SerializeField] private bool _finishPoint = true;

    private bool _playerIsNear = false;

    private string _questId;

    private QuestState _currentQuestState;
    [SerializeField] private QuestIcon questIcon;

    private void Awake()
    {
        _questId = questInfoForPoint.id;
    }

    private void OnEnable()
    {
        GameManager.Instance.questEvents.OnQuestStateChange += QuestStateChange;
        GameManager.Instance.inputEvents.OnSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        GameManager.Instance.questEvents.OnQuestStateChange -= QuestStateChange;
        GameManager.Instance.inputEvents.OnSubmitPressed -= SubmitPressed;
    }

    private void QuestStateChange(Quest quest)
    {
        // only update the quest state if this point has the corresponding quest
        if (quest.info.id == _questId)
        {
            _currentQuestState = quest.state;
            questIcon.SetState(_currentQuestState, _startPoint, _finishPoint);
        }
    }

    private void SubmitPressed()
    {
        if (!_playerIsNear) return;

        if ((_currentQuestState == QuestState.CAN_START) && _startPoint)
        {
            QuestManager.Instance.StartQuest(_questId);
        }
        else if ((_currentQuestState == QuestState.CAN_FINISH) && _finishPoint)
        {
            QuestManager.Instance.FinishQuest(_questId);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerIsNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerIsNear = false;
    }
}
