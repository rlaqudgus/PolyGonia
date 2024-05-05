using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestIcon : MonoBehaviour
{
    [SerializeField] private GameObject _questPoint;

    [Header("Icons")]
    private GameObject[] _icons;
    [SerializeField] private GameObject _requirementsNotMetIcon;
    [SerializeField] private GameObject _canStartIcon;
    [SerializeField] private GameObject _inProgressIcon;
    [SerializeField] private GameObject _canFinishIcon;
    [SerializeField] private GameObject _finishedIcon;

    private void Start()
    {
        _icons = new GameObject[] 
        { 
            _requirementsNotMetIcon, 
            _canStartIcon,
            _inProgressIcon,
            _canFinishIcon,
            _finishedIcon
        };

        float offsetY = Mathf.Abs(_questPoint.transform.localScale.y);
        this.transform.position = _questPoint.transform.position + offsetY * Vector3.up;

        foreach (GameObject icon in _icons)
        {
            icon.transform.position = this.transform.position;
            icon.SetActive(false);
        }
    }

    public void SetState(QuestState newState, bool startPoint, bool finishPoint)
    {
        // set all to inactive
        foreach (GameObject icon in _icons)
        {
            icon.SetActive(false);
        }

        // set the appropriate one to active based on the new state
        switch (newState)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                if (startPoint) { _requirementsNotMetIcon.SetActive(true); }
                break;
            case QuestState.CAN_START:
                if (startPoint) { _canStartIcon.SetActive(true); }
                break;
            case QuestState.IN_PROGRESS:
                _inProgressIcon.SetActive(true);
                break;
            case QuestState.CAN_FINISH:
                if (finishPoint) { _canFinishIcon.SetActive(true); }
                break;
            case QuestState.FINISHED:
                _finishedIcon.SetActive(true);
                break;
            default:
                Debug.LogWarning("Quest State not recognized by switch statement for quest icon: " + newState);
                break;
        }
    }
}