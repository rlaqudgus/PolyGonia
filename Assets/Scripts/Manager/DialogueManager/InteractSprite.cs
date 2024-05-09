using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSprite : MonoBehaviour
{
    [SerializeField] GameObject _interactableObject;

    [SerializeField] private float _moveDistance;
    [SerializeField] private float _moveSpeed;
    
    private float _angle;

    private void Start()
    {
        
    }

    
    private void Update()
    {
        Vector3 origin = _interactableObject.transform.position;
        Vector3 constant = Mathf.Abs(_interactableObject.transform.localScale.y) * Vector3.up;
        Vector3 variable = _moveDistance * Mathf.Sin(_angle) * Vector3.up;

        _angle += _moveSpeed * Time.deltaTime;

        transform.position = origin + constant + variable;
    }
    

    private void OnEnable() 
    {
        
    }

    private void OnDisable() 
    {
        _angle = 0f;   
    }

    public void SetActive(bool flag) 
    {
        this.gameObject.SetActive(flag);
    }
}
