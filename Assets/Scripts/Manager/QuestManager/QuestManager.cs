using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// How create a Quest System in Unity | RPG Style | Including Data Persistence
// https://www.youtube.com/watch?v=UyTJLDGcT64&list=LL

public class QuestManager : Singleton<QuestManager>
{
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
    }

    private void OnDisable()
    {   
        GameManager.Instance.playerEvents.OnPlayerLevelChange -= PlayerLevelChange;
    }

    private void Start()
    {
        foreach (Quest quest in questMap.Values)
        {
            GameManager.Instance.questEvents.QuestStateChange(quest);
        }
    }

    private void Update()
    {
        foreach (Quest quest in questMap.Values)
        {
            // if we're now meeting the requirements, switch over to the CAN_START state
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
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
            
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }

        return idToQuestMap;
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
        quest.state = state;
        GameManager.Instance.questEvents.QuestStateChange(quest);

        Debug.Log("Change " + id + " Quest State to " + state.ToString());
    }

    private void PlayerLevelChange(int level)
    {
        _currentPlayerLevel = level;
    }

    public void StartQuest(string id)
    {
        Debug.Log("Start Quest: " + id);

        Quest quest = GetQuestById(id);
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

}
