using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    public override ItemType itemType { get { return ItemType.Coin; } }

    [SerializeField] float rotationSpeed;

    void Start()
    {
        rotationSpeed = 50f;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);   
    }
}
