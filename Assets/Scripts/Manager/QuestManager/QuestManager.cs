using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// How create a Quest System in Unity | RPG Style | Including Data Persistence
// https://www.youtube.com/watch?v=UyTJLDGcT64&list=LL

public class QuestManager : Singleton<QuestManager>
{
    [Header("Config")]
    [SerializeField] private bool _loadQuestState = true;

    private Dictionary<string, Quest> questMap;

    private int _currentPlayerLevel = 0;

    private void Awake()
    {
        CreateSingleton(this);

        questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        GameManager.Instance.playerEvents.OnPlayerLevelChange += PlayerLevelChange;
        GameManager.Instance.questEvents.OnQuestStepStateChange += QuestStepStateChange;
    }

    private void OnDisable()
    {   
        GameManager.Instance.playerEvents.OnPlayerLevelChange -= PlayerLevelChange;
        GameManager.Instance.questEvents.OnQuestStepStateChange -= QuestStepStateChange;
    }

    private void Start()
    {
        // UpdateQuest 를 돌린다
        // 현재 Requirement 를 체크하는 조건은 Level 과 이전 퀘스트 Prerequisite 이다
        // 초기에는 Finish 된 퀘스트가 없으므로 Player Level Change 만 이용해서 Requirement 를 체크한다
        GameManager.Instance.playerEvents.PlayerLevelChange(_currentPlayerLevel);

        // QuestPoint 들은 OnEnable 에서 QuestEvents 를 구독하고 있음
        // QuestPoint 들의 초기 current quest state 를 업데이트
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }

            GameManager.Instance.questEvents.QuestStateChange(quest);
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
            
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }

        return idToQuestMap;
    }

    private void UpdateQuest()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private Quest GetQuestById(string id)
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
        GameManager.Instance.questEvents.QuestStateChange(quest);

        Debug.Log("Change " + id + " Quest State to " + state.ToString());
    }

    private void PlayerLevelChange(int level)
    {
        _currentPlayerLevel = level;
        UpdateQuest();
    }

    public void StartQuest(string id)
    {
        Debug.Log("Start Quest: " + id);

        Quest quest = GetQuestById(id);

        // 예외처리
        if (quest.info.questStepPrefabs.Length == 0)
        {
            Debug.LogWarning("It seems that Quest step prefabs are not assigned yet: " + id);
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
            return;
        }

        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
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
        UpdateQuest();
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

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);

    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    #region Save / Load

    private void SaveQuest(Quest quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            
            PlayerPrefs.SetString(quest.info.id, serializedData);

            Debug.Log(serializedData);
        }

        catch (System.Exception e)
        {   
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfo questInfo)
    {
        Quest quest = null;

        try
        {
            if (PlayerPrefs.HasKey(questInfo.id) && _loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);

                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            else
            {
                quest = new Quest(questInfo);
            }
        }

        catch (System.Exception e)
        {   
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }

        return quest;
    }

    #endregion

}
