using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

// Make a Dialogue System
// https://www.youtube.com/watch?v=jTPOCglHejE

public class NPC : MonoBehaviour, IInteractable
{
    public Dialogue dialogue;

    public void Interact()
    {
        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}
