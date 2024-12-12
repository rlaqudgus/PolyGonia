using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// How create a Quest System in Unity | RPG Style | Including Data Persistence
// https://www.youtube.com/watch?v=UyTJLDGcT64&list=LL

public class QuestManager : Singleton<QuestManager>
{
    [Header("Config")]

    private Dictionary<string, Quest> questMap;
    public HashSet<QuestStep> allQuestSteps;

    private int _currentPlayerLevel = 0;
    public event Action<Quest> OnQuestStateChange;

    private void Awake()
    {
        CreateSingleton(this);

        questMap = CreateQuestMap();
        allQuestSteps = new HashSet<QuestStep>();
    }

    private void OnEnable()
    {
        GameManager.Instance.playerEvents.OnPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable()
    {   
        GameManager.Instance.playerEvents.OnPlayerLevelChange -= PlayerLevelChange;
    }

    private void Start()
    {
        // UpdateQuests 를 돌린다
        // 현재 Requirement 를 체크하는 조건은 Level 과 이전 퀘스트 Prerequisite 이다
        // 초기에는 Finish 된 퀘스트가 없으므로 Player Level Change 만 이용해서 Requirement 를 체크한다

        // 로드 시 순서가 중요하다면 Event 를 사용할 수 없음
        GameManager.Instance.playerEvents.PlayerLevelChange(_currentPlayerLevel);

        // QuestPoint 들은 OnEnable 에서 QuestEvents 를 구독하고 있음
        // QuestPoint 들의 초기 current quest state 를 업데이트
        foreach (Quest quest in questMap.Values)
        {
            OnQuestStateChange?.Invoke(quest);
        }
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // Assets/Resources/Quests 폴더에 존재하는 모든 QuestInfo 로드
        QuestInfo[] allQuests = Resources.LoadAll<QuestInfo>("Quests");
        
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();

        foreach (QuestInfo questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            
            Quest quest = new Quest(questInfo);
            idToQuestMap.Add(questInfo.id, quest);
        }

        return idToQuestMap;
    }

    private void UpdateQuests()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    public Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }

        return quest;
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);

        if (quest.state == state) return;

        quest.state = state;

        OnQuestStateChange?.Invoke(quest);

        Debug.Log("Change " + id + " Quest State to " + state.ToString());
    }

    public void ChangeQuestStepState(QuestStep questStep, QuestStepState state)
    {
        Quest quest = GetQuestById(questStep.questId);
        quest.questStepStates[questStep.stepIndex] = state;
    }

    private void PlayerLevelChange(int level)
    {
        _currentPlayerLevel = level;
        UpdateQuests();
    }

    public void StartQuest(string id)
    {
        Debug.Log("Start Quest: " + id);

        Quest quest = GetQuestById(id);

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
            ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
        }
        else
        {
            Debug.Log("It seems that Quest step prefabs are not assigned yet: " + id);
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    public void AdvanceQuest(string id)
    {
        Debug.Log("Advance Quest: " + id);

        Quest quest = GetQuestById(id);

        // Move on to the next step
        quest.MoveToNextStep();

        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    public void FinishQuest(string id)
    {
        Debug.Log("Finish Quest: " + id);

        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        UpdateQuests();
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        // start true and prove to be false
        bool meetsRequirements = true;

        // check player level requirements
        if (_currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }

        // check quest prerequisites for completion
        foreach (QuestInfo prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
                break;
            }
        }

        return meetsRequirements;
    }

    private void ClaimRewards(Quest quest)
    {
        Debug.Log("You get the rewards!");
        return;
    }

    #region Save / Load

    public List<QuestData> GetQuestDataList()
    {
        List<QuestData> questDataList = new List<QuestData>();
        foreach (Quest quest in questMap.Values)
        {
            QuestData data = new QuestData(quest);
            questDataList.Add(data);
        }

        return questDataList;
    }

    public void SetQuestDataList(List<QuestData> questDataList)
    {
        if (questMap == null) questMap = CreateQuestMap();

        // 기존에 존재하는 모든 Quest Step 파괴
        HashSet<QuestStep> allQuestStepsCopied = new HashSet<QuestStep>(allQuestSteps);
        foreach (QuestStep questStep in allQuestStepsCopied)
        {
            Destroy(questStep.gameObject);
        }

        // 데이터 로드
        foreach (QuestData questData in questDataList)
        {
            Quest oldQuest = GetQuestById(questData.id);
            Quest newQuest = new Quest(oldQuest.info, questData);
            questMap[newQuest.info.id] = newQuest;
        }

        // 데이터 반영
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }

            OnQuestStateChange?.Invoke(quest);
        }

        Debug.Log("Set Quest Data List works");
    }

    #endregion
}
