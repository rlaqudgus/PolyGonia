using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class NPC : MonoBehaviour, IInteractable
{
    protected bool _isInteracting;
    public bool isInteracting { get { return _isInteracting; } }

    public virtual void Interact()
    {
        Debug.Log("Interact with NPC!");
    }
}
