using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bat : Triangle
{
    NavMeshAgent agent;
    NavMeshPath path;

    [SerializeField] private Transform[] _targetPointsInIdleState;

    protected override void Start()
    {
        base.Start();

        // 2024 AI Pathfinding: Unity 2D Pathfinding with NavMesh tutorial in 5 minutes
        // https://www.youtube.com/watch?v=HRX0pUSucW4

        // Unity C# > 컴포넌트 : NavMeshAgent 와 프로퍼티/함수 모음
        // https://ansohxxn.github.io/unitydocs/navmeshagent/

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        path = new NavMeshPath();
    }

    protected override IEnumerator Idle()
    {
        yield return base.Idle();

        // 예외 처리
        if (_target != null && agent.CalculatePath(_target.position, path))
        {
            StateChange(EnemyState.Detect);
            yield break;
        }

        // 미리 지정해 둔 포인트 및 현재 위치 중 하나 선택
        Vector3 position;
        
        int len = _targetPointsInIdleState.Length;
        int choice = Random.Range(0, len); // [0, len)
        position = _targetPointsInIdleState[choice].position;

        // 목적지 설정
        agent.SetDestination(position);

        yield return new WaitForSeconds(3f);
    }

    protected override IEnumerator Detect()
    {
        yield return base.Detect();

        if (_target == null)
        {
            StateChange(EnemyState.Idle);
            yield break;
        }

        Vector3 position = _target.position + Vector3.up;

        // target 에 도달할 수 없는 경우 Idle 상태로 변환
        if (!agent.CalculatePath(position, path))
        {
            StateChange(EnemyState.Idle);
            yield break;
        }

        // target 에 도달할 수 있는 경우 목적지 설정
        agent.SetDestination(position);

        // 남은 거리가 충분히 작다면 Attack State 로 전환
        if (agent.remainingDistance < _meeleeRange)
        {
            // _anim.SetTrigger("Attack");
            StateChange(EnemyState.Attack);
        }
    }

    protected override IEnumerator Attack()
    {
        yield return base.Attack();

        while (_target != null && _isMeeleeRange)
        {
            // Wait 하는 동안 target 이 사라질 수 있으므로 미리 저장
            Vector3 targetPos = _target.position;
            
            // 잠시 기다렸다가 공격
            agent.ResetPath();
            yield return new WaitForSeconds(0.5f);

            agent.SetDestination(targetPos + Vector3.up);
            yield return Rush(targetPos - this.transform.position, _moveSpd);
        }

        StateChange(EnemyState.Detect);
    }

    protected override IEnumerator Die()    
    {   
        yield return base.Die();    
    }

    private IEnumerator Rush(Vector3 dir, float speed)
    {
        speed = Mathf.Max(speed, 0.1f);
    
        float dist = dir.magnitude;
        float remainingDistance = dist;
        int MAX_ITER = 1000;
        int iter = 0;

        while (remainingDistance > 0)
        {
            if (iter > MAX_ITER) break;

            Vector3 delta = dir * speed * Time.deltaTime;

            transform.position += delta;
            remainingDistance -= delta.magnitude;
            iter++;

            yield return null;
        }        
    }
}
