using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    // static info
    public QuestInfo info;

    // state info
    public QuestState state;
    public int currentQuestStepIndex;
    public QuestStepState[] questStepStates;

    public Quest(QuestInfo questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
        this.questStepStates = new QuestStepState[info.questStepPrefabs.Length];

        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfo questInfo, QuestData questData)
    {
        this.info = questInfo;
        this.state = questData.state;
        this.currentQuestStepIndex = questData.questStepIndex;
        this.questStepStates = questData.questStepStates;

        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning(
                "Something may be changed in QuestInfo - saved data is out of sync. " + this.info.id
            );
        }
    }
    public Quest(QuestInfo questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
    {
        this.info = questInfo;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;

        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning(
                "Something may be changed in QuestInfo - saved data is out of sync. " + this.info.id
            );
        }
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.questStepPrefabs.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            // 최적화 시 오브젝트 풀링 사용
            GameObject newQuestStepObject = Object.Instantiate(questStepPrefab, parentTransform);
            QuestStep questStep = newQuestStepObject.GetComponent<QuestStep>();
            questStep.InitializeQuestStep(this);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists()) 
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that there is no current step: QuestId = " + info.id + " / " + "stepIndex = " + currentQuestStepIndex);
        }

        return questStepPrefab;
    }
}
