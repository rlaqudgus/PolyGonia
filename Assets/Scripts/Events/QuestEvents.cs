using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEvents
{
    public event Action<Quest> OnQuestStateChange;

    public void QuestStateChange(Quest quest)
    {
        if (OnQuestStateChange != null)
        {
            OnQuestStateChange(quest);
        }
    }

}
