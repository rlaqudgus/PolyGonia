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

    //�����ε� 1. Action 1��
    //time �� �ڿ� Ư�� Action�� �����Ű�� timer 
    public void StartTimer(float time, Action ontime)
    {
        tasks.Add(new Task(time, ontime));
    }

    //�����ε� 2. Action 2��
    //ù��° Action ���� �� time �� �ڿ� �ι�° Action ����
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

    //Task�� ������
    public Task(float restTime, Action onTime)
    {
        this.restTime = restTime;
        this.onTime = onTime;
    }
}

