using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class Jumper : Triangle
{
    protected override IEnumerator Idle()
    {
        _isMoving = true;
        Move();

        // [TG] [2024-04-06] [Refactor]
        // 1. Raybox의 CheckWithRay에 시작 위치 매개변수를 추가하였으므로 같이 수정 
        // 2. 기존 Raybox의 trasnform.position이 (-0.5, 0, 0) 이고 BoxCollider의 Offset이 (0, 0) 임을 반영하여 변경
        // 3. 기존 값을 확인하는 것은 비효율적이기 때문에 참조를 할 수 있게 수정이 필요해 보임
        if (!_ray.CheckWithRay(transform.position + new Vector3(-0.5f, 0, 0), Vector2.down, 5  ) || 
             _ray.CheckWithRay(transform.position + new Vector3(-0.5f, 0, 0), _moveDir     , .5f))
        {
            _isMoving = false;
            yield return new WaitForSeconds(2f);
            TurnAround();
            _isMoving = true;
        }
        
        yield return null;
    }

    protected override IEnumerator Detect()
    {
        //방향 설정
        CalculateDir();

        //state가 바뀌지 않았다면 이동
        _isMoving = true;

        //이동 로직
        if (TargetDistance(_target) < _meeleeRange)
        {
            StateChange(EnemyState.Attack);
        }
        Vector2 newPos = (Vector2)transform.position - (runDir * _moveSpd * Time.deltaTime);
        transform.position = newPos;

        yield return null;
    }

    protected override IEnumerator Attack()
    {
        // 거리에 들어오면 수행할 동작
        _isMoving = true;
        
        yield return Jump();
    }

    protected override IEnumerator Die()
    {
        Destroy(gameObject);
        
        yield return null;
    }

    protected override void TriangleAnim() 
    {
        _anim.SetBool("isChase", _isMoving);
        _anim.SetBool("isMeelee", _isMeeleeRange);
    }
}
