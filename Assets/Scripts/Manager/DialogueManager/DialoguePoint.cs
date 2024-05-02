using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePoint : NPC, ITalkable
{
    [SerializeField] Dialogue dialogue;

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
        Talk();
    }
}
