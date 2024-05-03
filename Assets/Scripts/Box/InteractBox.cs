using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBox : MonoBehaviour
{
    [SerializeField] GameObject _interactableObject;
    [SerializeField] InteractSprite _interactSprite;

    PlayerController _playerController;
    private bool _isWithinInteractBox;
    public  bool  isWithinInteractBox { get { return _isWithinInteractBox; } }

    private void Start()
    {   
        _playerController = GameManager.Instance.playerController;
        Debug.Assert(_playerController != null, "Player Controller is null");

        _isWithinInteractBox = false;
        _interactSprite.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player") return;
        
        if (_isWithinInteractBox) return;

        _isWithinInteractBox = true;

        _interactSprite.SetActive(true);

        _playerController.scannedObjects.Add(transform.parent.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!_isWithinInteractBox) return;
        
        _isWithinInteractBox = false;

        _interactSprite.SetActive(false);

        _playerController.scannedObjects.Remove(transform.parent.gameObject);
        
    }
}
