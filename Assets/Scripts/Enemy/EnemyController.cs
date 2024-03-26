using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public enum EnemyType 
{ 
    // Triangle
    Civilian, Soldier, Jumper 
}

public enum EnemyState 
{ 
    Idle, Detect, Attack, Die 
}

public class EnemyController : MonoBehaviour
{
    public Enemy enemy;
    public EnemyState enemyState;

    public void Start()
    {
        StartCoroutine(EnemyHandler());
    }
    
    public IEnumerator EnemyHandler()
    {
        while (true)
        {
            yield return StartCoroutine(enemy.Behaviour(enemyState));
        }
    }

    public void EnemyChange(EnemyType enemyType)
    {
        enemy.enabled = false;
        
        string enemyName = enemyType.ToString();
        enemy = (Enemy) GetComponent(enemyName);
        enemy.enabled = true;

        enemy.InitSetting();
        enemyState = EnemyState.Idle;
    }
}
