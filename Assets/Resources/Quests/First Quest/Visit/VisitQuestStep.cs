using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class VisitQuestStep : QuestStep
{
    // QuestManager의 위치를 zero 로 설정해야 함
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        // Do Nothing
        return;
    }
}
