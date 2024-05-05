using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    private bool isFinished = false;

    private string _questId;

    public void InitializeQuestStep(string questId)
    {
        this._questId = questId;
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
}
