using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class NPC : MonoBehaviour, IInteractable
{
    public virtual void Interact()
    {
        Debug.Log("Interact with NPC!");
    }
}
