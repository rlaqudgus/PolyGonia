using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class VisitQuestStep : QuestStep
{
    private void Start()
    {
        UpdateQuestStepState();
    }

    // QuestManager의 위치를 zero 로 설정해야 함
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        FinishQuestStep();
    }

    protected override void UpdateQuestStepState()
    {
        QuestStepState state = new QuestStepState("In Progress");
        ChangeQuestStepState(state);
    }

    protected override void SetQuestStepState(QuestStepState state)
    {
        // Do Nothing
        return;
    }
}
