using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePoint : NPC, ITalkable
{
    [SerializeField] Dialogue dialogue;

    private void OnEnable()
    {
        DialogueManager.Instance.OnDialogueEnded += DialogueEnded;
    }

    private void OnDisable()
    {
        DialogueManager.Instance.OnDialogueEnded -= DialogueEnded;
    }

    // Implement ITalkable Interface
    public void Talk()
    {
        if (dialogue == null)
        {
            Debug.LogWarning("There is no dialogue data assigned");
            return;
        }
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    // Implement IInteractable Interface in NPC
    public override void Interact()
    {   
        _isInteracting = true;

        Talk();
    }

    private void DialogueEnded()
    {
        // Dialogue가 끝났다는 소식을 들었는데 그것이 본인의 Dialogue가 아니면 return
        if (!_isInteracting) return;

        // Dialogue 가 끝났으면 isInteracting 을 false로 바꾼다
        _isInteracting = false;
    }
}
