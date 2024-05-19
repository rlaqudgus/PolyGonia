using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    // static info
    public QuestInfo info;

    // state info
    public QuestState state;
    private int _currentQuestStepIndex;
    public QuestStepState[] questStepStates;

    // Quest Point info
    [HideInInspector] 
    public List<QuestPoint> questPointList;

    public Quest(QuestInfo questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this._currentQuestStepIndex = 0;
        this.questStepStates = new QuestStepState[info.questStepPrefabs.Length];

        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }

        questPointList = new List<QuestPoint>();
    }

    public Quest(QuestInfo questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates, List<QuestPoint> questPointList)
    {
        this.info = questInfo;
        this.state = questState;
        this._currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;
        this.questPointList = questPointList;

        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning(
                "Something may be changed in QuestInfo - saved data is out of sync. " + this.info.id
            );
        }
    }

    public void MoveToNextStep()
    {
        _currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (_currentQuestStepIndex < info.questStepPrefabs.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            // 최적화 시 오브젝트 풀링 사용
            GameObject newQuestStepObject = Object.Instantiate(questStepPrefab, parentTransform);
            QuestStep questStep = newQuestStepObject.GetComponent<QuestStep>();
            questStep.InitializeQuestStep(info.id, _currentQuestStepIndex, questStepStates[_currentQuestStepIndex]);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists()) 
        {
            questStepPrefab = info.questStepPrefabs[_currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that there is no current step: QuestId = " + info.id + " / " + "stepIndex = " + _currentQuestStepIndex);
        }

        return questStepPrefab;
    }

    public QuestData GetQuestData()
    {
        return new QuestData(
            state, 
            _currentQuestStepIndex, 
            questStepStates,
            questPointList
        );
    }
}
