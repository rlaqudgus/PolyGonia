using UnityEngine;

namespace Manager.DialogueScripts
{
    public class NPC : MonoBehaviour, IInteractable
    {
        protected bool _isInteracting;
        public bool IsInteracting => _isInteracting;

        public virtual void Interact()
        {
            Debug.Log("Interact with NPC!");
        }
    }
}
