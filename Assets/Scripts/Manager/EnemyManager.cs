using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }

    // enemyArr는 prefab을 담는다
    // Dictionary<string, GameObject>를 사용하면 깔끔한데
    // Dictionary를 사용하면 인스펙터 창에서 수정이 안 된다
    public GameObject[] enemyArr;

    // enemySet은 모든 적의 GameObject를 담는다
    // index를 사용하지 않음 + 빠른 검색을 위해 Hash를 사용한다
    private HashSet<GameObject> _enemySet = new HashSet<GameObject>();

    // Singleton Pattern
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 빠른 검색을 위한 정렬
        Array.Sort(enemyArr, (left, right) => {
            return left.name.CompareTo(right.name); 
        });
    }

    public void Update()
    {
        
    }

    // Enemy를 Set에 추가한다
    // Enemy의 Start() 부분에서 사용된다
    public void AddEnemy(GameObject enemy) 
    {
        Debug.Assert(
            !_enemySet.Contains(enemy), 
            enemy.name + " already exists in enemy list"
        );

        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        Debug.Assert(
            enemyComponent != null, 
            enemy.name + " is not an emeny type"
        );

        _enemySet.Add(enemy);
        this.Log(enemy.name + " is added to the enemy list");
    }

    // Enemy를 Set에서 제거하고 Destroy한다
    public void RemoveEnemy(GameObject enemy) {
        Debug.Assert(
            _enemySet.Contains(enemy),
            enemy.name + " does not exists in enemy list"
        );

        _enemySet.Remove(enemy);
        Destroy(enemy);
        this.Log(enemy.name + " is removed and destroyed from the enemy list");
    }

    // 모든 Enemy를 Set에서 제거하고 Destroy한다
    public void RemoveAllEnemy() {
        foreach (GameObject enemy in _enemySet) 
        {
            Destroy(enemy);
        }
        _enemySet.Clear();
        this.Log("All enemies are destroyed");
    }

    // 현재 Enemy를 다른 종류의 Enemy로 바꾼다
    // Enemy의 위치는 그대로 두고 Enemy의 종류만 변경한다
    // Name에 해당하는 prefab을 이진 탐색을 이용해서 찾는다
    public void ChangeEnemy(GameObject curEnemy, string newEnemyName) {
        Enemy curEnemyComponent = curEnemy.GetComponent<Enemy>();
        Debug.Assert(
            curEnemyComponent != null, 
            curEnemy.name + " is not an emeny type"
        );

        Vector3 position = curEnemyComponent.transform.position;
        Quaternion rotation = curEnemyComponent.transform.rotation;

        GameObject newEnemy = null;

        Debug.Assert(
            enemyArr.Length > 0, 
            "The Length of Enemy Array is zero"
        );

        // Binary Search
        int left = 0, right = enemyArr.Length-1;
        while (left <= right)
        {
            int mid = (left + right) / 2;
            string prefabName = enemyArr[mid].name;

            if (prefabName == newEnemyName) { newEnemy = enemyArr[mid]; break; }
            else if (enemyArr[mid].name.CompareTo(newEnemyName) < 0) { left  = mid + 1; }
            else if (enemyArr[mid].name.CompareTo(newEnemyName) > 0) { right = mid - 1; }
        }

        Debug.Assert(
            newEnemy != null, 
            "The Enemy Name " + newEnemyName + " is wrong"
        );

        Enemy newEnemyComponent = newEnemy.GetComponent<Enemy>();
        
        Debug.Assert(
            newEnemyComponent != null, 
            newEnemy.name + " is not an enemy type"
        );

        RemoveEnemy(curEnemy);
        Instantiate(newEnemy, position, rotation);
        this.Log(curEnemy.name + " is changed to " + newEnemy.name);
    }
}
