using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;

    private string _questId;

    public void InitializeQuestStep(string questId)
    {
        this._questId = questId;
    }

    protected void FinishQuestStep()
    {
        Debug.Log("Finish Quest Step: " + this.gameObject.name);
        
        if (!isFinished)
        {
            isFinished = true;
            QuestManager.Instance.AdvanceQuest(_questId);
            Destroy(this.gameObject);
        }
    }
}
