using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    private bool isFinished = false;

    private string _questId;
    private int _stepIndex;

    protected virtual void OnEnable()
    {
        QuestManager.Instance.allQuestSteps.Add(this);
    }

    protected virtual void OnDisable()
    {
        QuestManager.Instance.allQuestSteps.Remove(this);
    }

    public void InitializeQuestStep(string questId, int stepIndex, QuestStepState questStepState)
    {
        this._questId = questId;
        this._stepIndex = stepIndex;

        if (questStepState != null && questStepState.state != "")
        {
            SetQuestStepState(questStepState);
        }        
    }

    protected void FinishQuestStep()
    {       
        if (isFinished) return;
        
        isFinished = true;
        Debug.Log("Finish Quest Step: " + this.gameObject.name);

        // 현재 QuestStep 을 끝냈음을 알리는 어떤 수단이 필요
        // 그것은 Dialogue 가 아닐 수도 있음
        if (dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }

        QuestManager.Instance.AdvanceQuest(_questId);

        Destroy(this.gameObject);
    }

    // QuestStep 의 상태에 변화가 생기면 QuestManager 한테 이를 알려주고
    // QuestManager 는 해당하는 Quest의 StepIndex 정보를 변경한다

    protected void ChangeQuestStepState(QuestStepState state)
    {
        QuestManager.Instance.ChangeQuestStepState(_questId, _stepIndex, state);
    }

    protected abstract void UpdateQuestStepState();
    protected abstract void SetQuestStepState(QuestStepState state);
}
