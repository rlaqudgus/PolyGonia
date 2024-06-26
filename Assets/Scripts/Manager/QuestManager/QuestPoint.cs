using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : NPC, ITalkable
{
    [Header("Quest")]
    [SerializeField] QuestInfo questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool _startPoint = true;
    [SerializeField] private bool _finishPoint = true;

    private string _questId;

    private QuestState _currentQuestState;

    [Header("Quest Icon")]
    [SerializeField] QuestIcon _questIcon;

    [Header("Dialogue")]
    [SerializeField] Dialogue _requirementsNotMetDialogue;
    [SerializeField] Dialogue _canStartDialogue;
    [SerializeField] Dialogue _inProgressDialogue;
    [SerializeField] Dialogue _canFinishDialogue;
    [SerializeField] Dialogue _finishedDialogue;

    Dialogue[] _questDialogues;
    Dialogue _currentDialogue;

    private void Awake()
    {
        _questId = questInfoForPoint.id;

        _questDialogues = new Dialogue[] 
        { 
            _requirementsNotMetDialogue, 
            _canStartDialogue,
            _inProgressDialogue,
            _canFinishDialogue,
            _finishedDialogue
        };
    }

    private void OnEnable()
    {
        QuestManager.Instance.GetQuestById(_questId).questPointList.Add(this);
        DialogueManager.Instance.OnDialogueEnded += DialogueEnded;

    }

    private void OnDisable()
    {
        QuestManager.Instance.GetQuestById(_questId).questPointList.Remove(this);
        DialogueManager.Instance.OnDialogueEnded -= DialogueEnded;
    }

    public void ChangeQuestState(Quest quest)
    {
        if (quest.info.id != _questId) 
        {
            Debug.LogWarning(
                "Quest and related NPC is not consistent: " + 
                quest.info.id + "-" + this.gameObject.name + " " + _questId
            );
        }

        // update the quest state if this point has the corresponding quest
        if (quest.info.id == _questId)
        {
            _currentQuestState = quest.state;
            _currentDialogue = _questDialogues[(int) _currentQuestState];
            _questIcon.SetState(_currentQuestState, _startPoint, _finishPoint);
        }
    }

    // Implement ITalkable Interface
    public void Talk()
    {
        DialogueManager.Instance.StartDialogue(_currentDialogue);
    }

    // Implement IInteractable Interface in NPC
    public override void Interact()
    {   
        _isInteracting = true;

        // 모든 QuestState 에 대해 반드시 대화 데이터가 존재해서 대화할 필요는 없다
        // 가령 REQUIREMENTS_NOT_MET 일 때 대화 불가능하도록 하려면 대화 데이터를 할당하지 않으면 된다
        if (_currentDialogue == null) _isInteracting = false;
        else Talk(); // EndDialogue 시 isInteracting 을 false 로 만든다
    }

    private void DialogueEnded()
    {
        // Dialogue가 끝났다는 소식을 들었는데
        // 그것이 QuestPoint 본인의 Dialogue가 아니면 return
        if (!_isInteracting) return;

        // Dialogue 가 끝났으면 isInteracting 을 false로 바꾼다
        _isInteracting = false;

        if ((_currentQuestState == QuestState.CAN_START) && _startPoint)
        {
            QuestManager.Instance.StartQuest(_questId);
        }
        else if ((_currentQuestState == QuestState.CAN_FINISH) && _finishPoint)
        {
            QuestManager.Instance.FinishQuest(_questId);
        }
    }
}
