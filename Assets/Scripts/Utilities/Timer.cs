using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Timer : Singleton<Timer>
{
    List<Task> tasks = new List<Task>();

    // Start is called before the first frame update
    void Start()
    {
        CreateSingleton(this);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = tasks.Count-1; i >= 0; i--)
        {
            tasks[i].restTime -= Time.deltaTime;
            if (tasks[i].restTime<=0)
            {
                tasks[i].onTime();
                tasks.RemoveAt(i);
            }
        }
    }

    //오버로딩 1. Action 1개
    //time 초 뒤에 특정 Action을 실행시키는 timer 
    public void StartTimer(float time, Action ontime)
    {
        tasks.Add(new Task(time, ontime));
    }

    //오버로딩 2. Action 2개
    //첫번째 Action 실행 후 time 초 뒤에 두번째 Action 실행
    public void StartTimer(float time, Action beforeTime, Action ontime)
    {
        beforeTime();
        tasks.Add(new Task(time, ontime));
    }
}

public class Task
{
    public Action onTime;
    public float restTime;
    public bool _value = false;

    //Task의 생성자
    public Task(float restTime, Action onTime)
    {
        this.restTime = restTime;
        this.onTime = onTime;
    }
}

