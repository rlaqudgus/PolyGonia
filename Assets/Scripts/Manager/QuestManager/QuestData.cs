using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quest Data for save and load

[System.Serializable]
public class QuestData
{
    public string id;
    public QuestState state;
    public int questStepIndex;
    public QuestStepState[] questStepStates;

    public QuestData(Quest quest)
    {
        this.id = quest.info.id;
        this.state = quest.state;
        this.questStepIndex = quest.currentQuestStepIndex;
        this.questStepStates = quest.questStepStates;
    }
}
