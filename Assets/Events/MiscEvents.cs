using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscEvents
{
    public event Action<Item> OnItemCollected;

    public void ItemCollected(Item item)
    {
        if (OnItemCollected != null) 
        {
            OnItemCollected(item);
        }
    }
}
