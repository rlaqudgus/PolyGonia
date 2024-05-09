using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class Item : MonoBehaviour
{
    public abstract ItemType itemType { get; }

    protected void CollectItem()
    {
        GameManager.Instance.miscEvents.ItemCollected(this);
    }

    protected void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject.CompareTag("Player"))
        {
            CollectItem();
            Destroy(gameObject);
        }
    }
}
