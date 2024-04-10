using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class Civilian : Triangle
{
    protected override IEnumerator Idle()
    {
        _isMoving = false;
        
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
            _isMoving = false;
            StateChange(EnemyState.Attack);
        }
        Vector2 newPos = (Vector2)transform.position + (runDir * _moveSpd * Time.deltaTime);
        transform.position = newPos;
    
        yield return null;
    }

    protected override IEnumerator Attack()
    {
        _isMoving = false;
        this.Log("dosth");
        StateChange(EnemyState.Idle);
        
        yield return null;
    }

    protected override IEnumerator Die()
    {
        Destroy(gameObject);
        
        yield return null;
    }

    protected override void TriangleAnim() 
    {
        _anim.SetBool("isRun", _isMoving);
        _anim.SetBool("isMeelee", _isMeeleeRange);
    }
}
