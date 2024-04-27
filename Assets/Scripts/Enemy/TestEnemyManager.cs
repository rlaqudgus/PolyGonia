using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyManager : MonoBehaviour
{
    public float rotationSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);   
    }

    private void OnTriggerEnter2D(Collider2D col) 
    {
        // Civilian을 모두 위로 띄우는 코드
        if (col.gameObject.CompareTag("Player"))
        {
            EnemyManager.Instance.Apply(
                (Enemy enemy) => {
                    Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                    rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
                },
                (Enemy enemy) => {
                    bool isCivilian = (enemy.gameObject.GetComponent<Civilian>() != null);
                    return isCivilian;
                }
            );

            Destroy(gameObject);
        }
    }
}
