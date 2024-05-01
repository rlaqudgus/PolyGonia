using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float rotationSpeed;

    void Start()
    {
        rotationSpeed = 50f;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);   
    }

    private void CollectCoin()
    {
        GameManager.Instance.miscEvents.CoinCollected();
    }

    private void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject.CompareTag("Player"))
        {
            CollectCoin();
            Destroy(gameObject);
        }
    }
}
