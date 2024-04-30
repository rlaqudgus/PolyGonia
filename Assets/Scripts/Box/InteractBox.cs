using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBox : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _interactableObject;
    [SerializeField] GameObject _interactSprite;

    PlayerController _playerController;
    private bool _isWithinInteractBox;
    public  bool  isWithinInteractBox { get { return _isWithinInteractBox; } }

    private void Start()
    {   
        _playerController = _player.GetComponent<PlayerController>();
        _isWithinInteractBox = false;
        _interactSprite.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player") return;
        
        if (_isWithinInteractBox) return;

        _isWithinInteractBox = true;

        _interactSprite.gameObject.SetActive(true);

        _playerController.scannedObjects.Add(transform.parent.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!_isWithinInteractBox) return;
        
        _isWithinInteractBox = false;

        _interactSprite.gameObject.SetActive(false);

        _playerController.scannedObjects.Remove(transform.parent.gameObject);
        
    }
}
