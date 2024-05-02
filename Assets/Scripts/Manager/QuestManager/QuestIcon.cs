using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestIcon : MonoBehaviour
{
    [SerializeField] private GameObject questPoint;

    [Header("Icons")]
    private GameObject[] icons;
    [SerializeField] private GameObject requirementsNotMetIcon;
    [SerializeField] private GameObject canStartIcon;
    [SerializeField] private GameObject inProgressIcon;
    [SerializeField] private GameObject canFinishIcon;

    private void Start()
    {
        icons = new GameObject[] 
        { 
            requirementsNotMetIcon, 
            canStartIcon,
            inProgressIcon,
            canFinishIcon,
        };

        float offsetY = Mathf.Abs(questPoint.transform.localScale.y);
        this.transform.position = questPoint.transform.position + offsetY * Vector3.up;

        foreach (GameObject icon in icons)
        {
            icon.transform.position = this.transform.position;
            icon.SetActive(false);
        }
    }

    public void SetState(QuestState newState, bool startPoint, bool finishPoint)
    {
        // set all to inactive
        foreach (GameObject icon in icons)
        {
            icon.SetActive(false);
        }

        // set the appropriate one to active based on the new state
        switch (newState)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                if (startPoint) { requirementsNotMetIcon.SetActive(true); }
                break;
            case QuestState.CAN_START:
                if (startPoint) { canStartIcon.SetActive(true); }
                break;
            case QuestState.IN_PROGRESS:
                inProgressIcon.SetActive(true);
                break;
            case QuestState.CAN_FINISH:
                if (finishPoint) { canFinishIcon.SetActive(true); }
                break;
            case QuestState.FINISHED:
                break;
            default:
                Debug.LogWarning("Quest State not recognized by switch statement for quest icon: " + newState);
                break;
        }
    }
}