using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quest Data for save and load

[System.Serializable]
public class QuestData
{
    public QuestState state;
    public int questStepIndex;
    public QuestStepState[] questStepStates;
    
    [HideInInspector] 
    public List<QuestPoint> questPointList;

    public QuestData(QuestState state, int questStepIndex, QuestStepState[] questStepStates, List<QuestPoint> questPointList)
    {
        this.state = state;
        this.questStepIndex = questStepIndex;
        this.questStepStates = questStepStates;
        this.questPointList = questPointList;
    }
}
