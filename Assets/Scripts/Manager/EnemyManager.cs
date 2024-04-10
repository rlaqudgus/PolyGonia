using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _instance;
    public  static EnemyManager  Instance { get { return _instance; } }
    
    // enemyPrefab은 prefab을 담는다
    // 나중에 EnemySpawn 관련해서도 활용 예정
    public GameObject[] enemyPrefab;

    // enemySet은 모든 Enemy를 담는다
    // 기존에는 GameObject를 담았고 각 GameObject가 Enemy에 해당하는지 검사했지만
    // 처음부터 Enemy를 담는 것이 더 직관적인 것 같아 바꾸었다
    // index를 사용하지 않음 + 빠른 검색을 위해 Hash를 사용한다
    private HashSet<Enemy> _enemySet = new HashSet<Enemy>();

    // enemyDict는 enemy의 이름으로부터 prefab 인스턴스를 생성하기 위해 만들었다
    private Dictionary<string, GameObject> _enemyDict = new Dictionary<string, GameObject>();

    // Singleton Pattern
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {   
        foreach (GameObject enemy in enemyPrefab) 
        {
            // enemyPrefab에 등록되는 프리팹은 Enemy 타입이어야 한다
            Debug.Assert(
                enemy.GetComponent<Enemy>() != null, 
                enemy.name + " is not an enemy type"
            );

            // 동일한 프리팹이 두 번 이상 등록되면 에러가 출력된다
            _enemyDict.Add(enemy.name, enemy);
        }
    }   

    // Add, Remove는 Enemy의 OnEnable / OnDisable을 통해 호출되도록 만들었다
    // Enemy를 프로그래밍하는 사람은 평소처럼 Instantiate, Destroy, SetActive를 호출하면 된다
    // 그러면 자동으로 OnEnable / OnDisable이 호출되면서 Enemy의 상태 정보가 EnemyManager로 전달된다

    // Enemy를 Set에 추가한다
    public void Add(Enemy enemy) 
    {
        string enemyName = enemy.gameObject.name;
        Debug.Assert(
            !_enemySet.Contains(enemy), 
            enemyName + " already exists in enemy list"
        );

        _enemySet.Add(enemy);
        this.Log(enemyName + " is added to the enemy list");
    }

    // Enemy를 Set에서 제거한다
    public void Remove(Enemy enemy) 
    {
        string enemyName = enemy.gameObject.name;
        Debug.Assert(
            _enemySet.Contains(enemy),
            enemyName + " does not exists in enemy list"
        );

        _enemySet.Remove(enemy);
        this.Log(enemyName + " is removed from the enemy list");
    }

    // 모든 Enemy를 Set에서 제거하고 Destroy한다
    public void Clear() 
    {
        foreach (Enemy enemy in _enemySet) 
        {
            Destroy(enemy.gameObject);
        }
        
        this.Log("All enemies are destroyed");
    }

    // Enemy의 개수를 반환한다
    public int Count() 
    {
        return _enemySet.Count;
    }

    // name에 해당하는 Enemy를 생성한다
    public void Create(string name, Vector3 position, Quaternion rotation) 
    {
        // 기본적으로 Dictionary 에 존재하지 않는 이름이면 에러가 발생한다
        // 그래도 Assert로 한 번 더 에러를 출력해 주었다
        Debug.Assert(
            _enemyDict.ContainsKey(name),
            name + " prefab is not assigned to enemy prefab list"
        );
        
        GameObject enemyObject = _enemyDict[name];
        Instantiate(enemyObject, position, rotation);
    }

    // 현재 Enemy를 다른 종류의 Enemy로 바꾼다
    // Enemy의 위치는 그대로 두고 Enemy의 종류만 변경한다

    // Example
    // EnemyManager.Instance.Replace(this, "Civilian");

    // 현재까지의 Replace는 Destroy -> Instantiate 을 이용하여 구현되었다
    // 필요하다면 SetActive를 이용한 Replace를 구현할 수도 있을 것이다
    public void Replace(Enemy curEnemy, string newEnemyName) 
    {
        Vector3 position = curEnemy.transform.position;
        Quaternion rotation = curEnemy.transform.rotation;

        Destroy(curEnemy.gameObject);
        Create(newEnemyName, position, rotation);
        this.Log(curEnemy.gameObject.name + " is changed to " + newEnemyName);
    }

    // action 함수를 인자로 받아서 모든 Enemy한테 action을 공통적으로 적용한다
    // filter를 적용하면 부분적인 Enemy에 대해 action이 적용된다
    // action은 void를 return해야 하고 filter는 bool을 return해야 한다

    /* Example - Triangle을 모두 위로 띄우는 코드
    *
    * EnemyManager.Instance.Apply(
    *     (Enemy enemy) => {
    *         Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
    *         rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
    *     },
    *     (Enemy enemy) => {
    *         bool isTriangle = (enemy.gameObject.GetComponent<Triangle>() != null);
    *         return isTriangle;
    *     }
    * );
    */

    public void Apply(Action<Enemy> action)
    {
        foreach (Enemy enemy in _enemySet)
        {   
            // SetActive == false 이면 패스한다
            bool isActive = enemy.gameObject.activeSelf;
            if (!isActive) continue;
            
            // 함수 또는 람다식 적용
            action(enemy);
        }
    }

    public void Apply(Action<Enemy> action, Func<Enemy, bool> filter)
    {
        foreach (Enemy enemy in _enemySet)
        {   
            // SetActive == false이면 패스한다
            bool isActive = enemy.gameObject.activeSelf;
            if (!isActive) continue;

            // 조건에 해당하는 enemy를 남긴다
            if (!filter(enemy)) continue;
            
            // 함수 또는 람다식 적용
            action(enemy);
        }
    }
}
